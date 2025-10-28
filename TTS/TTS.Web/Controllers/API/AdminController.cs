using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TTS.Domain.Domain;
using TTS.Domain.DTO.API;
using TTS.Domain.Identity;
using TTS.Domain.Shared;
using TTS.Service.Interface;

namespace TTS.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly SignInManager<TTSApplicationUser> _signInManager;

        public AdminController(IAdminService adminService, SignInManager<TTSApplicationUser> signInManager)
        {
            _adminService = adminService;
            _signInManager = signInManager;
        }        


        [HttpGet("[action]")]
        public List<Client> GetAllClients()
        {
            return _adminService.GetAllClients();
        }

        [HttpGet("[action]")]
        public List<Consultant> GetAllConsultants()
        {
            return _adminService.GetAllConsultants();
        }

        [HttpGet("[action]")]
        public List<Project> GetAllProjects()
        {
            return _adminService.GetAllProjects();
        }

        [HttpPost("[action]")]
        public Client GetDetailsForClient(BaseEntity model)
        {
            return _adminService.GetDetailsForClient(model);
        }

        [HttpPost("[action]")]
        public Consultant GetDetailsForConsultant(BaseEntity model)
        {
            return _adminService.GetDetailsForConsultant(model);
        }

        [HttpPost("[action]")]
        public DetailsProjectDto GetDetailsForProject(BaseEntity model)
        {
            return _adminService.GetDetailsForProject(model);
        }

        [HttpGet("[action]")]
        public List<Activity> GetAllActivities()
        {
            return _adminService.GetAllActivities();
        }

        [HttpPost("[action]")]
        public ActivitiesDto GetActivitiesForProject(BaseEntity model)
        {
            return _adminService.GetActivitiesForProject(model);
        }

        [HttpPost("[action]")]
        public Activity GetActivityDetails(BaseEntity model)
        {
            return _adminService.GetActivityDetails(model);
        }

        [HttpPut("[action]")]
        public bool EditClient(EditClientDto clientDto)
        {
            if (clientDto != null && ModelState.IsValid)
            {
                return _adminService.EditClient(clientDto);
            }

            return false;
        }

        [HttpPut("[action]")]
        public bool EditConsultant(EditConsultantDto consultantDto)
        {
            if (consultantDto != null && ModelState.IsValid)
            {
                return _adminService.EditConsultant(consultantDto);
            }

            return false;
        }

        [HttpPut("[action]")]
        public bool EditProject(EditProjectDto projectDto)
        {
            if (projectDto != null && ModelState.IsValid)
            {

                return _adminService.EditProject(projectDto);
            }

            return false;
        }

        [HttpPut("[action]")]
        public bool EditActivity(EditActivityDto activityDto)
        {
            if (activityDto != null && ModelState.IsValid)
            {
                return _adminService.EditActivity(activityDto);
            }

            return false;
        }

        [HttpPost("[action]")]
        public bool DeleteConsultantProject(BaseEntity model)
        {
            return _adminService.DeleteConsultantProject(model.Id);
        }

        [HttpPost("[action]")]
        public bool CreateProject(CreateProjectDto projectDto)
        {
            if (projectDto != null && ModelState.IsValid)
            {
                return _adminService.CreateProject(projectDto);
            }

            return false;
        }

        [HttpPost("[action]")]
        public List<ConsultantProject> GetAllConsultantsWorkingOnProject(BaseEntity model)
        {
            return _adminService.GetAllConsultantsWorkingOnProject(model.Id);
        }

        [HttpPost("[action]")]
        public bool AddConsultantToProject(AddConsultantToProjectDto dto)
        {
            if (dto != null && ModelState.IsValid)
            {
                return _adminService.AddConsultantToProjectDto(dto);
            }

            return false;
        }


        [HttpPost("[action]")]
        public bool CreateActivity(CreateActivityDto dto)
        {
            if (dto != null && ModelState.IsValid)
            {
                return _adminService.CreateActivity(dto);
            }

            return false;
        }

        [HttpPost("[action]")]
        public bool AcceptApplication(BaseEntity model)
        {
            return _adminService.AcceptApplication(model.Id);
        }

        [HttpPost("[action]")]
        public bool RejectApplication(BaseEntity model)
        {
            return _adminService.RejectApplication(model.Id);
        }

        [HttpPost("[action]")]
        public bool DeleteProject(BaseEntity model)
        {
            return _adminService.DeleteProject(model.Id);
        }

        [HttpPost("[action]")]
        public bool DeleteActivity(BaseEntity model)
        {
            return _adminService.DeleteActivity(model.Id);
        }

        [HttpGet]
        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
