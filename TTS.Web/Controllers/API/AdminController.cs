using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTS.Domain.Domain;
using TTS.Service.Interface;

namespace TTS.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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

    }
}
