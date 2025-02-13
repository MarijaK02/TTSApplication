using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;

namespace TTS.Service.Interface
{
    public interface IProjectsService
    {
        public List<Project> GetAllProjects();
        public ProjectDetailsDto GetProjectDetails(Guid? projectId, bool isUserConsultant, string? userId);
        public Project Get(Guid projectId);
        public List<Project> GetProjectsForClient(string userId, string? searchTerm, Expertise? selectedExpertise);
        public List<Project> GetProjectsForConsultant(string userId, string? searchTerm);
        public List<Project> GetAllProjectsForApplication(string userId);
        public ConsultantApplicationsDto GetConsultantApplicationsFiltered(string userId, ApplicationStatus? status);
        public ConsultantProject GetApplication(Guid applicationId);

        public void CreateProject(CreateAndEditProjectDto dto, string userId);
        public void EditProject(Guid projectId, CreateAndEditProjectDto dto);
        public void DeleteProject(Guid projectId);
        public void ChangeProjectStatus(Guid projectId, ProjectStatus status);

        public void ApplyForProject(string userId, Guid? projectId, Guid? applicationId);

        public void AcceptApplication(Guid applicationId);
        public void RejectApplication(Guid applicationId);
        public void RemoveApplication(Guid applicationId);
    }
}
