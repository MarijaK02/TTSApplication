using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Domain.Shared;
using TTS.Repository;

namespace TTS.Web.Controllers
{
    [Route("Activities")]
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<TTSApplicationUser> _userManager;

        public ActivitiesController(ApplicationDbContext context, UserManager<TTSApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Activities
        [HttpGet]
        public async Task<IActionResult> Index(Guid projectId, string projectTitle)
        {
            var dto = new IndexActivitesDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle ?? "",
                Activities = await _context.Activities
                .Include(a => a.ConsultantProject)
                .Include("ConsultantProject.Consultant")
                .Include("ConsultantProject.Project")
                .Include("ConsultantProject.Consultant.User")
                .Include("Comments.CreatedBy")
                .Where(a => a.ConsultantProject.Project.Id == projectId)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync() ?? new List<Activity>()
                
            };

            return View(dto);
        }

        // GET: Activities/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid projectId, Guid? id, string projectTitle)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.Comments)
                .Include("ConsultantProject")
                .Include("ConsultantProject.Consultant")
                .Include("ConsultantProject.Consultant.User")
                .Include("Comments.CreatedBy")
                .Include("Comments.Attachments")
                .FirstOrDefaultAsync(m => m.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            List<Comment>? comments = activity.Comments == null ? null : activity.Comments.OrderByDescending(c => c.CreatedOn).ToList();
            var dto = new ActivityDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle,
                Activity = activity,
                Comments = comments
            };
            return View(dto);
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId, ProjectTitle, NewActivityTitle, NewActivityDescription")] IndexActivitesDto dto)
        {
            if(dto == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var consultant = await _context.Consultants
                    .Include(c => c.Projects)
                    .Include("Projects.Project")
                    .FirstOrDefaultAsync(c => c.User.Id == user!.Id);

                var project = consultant?.Projects?.FirstOrDefault(cp => cp.Project.Id == dto.ProjectId);

                if (project?.Project != null)
                {
                    if(project.Project.Status == ProjectStatus.New)
                    {
                        project.Project.Status = ProjectStatus.InProgress;
                    }
                    _context.Update(project.Project);
                    Activity activity = new Activity
                    {
                        Id = Guid.NewGuid(),
                        Title = dto.NewActivityTitle ?? "",
                        Description = dto.NewActivityDescription,
                        Status = ActivityStatus.New,
                        StartDate = DateTime.Now,
                        ConsultantProject = project,
                        Comments = null
                    };
                   
                    _context.Add(activity);

                    await _context.SaveChangesAsync();
                    
                }               
            }
            return RedirectToAction(nameof(Index), new { projectId = dto.ProjectId, projectTitle = dto.ProjectTitle });
        }

        // GET: Activities/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid projectId, Guid? id, string projectTitle)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            var dto = new CreateAndEditActivityDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle,
                ActivityId = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                ActivityStatus = activity.Status
            };          
            return View(dto);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateAndEditActivityDto dto)
        {
            if(dto.ActivityId == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var activity = await _context.Activities.FindAsync(dto.ActivityId);
                try
                {                   
                    activity!.Title = dto.Title ?? "";
                    activity.Description = dto.Description;
                    activity.Status = dto.ActivityStatus ?? activity.Status;
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity!.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { projectId = dto.ProjectId, projectTitle = dto.ProjectTitle });
            }
            return View(dto);
        }

        
        // POST: Activities/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid projectId, Guid id, string projectTitle)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { projectId = projectId, projectTitle = projectTitle});
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment(Guid projectId, Guid id, string? commentBody, IFormFile[]? files)
        {
            if(String.IsNullOrEmpty(commentBody) && (files==null || !files.Any()))
            {
                return View("Error");
            }

            var activity = await _context.Activities
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if (activity == null || user == null)
            {
                return NotFound();
            }

            if(activity.Status == ActivityStatus.New)
            {
                activity.Status = ActivityStatus.Active;
            }

            Comment comment = new Comment
            {
                Id = Guid.NewGuid(),
                CommentBody = commentBody,
                CreatedBy = user,
                CreatedOn = DateTime.UtcNow,
                Activity = activity
            };

            if (activity.Comments == null)
            {
                activity.Comments = new List<Comment>();
            }

            _context.Comments.Add(comment);

            if (files != null && files.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in files)
                {
                    //if (file.Length > 5 * 1024 * 1024)
                    //{
                    //    ModelState.AddModelError("Files", $"File {file.FileName} is too large. Max size is 5 MB.");
                    //    return BadRequest("One or more files are too large.");
                    //}

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    if (activity.Status == ActivityStatus.New)
                    {
                        activity.Status = ActivityStatus.Active;
                    }

                    var attachment = new Attachment
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        FilePath = filePath,
                        Comment = comment
                    };

                    _context.Attachments.Add(attachment);
                }
            }
          
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { projectId = projectId, id = activity.Id });
        }

        [HttpGet("Download/{fileId}")]
        public IActionResult Download(Guid fileId)
        {
            var attacment = _context.Attachments.FirstOrDefault(a => a.Id == fileId);
            if (attacment == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(attacment.FilePath))
            {
                return File(System.IO.File.OpenRead(attacment.FilePath), "application/octet-stream", attacment.FileName);
            }
            return NotFound();
        }

        [HttpGet("RemoveFile/{fileId}")]
        public IActionResult RemoveFile(Guid fileId, Guid projectId, Guid id)
        {
            var attachment = _context.Attachments.FirstOrDefault(a => a.Id == fileId);
            if (attachment != null && System.IO.File.Exists(attachment.FilePath))
            {
                System.IO.File.Delete(attachment.FilePath);
                _context.Attachments.Remove(attachment);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Details), new { projectId = projectId, id = id });
        }

        private bool ActivityExists(Guid id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}
