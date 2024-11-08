using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Repository;

namespace TTS.Web.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<TTSApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectsController(
            ApplicationDbContext context, 
            UserManager<TTSApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: MyProjects
        [Route("MyProjects")]
        public async Task<IActionResult> CustomIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user!)).FirstOrDefault();
            var projects = new List<GetProjectDto>();

            if (role!.Equals("Client"))
            {
                var client = _context.Clients.FirstOrDefault(c => c.User.Id == user!.Id);
                projects = await _context.Projects
                    .Where(p => p.CreatedBy.Id == client!.Id)
                    .Select(p => new GetProjectDto
                {
                    Project = p,
                    Disabled = false
                }).ToListAsync();
            }
            if (role!.Equals("Consultant"))
            {
                var consultant = _context.Consultants.FirstOrDefault(c => c.User.Id == user!.Id);
                projects = await _context.ConsultantWorksOnProjects
                    .Where(cp => cp.Consultant.Id == consultant!.Id)
                    .Select(cp => cp.Project)
                    .Select(p => new GetProjectDto
                    {
                        Project = p,
                        Disabled = false
                    }).ToListAsync();
            }

            return View(projects);
        }

        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

                var consultant = _context.Consultants.FirstOrDefault(c => c.User.Id == user!.Id);
                var consultantProjects = _context.ConsultantWorksOnProjects
                    .Where(cp => cp.Consultant.Id == consultant!.Id)
                    .Select(cp => cp.Project);
                var projects = await _context.Projects.Select(p => new GetProjectDto
                {
                    Project = p,
                    Disabled = consultantProjects.Contains(p) ? true : false
                }).ToListAsync();
            
            return View(projects);
        }
        

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Client")]
        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Client")]
        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAndEditProjectDto createProjectDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.GetUserAsync(User);
                if(result != null)
                {
                    var client = _context.Clients.FirstOrDefault(c => c.User.Id == result.Id);
                    if(client != null)
                    {
                        var project = new Project
                        {
                            Title = createProjectDto.Title,
                            Expertise = createProjectDto.Expertise,
                            Description = createProjectDto.Description,
                            StartDate = DateTime.Now,
                            EndDate = null,
                            Status = ProjectStatus.New,
                            TotalHours = 0,
                            CreatedBy = client
                        };

                        _context.Add(project);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }                
            }
            return View(createProjectDto);
        }

        [Authorize(Roles = "Client")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || !ProjectExists(id))
            {
                return NotFound();
            }

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            var dto = new CreateAndEditProjectDto
            {
                Id = project!.Id,
                Title = project.Title,
                Expertise = project.Expertise,
                Description = project.Description
            };

            return View(dto);
        }

        [Authorize(Roles = "Client")]
        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateAndEditProjectDto dto)
        {
            if (!ProjectExists(id))
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
                try
                {
                    project!.Title = dto.Title;
                    project.Description = dto.Description;
                    project.Expertise = dto.Expertise;

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project!.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CustomIndex));
            }
            return View(dto);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CustomIndex));
        }

        [HttpPost]
        [Authorize(Roles = "Consultant")]
        //GET: Projects/ApplyForProject/id
        public async Task<IActionResult> ApplyForProject(Guid id)
        {           
            if (ProjectExists(id))
            {
                var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
                var user = await _userManager.GetUserAsync(User);
                var consultant = await _context.Consultants.FirstOrDefaultAsync(c => c.User.Id == user!.Id);
                var consultantWorksOnProject = new ConsultantWorksOnProject
                {
                    Id = Guid.NewGuid(),
                    Consultant = consultant,
                    Project = project,
                    StartTime = DateTime.Now,
                    TotalHoursSpentWorking = 0
                };
                _context.Add(consultantWorksOnProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CustomIndex));
            }
            return NotFound();
        }

        private bool ProjectExists(Guid? id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
