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
using TTS.Domain.Shared;
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
        private readonly IEmailService _emailService;

        public ProjectsService(IRepository<Project> projectRepository, 
            IRepository<ConsultantProject> consultantProjectRepository, 
            IRepository<Activity> activitiesRepository, 
            IUserService userService,
            IEmailService emailService) 
        {
            _projectRepository = projectRepository;
            _activitiesRepository = activitiesRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _userService = userService;
             _emailService = emailService;
        }

        public List<Project> GetAllProjects()
        {
            return _projectRepository.GetAll().ToList();
        }

        public List<Project> GetProjectsForClient(string userId, string? searchTerm, Expertise? selectedExpertise)
        {
            var client = _userService.GetClient(userId);
                
            var projects = _projectRepository.GetAll()
                .Where(p => p.CreatedById == client.Id)
                .ToList();

            return FilterProjects(projects, searchTerm, selectedExpertise);
        }

        public List<Project> GetProjectsForConsultant(string userId, string? searchTerm)
        {
            var consultant = _userService.GetConsultant(userId);

            var projects = _projectRepository.GetAll()
                   .Include(p => p.ConsultantProjects)
                   .Where(p => p.ConsultantProjects!
                      .Any(cp => cp.ConsultantId.Equals(consultant.Id) && cp.ApplicationStatus == ApplicationStatus.Accepted)
                   )
                   .ToList();

            var consultantActivities = consultant.Projects?.Where(p => p.Activites != null && p.Activites.Any()).SelectMany(p => p.Activites).ToList();

            return FilterProjects(projects, searchTerm, null);
        }

        private int TotalConsultantWorkedHours(List<Activity> activities)
        {
            //zapocnata - kompletirana
            //Activity 1 01.02 - 02.02
            //Activity 2 04.02 - 08.02
            //Activity 3 05.02 - 06.02
            //Activity 4 07.02 - 10.02
            //Activity 5 15.02 - 17.02
            //Intervals: [ (01.02 - 02.02), (04.02 - 10.02) ]
            if (activities == null || activities.Count == 0)
            {
                return 0;
            }

            var sortedActivities = activities.Where(a => a.StartDate < DateTime.Now).OrderBy(a => a.StartDate).ToList();

            if (!sortedActivities.Any())
            {
                return 0;
            }

            var intervals = new List<Interval>();

            intervals.Add(new Interval
            {
                From = sortedActivities[0].StartDate,
                To = sortedActivities[0].CompletedOn ?? DateTime.Now
            });

            for (int i = 1; i < sortedActivities.Count; i++)
            {
                var activity = sortedActivities[i];
                var lastInterval = intervals.Last();

                if (activity.StartDate <= lastInterval.To)
                {
                    if (activity.EndDate > lastInterval.To)
                    {
                        lastInterval.To = sortedActivities[i].CompletedOn ?? DateTime.Now;
                    }
                }
                else
                {
                    intervals.Add(new Interval
                    {
                        From = activity.StartDate,
                        To = sortedActivities[i].CompletedOn ?? DateTime.Now
                    });
                }
            }

            var total = intervals.Sum(i => (i.To - i.From).TotalHours);

            return (int)Math.Round(total);

        }

        private int TotalConsultantsWorkingOnProject(Guid projectId)
        {
            var cps = _consultantProjectRepository.GetAll()
                .Where(cp => cp.ProjectId == projectId)
                .ToList()
                .Where(cp => cp.ApplicationStatus == ApplicationStatus.Accepted);

            return cps.Where(cp => cp.ApplicationStatus == ApplicationStatus.Accepted).Count();
        }

        private int TotalActivitesForProject(Guid? projectId)
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
                .First(cp => cp.ProjectId == projectId && cp.ConsultantId == consultantId)
                .Activites
                .Count();
        }

        private int TotalProjectHours(Project p)
        {
            if(p.Status == ProjectStatus.Invalid || p.Status == ProjectStatus.Completed)
            {
                return (int)(p.EndDate - p.StartDate).TotalHours;
            }
            if(p.Status == ProjectStatus.New)
            {
                return 0;
            }
            return (int)(DateTime.Now - p.StartDate).TotalHours;
        }

        private int TotalProjectExpectedHours(Project p)
        {
            return (int)(p.EndDate - p.StartDate).TotalHours;
        }

        public List<Project> GetAllProjectsForApplication(string userId)
        {
            var consultant = _userService.GetConsultant(userId);

            var projects = _projectRepository.GetAll()
                .Include(p => p.ConsultantProjects)
                .Where(p => p.Expertise == consultant.Expertise && !p.ConsultantProjects!.Any(cp => cp.ConsultantId == consultant.Id) && p.Status == ProjectStatus.New)
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

        public ProjectDetailsDto GetProjectDetails(Guid? projectId, bool isUserConsultant, string? userId)
        {
            var project = _projectRepository.Get(projectId);

            int numConsultantActivities = -1;
            int totalWorkedHours = -1;

            if (isUserConsultant && userId != null)
            {
                var consultant = _userService.GetConsultant(userId);

                var consultantActivities = _consultantProjectRepository.GetAll()
                                        .Include(cp => cp.Activites)
                                        .First(cp => cp.ConsultantId == consultant.Id && cp.ProjectId == projectId)
                                        .Activites
                                        .ToList();

                numConsultantActivities = consultantActivities.Count();
                totalWorkedHours = TotalConsultantWorkedHours(consultantActivities);
            }

            var responsibles = GetResponsiblesForProject(projectId);

            ProjectDetailsDto dto = new ProjectDetailsDto
            {
                Project = project,
                Applications = GetApplicationsForProject(projectId),
                Responsibles = responsibles,
                TotalExpectedHours = TotalProjectExpectedHours(project),
                TotalHours = TotalProjectHours(project),
                TotalActivites = TotalActivitesForProject(projectId),
                NumOfConsultantActivites = numConsultantActivities,
                TotalConsultantWorkedHours = totalWorkedHours
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

        public void CreateProject (CreateAndEditProjectDto dto, string userId)
        {
            var client = _userService.GetClient(userId);

            var project = new Project
            {
                Title = dto.Title,
                Expertise = dto.Expertise,
                Description = dto.Description,
                CreatedOn = DateTime.Now,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
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
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;

            _projectRepository.Update(project);
        }

        public void ChangeProjectStatus(Guid projectId, ProjectStatus status)
        {
            var project = _projectRepository.Get(projectId);
            project.Status = status;
            if(project.Status == ProjectStatus.Completed)
            {
                project.CompletedOn = DateTime.Now;
            }
            else
            {
                project.CompletedOn = null;
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

                    _emailService.SendEmailAsync(project.CreatedBy.User.Email, project.CreatedBy.User.FirstName, project.CreatedBy.User.LastName,
                        "Известување за апликации на проект " + project.Title,
                        $"Почитуван/а {project.CreatedBy.User.FirstName} {project.CreatedBy.User.LastName}, \r\n\r\nНов консултант, {consultant.User.FirstName} {consultant.User.LastName}, аплицираше да се придружи на вашиот проект „{project.Title}“.\r\n\r\nВе молиме разгледајте ја неговата апликација и одлучете дали да ја прифатите или одбиете.\r\n\r\nВи благодариме за вашето внимание.");
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

            _emailService.SendEmailAsync(application.Consultant!.User!.Email, application.Consultant.User.FirstName, application.Consultant.User.LastName,
                "Известување за вашата апликација на проектот" + application.Project.Title, 
                $"Вашата апликација за проектот '{application.Project!.Title}' е прифатена. Вашите апликации можете да ги прегледате во делот Мои Проекти. Ви благодариме за разбирањето и соработката.");
        }

        public void RejectApplication(Guid applicationId)
        {
            var application = _consultantProjectRepository.Get(applicationId);

            application.ApplicationStatus = ApplicationStatus.Rejected;

            _consultantProjectRepository.Update(application);

            _emailService.SendEmailAsync(application.Consultant!.User!.Email, application.Consultant.User.FirstName, application.Consultant.User.LastName,
                "Известување за вашата апликација на проектот" + application.Project.Title,
                $"Вашата апликација за проектот '{application.Project!.Title}' е одбиена. Вашите апликации можете да ги прегледате во делот Мои Проекти. Ви благодариме за разбирањето и соработката.");        
        }

        public void RemoveApplication(Guid applicationId)
        {
            var application = _consultantProjectRepository.Get(applicationId);

            _consultantProjectRepository.Delete(application);
        }

        private List<Project> FilterProjects(List<Project> rawProjects, string? searchTerm, Expertise? selectedExpertise)
        {
            var result = rawProjects;

            if(searchTerm != null)
            {
                result = result.Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if(selectedExpertise != null)
            {
                result = result.Where(p => p.Expertise == selectedExpertise).ToList();
            }

            return result;
        }     
    }
}
