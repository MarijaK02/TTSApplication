using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO.API;
using TTS.Domain.Shared;

namespace TTS.Service.Interface
{
    public interface IAdminService
    {
        public List<Client> GetAllClients();
        public List<Consultant> GetAllConsultants();
        public List<Project> GetAllProjects();
        public Client GetDetailsForClient(BaseEntity model);
        public Consultant GetDetailsForConsultant(BaseEntity model);
        public DetailsProjectDto GetDetailsForProject(BaseEntity model);
        public List<Activity> GetActivitiesForProject(BaseEntity model);
        public Activity GetActivityDetails(BaseEntity model);
        public List<ConsultantProject> GetAllConsultantsWorkingOnProject(Guid projectId);

        public bool EditClient(EditClientDto dto);
        public bool EditConsultant(EditConsultantDto dto);
        public bool EditProject(EditProjectDto dto);
        public bool EditActivity(EditActivityDto dto);

        public bool CreateProject(CreateProjectDto dto);
        public bool CreateActivity(CreateActivityDto dto);

        public bool DeleteProject(Guid projectId);
        public bool DeleteActivity(Guid actiivtyId);

        public bool DeleteConsultantFromProject(Guid consultantProjectId);
        public bool AddConsultantToProjectDto(AddConsultantToProjectDto dto);


        public bool AcceptApplication(Guid applicationId);
        public bool RejectApplication(Guid applicationId);
    }
}
