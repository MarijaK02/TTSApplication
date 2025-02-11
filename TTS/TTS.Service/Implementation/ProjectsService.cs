using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Repository;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class ProjectsService : IProjectsService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Activity> _activitiesRepository;
        private readonly IRepository<ConsultantProject> _consultantProjectRepository;
        private readonly IUserService _userService;

        public ProjectsService(IRepository<Project> projectRepository, 
            IRepository<ConsultantProject> consultantProjectRepository, 
            IRepository<Activity> activitiesRepository, 
            IUserService userService)
        {
            _projectRepository = projectRepository;
            _activitiesRepository = activitiesRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _userService = userService;
        }

        public List<Project> GetAllProjects()
        {
            return _projectRepository.GetAll().ToList();
        }

        public List<GetProjectDto> GetProjectsForClient(string userId, string? searchTerm, List<Expertise>? expertiseList)
        {
            var client = _userService.GetClient(userId);
                
            var projects = _projectRepository.GetAll()
                .Where(p => p.CreatedById == client.Id)
                .ToList();

            var dto = FilterProjects(projects, searchTerm, expertiseList)
                                    .Select(p => new GetProjectDto
                                    {
                                        Project = p,
                                        NumConsultants = TotalConsultantsWorkingOnProject(p.Id),
                                        TotalActivites = TotalActivitesForProject(p.Id),
                                        EndDate = p.EndDate
                                    }).ToList();
            return dto;
        }

        public List<GetProjectDto> GetProjectsForConsultant(string userId, string? searchTerm)
        {
            var dto = new List<GetProjectDto>();
            var consultant = _userService.GetConsultant(userId);

            if (consultant!.Projects != null)
            {
                var projects = _projectRepository.GetAll()
                    .Include(p => p.ConsultantProjects)
                    .Where(p => p.ConsultantProjects!
                       .Any(cp => cp.ConsultantId.Equals(consultant.Id) && cp.ApplicationStatus == ApplicationStatus.Accepted)
                    )
                    .ToList();

                dto = FilterProjects(projects, searchTerm, null)
                                    .Select(p => new GetProjectDto
                                    {
                                        Project = p,
                                        NumConsultants = TotalConsultantsWorkingOnProject(p.Id),
                                        TotalActivites = TotalActivitesForProject(p.Id),
                                        NumOfConsultantActivites = TotalConsultantActivitesForProject(consultant.Id, p.Id),
                                        TotalHours = TotalProjectActiveHours(p),
                                        TotalExpectedHours = TotalProjectExpectedHours(p)
                                    }).ToList();
            }

            return dto;
        }

        private int TotalConsultantsWorkingOnProject(Guid projectId)
        {
            var p = _projectRepository.GetAll()
                .Include(p => p.ConsultantProjects)
                .First(p => p.Id == projectId);

            return p.ConsultantProjects != null ? p.ConsultantProjects.Where(cp => cp.ApplicationStatus == ApplicationStatus.Accepted).Count() : 0;
        }

        private int TotalActivitesForProject(Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Include(cp => cp.Activites)
                .Where(cp => cp.ProjectId == projectId)
                .SelectMany(cp => cp.Activites)
                .Count();
        }

        private int TotalConsultantActivitesForProject(Guid consultantId, Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Include(cp => cp.Activites)
                .Where(cp => cp.ProjectId == projectId && cp.ConsultantId == consultantId)
                .Select(cp => cp.Activites)
                .Count();
        }

        private int TotalProjectActiveHours(Project p)
        {
            return (int)((TimeSpan)(DateTime.Now - p.StartDate)).TotalHours;
        }

        private int TotalProjectExpectedHours(Project p)
        {
            return p.EndDate !=null ? (int)((TimeSpan)(p.EndDate - p.StartDate)).TotalHours : 0;
        }

        public List<Project> GetAllProjectsForApplication(string userId)
        {
            var consultant = _userService.GetConsultant(userId);

            var projects = _projectRepository.GetAll()
                .Include(p => p.ConsultantProjects)
                .Where(p => p.Expertise == consultant.Expertise && !p.ConsultantProjects!.Any(cp => cp.ConsultantId == consultant.Id))
                .ToList();             

            return projects ?? [];
        }

        public ConsultantApplicationsDto GetConsultantApplicationsFiltered(string userId, ApplicationStatus? status)
        {
            var consultant = _userService.GetConsultant(userId);

            var projects = _consultantProjectRepository.GetAll()
                .Include(cp => cp.Project)
                .Where(cp => cp.ConsultantId == consultant.Id && cp.ApplicationStatus == status)
                .ToList();

            var dto = new ConsultantApplicationsDto
            {
                Status = status ?? ApplicationStatus.Applied,
                Applications = projects
            };

            return dto;
        }

        public Project Get(Guid projectId)
        {
            return _projectRepository.Get(projectId);
        }

        public ProjectDetailsDto GetProjectDetails(Guid? projectId)
        {
            var project = _projectRepository.Get(projectId);

            var applications = GetApplicationsForProject(projectId);
            var responsibles = GetResponsiblesForProject(projectId);
            var totalActiveHours = TotalProjectActiveHours(project);
            var totalExpectedHours = TotalProjectExpectedHours(project);


            ProjectDetailsDto dto = new ProjectDetailsDto
            {
                Project = project,
                Applications = applications,
                Responsibles = responsibles,
                TotalExpectedHours = totalExpectedHours,
                TotalHours = totalActiveHours
            };

            return dto;
        }


        private List<ConsultantProject> GetApplicationsForProject (Guid? projectId)
        {
            return projectId == null ? [] :_consultantProjectRepository.GetAll()
                .Include(cp => cp.Consultant)
                .Include("Consultant.User")
                .Where(cp => cp.ProjectId == projectId && cp.ApplicationStatus == ApplicationStatus.Applied)
                .ToList() ?? [];
        }

        private List<Consultant> GetResponsiblesForProject(Guid? projectId)
        {
            return projectId == null ? [] : _consultantProjectRepository.GetAll()
                .Include(cp => cp.Consultant)
                .Include("Consultant.User")
                .Where(cp => cp.ProjectId == projectId && cp.ApplicationStatus == ApplicationStatus.Accepted)
                .Select(cp => cp.Consultant)
                .ToList();
        }

        public void CreateProject (CreateAndEditProjectDto dto, Client client)
        {
            var project = new Project
            {
                Title = dto.Title,
                Expertise = dto.Expertise,
                Description = dto.Description,
                StartDate = DateTime.Now,
                EndDate = dto.EndDate ?? null,
                Status = ProjectStatus.New,
                CreatedById = client.Id,
                ConsultantProjects = new List<ConsultantProject>()
            };

            _projectRepository.Insert(project);
        }

        public void EditProject(Guid projectId, CreateAndEditProjectDto dto)
        {
            var project = _projectRepository.Get(projectId);

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Expertise = dto.Expertise;
            project.EndDate = dto.EndDate;

            _projectRepository.Update(project);
        }

        public void ChangeProjectStatus(Guid projectId, ProjectStatus status)
        {
            var project = _projectRepository.Get(projectId);
            project.Status = status;

            if (project.Status == ProjectStatus.Invalid || project.Status == ProjectStatus.Completed)
            {
                project.EndDate = DateTime.Now;
            }
            _projectRepository.Update(project);
        }

        public void DeleteProject(Guid projectId)
        {
            var project = _projectRepository.Get(projectId);
            _projectRepository.Delete(project);
        }

        public void ApplyForProject(string userId, Guid? projectId, Guid? applicationId)
        {
            if(applicationId != null)
            {
                var application = _consultantProjectRepository.Get(applicationId.Value);
                application.ApplicationStatus = ApplicationStatus.Applied;

                _consultantProjectRepository.Update(application);
            }
            else
            {
                if(projectId != null)
                {
                    var consultant = _userService.GetConsultant(userId);
                    var project = _projectRepository.Get(projectId.Value);

                    var application = new ConsultantProject
                    {
                        Id = Guid.NewGuid(),
                        ConsultantId = consultant.Id,
                        ProjectId = projectId.Value,
                        DateApplied = DateTime.Now,
                        ApplicationStatus = ApplicationStatus.Applied,
                        Activites = new List<Activity>()
                    };

                    _consultantProjectRepository.Insert(application);

                    project.ConsultantProjects?.Add(application);

                    _projectRepository.Update(project);
                }                
            }           
        }

        public ConsultantProject GetApplication(Guid applicationId)
        {
            return _consultantProjectRepository.Get(applicationId);
        }

        public void AcceptApplication(Guid applicationId)
        {
            var application = _consultantProjectRepository.Get(applicationId);


            application.ApplicationStatus = ApplicationStatus.Accepted;
            
            _consultantProjectRepository.Update(application);
        }

        public void RejectApplication(Guid applicationId)
        {
            var application = _consultantProjectRepository.Get(applicationId);

            application.ApplicationStatus = ApplicationStatus.Rejected;

            _consultantProjectRepository.Update(application);
        }

        public void RemoveApplication(Guid applicationId)
        {
            var application = _consultantProjectRepository.Get(applicationId);

            _consultantProjectRepository.Delete(application);
        }

        private List<Project> FilterProjects(List<Project> rawProjects, string? searchTerm, List<Expertise>? expertiseList)
        {
            return rawProjects
                .Where(p =>
                    (string.IsNullOrEmpty(searchTerm) || p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                    (expertiseList == null || !expertiseList.Any() || expertiseList.Contains(p.Expertise))
                )
                .ToList();
        }     
    }
}
