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
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Activity> _activitiesRepository;
        private readonly IRepository<ConsultantProject> _consultantProjectRepository;
        private readonly IUserService _userService;

        public ProjectsService(ApplicationDbContext context, 
            IRepository<Project> projectRepository, 
            IRepository<ConsultantProject> consultantProjectRepository, 
            IRepository<Activity> activitiesRepository, 
            IUserService userService)
        {
            _context = context;
            _projectRepository = projectRepository;
            _activitiesRepository = activitiesRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _userService = userService;
        }

        public List<Project> GetProjectsForClient(string userId)
        {
            var client = _userService.GetClient(userId);
                
            return _projectRepository.GetAll()
                .Where(p => p.CreatedBy.Id == client.Id)
                .ToList();            
        }

        public List<Project> GetProjectsForConsultant(string userId)
        {
            var projects = new List<Project>();
            var consultant = _userService.GetConsultant(userId);

            if (consultant!.Projects != null)
            {
                projects = _projectRepository.GetAll()
                    .Include(p => p.ConsultantProjects)
                    .Where(p => p.ConsultantProjects!
                       .Any(cp => cp.Consultant.Id.Equals(consultant.Id) && cp.ApplicationStatus == ApplicationStatus.Accepted)
                    )
                    .ToList();
            }

            return projects;
        }

        public int TotalConsultantsWorkingOnProject(Guid projectId)
        {
            var p = _projectRepository.GetAll()
                .Include(p => p.ConsultantProjects)
                .First(p => p.Id == projectId);

            return p.ConsultantProjects != null ? p.ConsultantProjects.Count() : 0;
        }

        public int TotalActivitesForProject(Guid projectId)
        {
            return _activitiesRepository.GetAll()
                .Where(a => a.ConsultantProject.Project.Id == projectId)
                .Count();
        }

        public int TotalConsultantActivitesForProject(Guid consultantId, Guid projectId)
        {
            return _activitiesRepository.GetAll()
                .Where(a => a.ConsultantProject.Project.Id == projectId && a.ConsultantProject.Consultant.Id == consultantId)
                .Count();
        }

        public int TotalProjectActiveHours(Project p)
        {
            return (int)((TimeSpan)(DateTime.Now - p.StartDate)).TotalHours;
        }

        public List<Project> GetAllProjectsForApplication(string userId)
        {
            var consultant = _userService.GetConsultant(userId);

            var projects = _projectRepository.GetAll()
                .Include(p => p.ConsultantProjects)
                .Where(p => p.Expertise == consultant.Expertise && !p.ConsultantProjects!.Any(cp => cp.Consultant.Id == consultant.Id))
                .ToList();             

            return projects ?? [];
        }

        public Project GetProjectDetails(Guid projectId)
        {
            return _projectRepository.Get(projectId);
        }


        public List<ConsultantProject> GetApplicationsForProject (Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Where(cp => cp.Project.Id == projectId && cp.ApplicationStatus == ApplicationStatus.Applied)
                .ToList();
        }

        public List<Consultant> GetResponsiblesForProject(Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Where(cp => cp.Project.Id == projectId)
                .Select(cp => cp.Consultant)
                .ToList() ?? [];
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
                CreatedBy = client
            };

            _projectRepository.Insert(project);
        }

        public void EditProject(Guid projectId, CreateAndEditProjectDto dto)
        {
            var project = _projectRepository.Get(projectId);

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Expertise = dto.Expertise;
            project.Status = dto.ProjectStatus ?? project.Status;

            _projectRepository.Update(project);
        }

        public void DeleteProject(Guid projectId)
        {
            var project = _projectRepository.Get(projectId);
            _projectRepository.Delete(project);
        }

        public void ApplyForProject(string userId, Guid projectId)
        {
            var consultant = _userService.GetConsultant(userId);
            var project = _projectRepository.Get(projectId);

            var application = new ConsultantProject
            {
                Id = Guid.NewGuid(),
                Consultant = consultant,
                Project = project,
                DateApplied = DateTime.Now,
                ApplicationStatus = ApplicationStatus.Applied
            };

            if(project.ConsultantProjects == null)
            {
                project.ConsultantProjects = new List<ConsultantProject>();
            }
            
            project.ConsultantProjects.Add(application);

            _consultantProjectRepository.Insert(application);
        }

        public void AcceptApplication(Guid applicationId)
        {
            var application = GetApplicationsForProject(applicationId)
                .First(a => a.Id == applicationId);


            application.ApplicationStatus = ApplicationStatus.Accepted;
            
            _consultantProjectRepository.Update(application);
        }

        public void RejectApplication(Guid applicationId)
        {
            var application = GetApplicationsForProject(applicationId)
                .First(a => a.Id == applicationId);

            application.ApplicationStatus = ApplicationStatus.Rejected;

            _consultantProjectRepository.Update(application);
        }


    }
}
