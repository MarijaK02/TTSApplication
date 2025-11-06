using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using TTS.Service.Interface;
using System.Reflection.Metadata;

namespace TTS.Web.Controllers
{
    [Authorize(Roles = "Client, Consultant")]
    public class ProjectsController : Controller
    {
        private readonly UserManager<TTSApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProjectsService _projectsService;
        private readonly IUserService _userService;

        public ProjectsController(
            UserManager<TTSApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IProjectsService projectsService,
            IUserService userService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _projectsService = projectsService;
            _userService = userService;
        }

        [Route("MyProjects")]
        [Route("/")]
        public async Task<IActionResult> MyProjects(string? searchTerm, Expertise? selectedExpertise)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var projects = new List<Project>();

            if (role!.Equals("Client"))
            {
                projects = _projectsService.GetProjectsForClient(user.Id, searchTerm, selectedExpertise);
            }
            else if (role!.Equals("Consultant"))
            {
                projects = _projectsService.GetProjectsForConsultant(user.Id, searchTerm);               
            }

            var dto = new IndexProjectsDto
            {
                Projects = projects,
                SearchTerm = searchTerm,
                SelectedExpertise = selectedExpertise
            };

            return View(dto);
        }

        [Authorize(Roles = "Consultant")]
        [Route("AllProjects")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            if(userId == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            var projects = _projectsService.GetAllProjectsForApplication(userId);

            return View(projects);
        }

        [Authorize(Roles = "Consultant")]
        [Route("My Applications")]
        public IActionResult MyApplications(ApplicationStatus? status)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            var applicaitons = _projectsService.GetConsultantApplicationsFiltered(userId, status ?? ApplicationStatus.Applied);

            return View(applicaitons);
        }


        // GET: Projects/Details/5
        public IActionResult Details(Guid? id)
        {         
            if (id == null)
            {
                return NotFound("Проектот не е пронајден");
            }
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            var project = _projectsService.GetProjectDetails(id, User.IsInRole("Consultant"), userId);

            return View(project);
        }

        [Authorize(Roles = "Client")]
        // GET: Projects/Create
        public IActionResult Create()
        {
            CreateAndEditProjectDto dto = new CreateAndEditProjectDto
            { 
                Title = "",
                Expertise = Expertise.FrontEndDevelopment
            };
            return View(dto);
        }

        [Authorize(Roles = "Client")]
        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAndEditProjectDto dto)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if(userId == null)
                {
                    return NotFound("Коирсинкот не постои");
                }              

                _projectsService.CreateProject(dto, userId);
                return RedirectToAction(nameof(MyProjects));                                            
            }
            return View(dto);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Client")]
        public IActionResult Edit(Guid id)
        {
            var project = _projectsService.Get(id);

            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            var dto = new CreateAndEditProjectDto
            {
                Id = project!.Id,
                Title = project.Title,
                Expertise = project.Expertise,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };

            return View(dto);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public IActionResult Edit(Guid id, CreateAndEditProjectDto dto)
        {
            if (ModelState.IsValid)
            {
                var project = _projectsService.Get(id);

                if (project == null)
                {
                    return NotFound("Проектот не посоти");
                }

                _projectsService.EditProject(id, dto);
                
                return RedirectToAction(nameof(Details), new { id = id });
            }
            return View(dto);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var project = _projectsService.Get(id);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            _projectsService.DeleteProject(id);

            return RedirectToAction(nameof(MyProjects));
        }

        [HttpPost]
        [Authorize(Roles = "Consultant")]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyForProject(Guid projectId, Guid? applicationId)
        {         
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            _projectsService.ApplyForProject(userId, projectId, applicationId);
                
            return RedirectToAction(nameof(MyApplications), new { status = ApplicationStatus.Applied });
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AcceptApplication(Guid applicationId, Guid projectId)
        {
            _projectsService.AcceptApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectApplication(Guid applicationId, Guid projectId)
        {
            _projectsService.RejectApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [Authorize(Roles = "Consultant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveApplication(Guid applicationId)
        {
            _projectsService.RemoveApplication(applicationId);

            return RedirectToAction(nameof(MyApplications), new { status = ApplicationStatus.Applied });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(Guid projectId, ProjectStatus status)
        {
            var project = _projectsService.Get(projectId);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            _projectsService.ChangeProjectStatus(projectId, status);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }
    }
}
