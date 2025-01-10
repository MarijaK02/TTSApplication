using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO;

namespace TTS.Service.Interface
{
    public interface IProjectsService
    {
        public List<Project> GetProjectsForClient(string userId);

        public List<Project> GetProjectsForConsultant(string userId);

        public int TotalConsultantsWorkingOnProject(Guid projectId);
        public int TotalActivitesForProject(Guid projectId);
        public int TotalConsultantActivitesForProject(Guid consultantId, Guid projectId);

        public int TotalProjectActiveHours(Project p);

        public List<Project> GetAllProjectsForApplication(string userId);
        public Project GetProjectDetails(Guid projectId);
        public List<ConsultantProject> GetApplicationsForProject(Guid projectId);
        public List<Consultant> GetResponsiblesForProject(Guid projectId);
        public void CreateProject(CreateAndEditProjectDto dto, Client client);
        public void EditProject(Guid projectId, CreateAndEditProjectDto dto);
        public void DeleteProject(Guid projectId);
        public void ApplyForProject(string userId, Guid projectId);
        public void AcceptApplication(Guid applicationId);
        public void RejectApplication(Guid applicationId);
    }
}
