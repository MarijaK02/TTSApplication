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

        [Route("MyProjects")]
        [Route("/")]
        public async Task<IActionResult> MyProjects(string? searchTerm, List<Expertise>? expertiseList)
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
                projects = _projectsService.GetProjectsForClient(user.Id);
                var dto = _projectsService.FilterProjects(projects, searchTerm, expertiseList)
                                    .Select(p => new GetProjectDto
                                    {
                                        Project = p,
                                        NumConsultants = _projectsService.TotalConsultantsWorkingOnProject(p.Id),
                                        TotalActivites = _projectsService.TotalActivitesForProject(p.Id),
                                        EndDate = p.EndDate
                                    }).ToList();

                return View(dto);
            }
            else if (role!.Equals("Consultant"))
            {
                var consultant = _userService.GetConsultant(user.Id);
                projects = _projectsService.GetProjectsForConsultant(user.Id);
                var dto = _projectsService.FilterProjects(projects, searchTerm, expertiseList)
                                    .Select(p => new GetProjectDto
                                    {
                                        Project = p,
                                        NumConsultants = _projectsService.TotalConsultantsWorkingOnProject(p.Id),
                                        TotalActivites = _projectsService.TotalActivitesForProject(p.Id),
                                        NumOfConsultantActivites = _projectsService.TotalConsultantActivitesForProject(consultant.Id, p.Id),
                                        TotalHours = _projectsService.TotalProjectActiveHours(p),
                                        TotalExpectedHours = _projectsService.TotalProjectExpectedHours(p)
                                    }).ToList();

                return View(dto);
            }

            return View(null);
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

            var dto = new ConsultantApplicationsDto
            {
                Status = status ?? ApplicationStatus.Applied,
                Applications = _projectsService.GetConsultantApplicationsFiltered(userId, status ?? ApplicationStatus.Applied) ?? []
            };

            return View(dto);
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
            var totalActiveHours = _projectsService.TotalProjectActiveHours(project);
            var totalExpectedHours = _projectsService.TotalProjectExpectedHours(project);


            ProjectDetailsDto dto = new ProjectDetailsDto
            {
                Project = project,
                Applications = applications,
                Responsibles = responsibles,
                TotalExpectedHours = totalExpectedHours,
                TotalHours = totalActiveHours
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
                return RedirectToAction(nameof(MyProjects));                                            
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
                
                return RedirectToAction(nameof(MyProjects));
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

            return RedirectToAction(nameof(MyProjects));
        }

        [HttpPost]
        [Authorize(Roles = "Consultant")]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyForProject(Guid? projectId, Guid? applicationId)
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
            var application = _projectsService.GetApplication(applicationId);

            _projectsService.AcceptApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectApplication(Guid applicationId, Guid projectId)
        {
            var application = _projectsService.GetApplication(applicationId);

            _projectsService.RejectApplication(applicationId);

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [Authorize(Roles = "Consultant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveApplication(Guid applicationId)
        {
            var application = _projectsService.GetApplication(applicationId);

            _projectsService.RemoveApplication(applicationId);

            return RedirectToAction(nameof(MyApplications), new { status = ApplicationStatus.Applied });
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
