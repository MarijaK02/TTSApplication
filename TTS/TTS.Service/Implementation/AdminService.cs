using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO.API;
using TTS.Domain.Shared;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ConsultantProject> _consultantProjectRepository;
        private readonly IRepository<Activity> _activityRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IRepository<Consultant> _consultantRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IProjectsService _projectsService;

        public AdminService(IRepository<Project> projectRepository, 
            IRepository<ConsultantProject> consultantProjectRepository, 
            IRepository<Activity> activityRepository,
            IRepository<Comment> commentRepository,
            IRepository<Attachment> attachmentRepository,
            IRepository<Consultant> consultantRepository,
            IRepository<Client> clientRepository,
            IProjectsService projectsService)
        {
            _projectRepository = projectRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _activityRepository = activityRepository;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
            _consultantRepository = consultantRepository;
            _clientRepository = clientRepository;
            _projectsService = projectsService;
        }


        public List<Client> GetAllClients()
        {
            return _clientRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .ToList();
        }

        public List<Consultant> GetAllConsultants()
        {
            return _consultantRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .Include("Projects.Project")
                .ToList();
        }

        public List<Project> GetAllProjects()
        {
            return _projectRepository.GetAll()
                .Include(p => p.CreatedBy)
                .Include("CreatedBy.User")
                .Include(p => p.ConsultantProjects)
                .ToList();
        }

        public Client GetDetailsForClient(BaseEntity model)
        {
            return _clientRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .First(c => c.Id == model.Id);                
        }

        public Consultant GetDetailsForConsultant(BaseEntity model)
        {
            return _consultantRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .Include("Projects.Project")
                .First(c => c.Id == model.Id);
        }

        public DetailsProjectDto GetDetailsForProject(BaseEntity model)
        {
            var project = _projectRepository.GetAll()
                .Include(p => p.CreatedBy)
                .Include("CreatedBy.User")
                .Include(p => p.ConsultantProjects)
                .Include("ConsultantProjects.Consultant")
                .Include("ConsultantProjects.Consultant.User")
                .First(p => p.Id == model.Id);

            var consultants = _consultantRepository.GetAll()
                .Include(c => c.User)
                .Where(c => c.Expertise == project.Expertise)
                .ToList();

            var dto = new DetailsProjectDto
            {
                Project = project,
                Consultants = consultants
            };

            return dto;
        }

        public List<Activity> GetActivitiesForProject(BaseEntity model)
        {
            return _activityRepository.GetAll()
                .Include(a => a.ConsultantProject)
                .Where(a => a.ConsultantProject!.ProjectId == model.Id)                
                .ToList();
        }

        public Activity GetActivityDetails(BaseEntity model)
        {
            return _activityRepository.GetAll()
                .Include(a => a.ConsultantProject)
                .Include("ConsultantProject.Consultant")
                .Include("ConsultantProject.Consultant.User")
                .First(a => a.Id == model.Id);
        }

        public bool EditClient(EditClientDto dto)
        {
            var client = _clientRepository.GetAll()
                .Include(c => c.User)
                .First(c => c.Id == dto.ClientId);

            if(client == null)
            {
                return false;
            }

            client.User.FirstName = dto.FirstName;
            client.User.LastName = dto.LastName;
            client.User.Email = dto.Email;
            client.User.PhoneNumber = dto.PhoneNumber;
            client.Address = dto.Address;
            client.Industry = dto.Industry;

            _clientRepository.Update(client);

            return true;
        }

        public bool EditConsultant(EditConsultantDto dto)
        {
            var consultant = _consultantRepository.GetAll()
                .Include(c => c.User)
                .First(c => c.Id == dto.ConsultantId);

            if (consultant == null)
            {
                return false;
            }

            consultant.User.FirstName = dto.FirstName;
            consultant.User.LastName = dto.LastName;
            consultant.User.Email = dto.Email;
            consultant.User.PhoneNumber = dto.PhoneNumber;
            consultant.Expertise = dto.Expertise;

            _consultantRepository.Update(consultant);

            return true;
        }

        public bool EditProject(EditProjectDto dto)
        {
            var project = _projectRepository.Get(dto.ProjectId);

            if (project == null)
            {
                return false;
            }

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.Expertise = dto.Expertise;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;

            _projectRepository.Update(project);

            return true;
        }

        public bool EditActivity(EditActivityDto dto)
        {
            var activity = _activityRepository.Get(dto.Id);

            if (activity == null)
            {
                return false;
            }

            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.Status = dto.Status;
            activity.StartDate = dto.StartDate;
            activity.EndDate = dto.EndDate;

            _activityRepository.Update(activity);

            return true;
        }

        public bool DeleteConsultantFromProject(Guid consultantProjectId)
        {
            var cp = _consultantProjectRepository.Get(consultantProjectId);

            if(cp == null)
            {
                return false;
            }

            _consultantProjectRepository.Delete(cp);
            return true;
        }

        public bool CreateProject(CreateProjectDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Expertise = dto.Expertise,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedById = dto.CreatedById
            };

            _projectRepository.Insert(project);

            return true;
        }

        public List<ConsultantProject> GetAllConsultantsWorkingOnProject(Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Include(c => c.Consultant)
                .Include("Consultant.User")
                .Where(cp => cp.ProjectId == projectId && cp.ApplicationStatus == Domain.Enum.ApplicationStatus.Accepted)
                .ToList();
        }

        public bool AddConsultantToProjectDto(AddConsultantToProjectDto dto)
        {
            var consultantProject = new ConsultantProject
            {
                Id = Guid.NewGuid(),
                ConsultantId = dto.ConsultantId,
                ProjectId = dto.ProjectId,
                Activites = new List<Activity>(),
                DateApplied = DateTime.Now,
                DateModified = DateTime.Now,
                ApplicationStatus = Domain.Enum.ApplicationStatus.Accepted
            };

            _consultantProjectRepository.Insert(consultantProject);
            return true;
        }

        public bool CreateActivity(CreateActivityDto dto)
        {
            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                ConsultantProjectId = dto.ConsultantProjectId,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Comments = new List<Comment>()
            };

            _activityRepository.Insert(activity);
            return true;
        }

        public bool AcceptApplication(Guid applicationId)
        {
            _projectsService.AcceptApplication(applicationId);
            return true;
        }

        public bool RejectApplication(Guid applicationId) 
        { 
            _projectsService.RejectApplication(applicationId);
            return true;
        }

        public bool DeleteProject(Guid projectId)
        {
            _projectsService.DeleteProject(projectId);
            return true;
        }

        public bool DeleteActivity(Guid actiivtyId)
        {
            var activity = _activityRepository.Get(actiivtyId);
            _activityRepository.Delete(activity);
            return true;
        }
    }
}
