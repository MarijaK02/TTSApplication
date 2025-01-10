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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<TTSApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProjectsService _projectsService;
        private readonly IUserService _userService;

        public ProjectsController(
            ApplicationDbContext context, 
            UserManager<TTSApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IProjectsService projectsService,
            IUserService userService
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _projectsService = projectsService;
            _userService = userService;
        }

        // GET: MyProjects
        [Route("MyProjects")]
        [Route("/")]
        public async Task<IActionResult> CustomIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Корисникот не е пронајден");
            }
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var projects = new List<GetProjectDto>();

            if (role!.Equals("Client"))
            {
                projects = _projectsService.GetProjectsForClient(user.Id)
                                       .Select(p => new GetProjectDto
                                       {
                                           Project = p,
                                           NumConsultants = _projectsService.TotalConsultantsWorkingOnProject(p.Id),
                                           TotalActivites = _projectsService.TotalActivitesForProject(p.Id),
                                           EndDate = p.EndDate
                                       }).ToList();
            }
            if (role!.Equals("Consultant"))
            {
                var consultant = _userService.GetConsultant(user.Id);
                projects = _projectsService.GetProjectsForConsultant(user.Id)
                                   .Select(p => new GetProjectDto
                                   {
                                       Project = p,
                                       NumConsultants = _projectsService.TotalConsultantsWorkingOnProject(p.Id),
                                       TotalActivites = _projectsService.TotalActivitesForProject(p.Id),
                                       NumOfConsultantActivites = _projectsService.TotalConsultantActivitesForProject(consultant.Id, p.Id),
                                       TotalHours = _projectsService.TotalProjectActiveHours(p)
                                   }).ToList();
            }
            return View(projects);
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
        

        // GET: Projects/Details/5
        public IActionResult Details(Guid id)
        {
            var project = _projectsService.GetProjectDetails(id);

            if (project == null)
            {
                return NotFound("Проектот не е пронајден");
            }

            var applications = _projectsService.GetApplicationsForProject(id);
            var responsibles = _projectsService.GetResponsiblesForProject(id);

            ProjectDetailsDto dto = new ProjectDetailsDto
            {
                Project = project,
                Applications = applications,
                Responsibles = responsibles
            };
                
            return View(dto);
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
                
                var client = _userService.GetClient(userId);
                if(client == null)
                {
                    return BadRequest();
                }

                _projectsService.CreateProject(dto, client);
                return RedirectToAction(nameof(CustomIndex));
                
                               
            }
            return View(dto);
        }

        // GET: Projects/Edit/5
        public IActionResult Edit(Guid id)
        {
            var project = _projectsService.GetProjectDetails(id);

            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            var dto = new CreateAndEditProjectDto
            {
                Id = project!.Id,
                Title = project.Title,
                Expertise = project.Expertise,
                ProjectStatus = project.Status,
                Description = project.Description
            };

            return View(dto);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, CreateAndEditProjectDto dto)
        {
            if (ModelState.IsValid)
            {
                var project = _projectsService.GetProjectDetails(id);

                if (project == null)
                {
                    return NotFound("Проектот не посоти");
                }

                _projectsService.EditProject(id, dto);
                
                return RedirectToAction(nameof(CustomIndex));
            }
            return View(dto);
        }

        // GET: Projects/Delete/5
        public IActionResult Delete(Guid id)
        {
            var project = _projectsService.GetProjectDetails(id);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            return View(project);
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            _projectsService.DeleteProject(id);

            return RedirectToAction(nameof(CustomIndex));
        }

        [HttpPost]
        [Authorize(Roles = "Consultant")]
        [ValidateAntiForgeryToken]
        //GET: Projects/ApplyForProject/id
        public IActionResult ApplyForProject(Guid id)
        {

            var project = _projectsService.GetProjectDetails(id);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Корисникот не е пронајден");
            }

            _projectsService.ApplyForProject(userId, id);
                
            return RedirectToAction(nameof(CustomIndex));
        }

        [Authorize(Roles = "Client")]
        public IActionResult AcceptApplication(Guid applicationId)
        {
            var application = _projectsService.GetApplicationsForProject(applicationId)
                .First(a => a.Id == applicationId);

            if (application == null)
            {
                return NotFound("Апликацијата не е пронајдена");
            }

            _projectsService.AcceptApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = application.Project.Id});
        }

        [Authorize(Roles = "Client")]
        public IActionResult RejectApplication(Guid applicationId)
        {
            var application = _projectsService.GetApplicationsForProject(applicationId)
                .First(a => a.Id == applicationId);

            if (application == null)
            {
                return NotFound("Апликацијата не е пронајдена");
            }

            _projectsService.RejectApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = application.Project.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(Guid projectId, ProjectStatus status)
        {
            var project = _projectsService.GetProjectDetails(projectId);
            if (project == null)
            {
                return NotFound("Проектот не посоти");
            }

            CreateAndEditProjectDto dto = new()
            {
                Title = project.Title,
                Expertise = project.Expertise,
                ProjectStatus = status,
                Description = project.Description,
                EndDate = project.EndDate
            };

            _projectsService.EditProject(projectId, dto);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }
    }
}
