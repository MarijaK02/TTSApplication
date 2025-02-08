using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TTS.Domain.Domain;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Repository;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class ActivitiesService : IActivitesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Activity> _activitiesRepository;
        private readonly IRepository<ConsultantProject> _consultantProjectRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserService _userService;

        public ActivitiesService(ApplicationDbContext context,
            IRepository<Project> projectRepository,
            IRepository<ConsultantProject> consultantProjectRepository,
            IRepository<Activity> activitiesRepository,
            IRepository<Comment> commentRepository,
            IAttachmentService attachmentService,
            IUserService userService)
        {
            _context = context;
            _projectRepository = projectRepository;
            _activitiesRepository = activitiesRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _commentRepository = commentRepository;
            _attachmentService = attachmentService;
            _userService = userService;
        }

        public List<Activity> GetAllProjectActivites(Guid projectId)
        {
            var consultantProjects = _consultantProjectRepository.GetAll()
                .Include(cp => cp.Activites)
                .Include(cp => cp.Consultant)
                .Include("Consultant.User")
                .Where(cp => cp.ProjectId == projectId)
                .ToList()
                .Where(cp => cp.Activites != null)
                .ToList();

            var activities = consultantProjects
                .SelectMany(cp => cp.Activites)
                .ToList();

            return activities;
        }

        public List<Activity> FilterActivitiesByConsultant(List<Activity> rawActivities, Guid? selectedConsultantId)
        {
            return rawActivities.Where(a => a.ConsultantProject?.ConsultantId == selectedConsultantId).ToList();
        }

        public Activity GetDetails(Guid activityId)
        {
            return _activitiesRepository.Get(activityId);
        }

        public int GetTotalActiveHours(Activity activity)
        {
            return (int)((TimeSpan)(DateTime.Now - activity.StartDate)).TotalHours;
        }
        public int GetTotalExpectedHours(Activity activity)
        {
            return activity.EndDate != null ? (int)((TimeSpan)(activity.EndDate - activity.StartDate)).TotalHours : 0;
        }

        public void Create(string userId, Guid projectId, string title, string? description, DateTime? endDate)
        {
            var consultant = _userService.GetConsultant(userId);

            var consultantProject = consultant.Projects?.FirstOrDefault(cp => cp.ProjectId == projectId);

            if (consultantProject?.Project != null)
            {
                if (consultantProject.Project.Status == ProjectStatus.New)
                {
                    consultantProject.Project.Status = ProjectStatus.InProgress;
                }
                

                Activity activity = new Activity
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Description = description,
                    Status = ActivityStatus.New,
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    ConsultantProjectId = consultantProject.Id,
                    Comments = new List<Comment>()
                };

                _activitiesRepository.Insert(activity);

                consultantProject.Activites?.Add(activity);

                _consultantProjectRepository.Update(consultantProject);
            }
        }

        public void Edit(Guid activityId, string title, string description, ActivityStatus status, DateTime? endDate)
        {
            var activity = GetDetails(activityId);

            activity.Title = title;
            activity.Description = description;
            activity.Status = status;
            activity.EndDate = endDate;

            _activitiesRepository.Update(activity);            
        }

        public void Delete(Activity activity)
        {
            _activitiesRepository.Delete(activity);
        }
    }
}
