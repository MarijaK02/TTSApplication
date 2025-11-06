using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Domain.Shared;
using TTS.Repository;
using TTS.Service.Interface;

namespace TTS.Web.Controllers
{
    [Route("ConsultantActivites")]
    [Authorize(Roles = "Client, Consultant")]
    public class ActivitiesController : Controller
    {
        private readonly UserManager<TTSApplicationUser> _userManager;
        private readonly IActivitesService _activitesService;
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;
        private readonly ICommentsService _commentsService;
        private readonly IProjectsService _projectsService;
        private readonly IConfiguration _config;

        public ActivitiesController(
            UserManager<TTSApplicationUser> userManager, 
            IActivitesService activitesService,
            IUserService userService,
            IAttachmentService attachmentService,
            ICommentsService commentsService,
            IProjectsService projectsService,
            IConfiguration config)
        {
            _userManager = userManager;
            _activitesService = activitesService;
            _userService = userService;
            _attachmentService = attachmentService;
            _commentsService = commentsService;
            _projectsService = projectsService;
            _config = config;
        }

        // GET: ConsultantActivites
        [HttpGet]
        public IActionResult Index(Guid projectId, string projectTitle, Guid? selectedConsultantId, ActivityStatus? selectedStatus, string? searchTerm)
        {
            var activites = _activitesService.GetAllProjectActivites(projectId, projectTitle, selectedConsultantId, selectedStatus, searchTerm);            

            return View(activites);
        }

        // GET: ConsultantActivites/Details/5
        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid projectId, Guid? id, string projectTitle, Interval projectDeadline, DateTime from, DateTime to)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = _activitesService.GetDetails(id, projectId, projectTitle, projectDeadline ?? new Interval { From = from, To = to });
            

            return View(activity);
        }

        // POST: ConsultantActivites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Guid projectId, string projectTitle, string newActivityTitle, string newActivityDescription, DateTime startDate, DateTime endDate)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return NotFound("Коирсинкот не постои");
                }

                _activitesService.Create(userId, projectId, newActivityTitle, newActivityDescription, startDate, endDate);
                               
            }
            return RedirectToAction(nameof(Index), new { projectId = projectId, projectTitle = projectTitle });
        }

        // GET: ConsultantActivites/Edit/5
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(Guid projectId, Guid id, string projectTitle, DateTime from, DateTime toEnd)
        {
            var activity = _activitesService.Get(id);
            if (activity == null)
            {
                return NotFound();
            }

            var dto = new CreateAndEditActivityDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle,
                ProjectDeadline = new Interval() { From = from, To = toEnd },
                ActivityId = activity.Id,
                Title = activity.Title,
                Description = activity.Description ?? "",
                ActivityStatus = activity.Status,
                StartDate = activity.StartDate,
                EndDate = activity.EndDate
            };   
            
            return View(dto);
        }

        // POST: ConsultantActivites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{activityId}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid activityId, CreateAndEditActivityDto dto)
        {
            if (ModelState.IsValid)
            {
                var activity = _activitesService.Get(activityId);
                if (activity == null)
                {
                    return NotFound();
                }

                _activitesService.Edit(activityId, dto.Title, dto.Description, dto.ActivityStatus, dto.StartDate, dto.EndDate);

                return RedirectToAction(nameof(Details), new { projectId = dto.ProjectId, id = activityId, projectTitle = dto.ProjectTitle, projectDeadline = dto.ProjectDeadline });
            }
            return View(dto);
        }

        
        // POST: ConsultantActivites/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid projectId, Guid id, string projectTitle)
        {
            var activity = _activitesService.Get(id);
            if (activity != null)
            {
                _activitesService.Delete(activity);
            }

            return RedirectToAction(nameof(Index), new { projectId = projectId, projectTitle = projectTitle});
        }

        [HttpPost("AddComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid projectId, string projectTitle, Interval projectDeadline, Guid id, string? commentBody, IFormFile[]? files)
        {
            if(String.IsNullOrEmpty(commentBody) && (files==null || !files.Any()))
            {
                return View("Error");
            }

            var activity = _activitesService.Get(id);
            var user = await _userManager.GetUserAsync(User);
            if (activity == null || user == null)
            {
                return NotFound();
            }

            _commentsService.CreateAsync(activity, user, commentBody, files);

            return RedirectToAction(nameof(Details), new { projectId = projectId, projectTitle = projectTitle, id = activity.Id, projectDeadline = projectDeadline });
        }

        [HttpGet("Download/{fileId}")]
        public async Task<IActionResult> DownloadAsync(Guid fileId)
        {
            var attacment = _attachmentService.GetDetails(fileId);
            if (attacment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(_config["AzureBlobStorage:ConnectionString"]))
            {
                if (System.IO.File.Exists(attacment.FilePath))
                {
                    return File(System.IO.File.OpenRead(attacment.FilePath), "application/octet-stream", attacment.FileName);
                }
            }
            else
            {
                var blobServiceClient = new BlobServiceClient(_config["AzureBlobStorage:ConnectionString"]);
                var containerClient = blobServiceClient.GetBlobContainerClient(_config["AzureBlobStorage:ContainerName"]);
                var blobClient = containerClient.GetBlobClient(attacment.FilePath);

                if (await blobClient.ExistsAsync())
                {
                    var stream = await blobClient.OpenReadAsync();
                    return File(stream, "application/octet-stream", attacment.FileName);
                }
            }

            return NotFound();
        }

        [HttpPost("RemoveFile/{fileId}")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFile(Guid fileId, Guid projectId, Guid id, string projectTitle, Interval projectDeadline)
        {
            var attachment = _attachmentService.GetDetails(fileId);
            if (attachment != null && System.IO.File.Exists(attachment.FilePath))
            {
                System.IO.File.Delete(attachment.FilePath);
                _attachmentService.Delete(attachment);
            }

            return RedirectToAction(nameof(Details), new { projectId = projectId, id = id, projectTitle = projectTitle, projectDeadline = projectDeadline });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(Guid projectId, Guid activityId, Guid commentId, string projectTitle, Interval projectDeadline)
        {  
            var comment = _commentsService.GetDetails(commentId);
            if (comment != null)
            {
                _commentsService.Delete(comment);
            }
            
            return RedirectToAction(nameof(Details), new { projectId = projectId, id = activityId, projectTitle = projectTitle, projectDeadline = projectDeadline });
        }
    }
}
