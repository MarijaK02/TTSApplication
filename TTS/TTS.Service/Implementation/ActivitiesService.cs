using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
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
        private readonly ICommentsService _commentsService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserService _userService;

        public ActivitiesService(ApplicationDbContext context,
            IRepository<Project> projectRepository,
            IRepository<ConsultantProject> consultantProjectRepository,
            IRepository<Activity> activitiesRepository,
            ICommentsService commentsService,
            IAttachmentService attachmentService,
            IUserService userService)
        {
            _context = context;
            _projectRepository = projectRepository;
            _activitiesRepository = activitiesRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _commentsService = commentsService;
            _attachmentService = attachmentService;
            _userService = userService;
        }

        public IndexActivitesDto GetAllProjectActivites(Guid projectId, string projectTitle, Guid? selectedConsultantId, ActivityStatus? selectedStatus, string? searchTerm)
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

            activities = FilterActivitiesByConsultant(activities, selectedConsultantId, selectedStatus, searchTerm);

            var dto = new IndexActivitesDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle,
                Activites = activities,
                Consultants = _userService.GetConsultantsForProject(projectId),
                SelectedConsultantId = selectedConsultantId
            };

            return dto;
        }

        private List<Activity> FilterActivitiesByConsultant(List<Activity> rawActivities, Guid? selectedConsultantId, ActivityStatus? selectedStatus, string? searchTerm)
        {
            var result = rawActivities;

            if(selectedConsultantId != null)
            {
                result = result.Where(a => a.ConsultantProject?.ConsultantId == selectedConsultantId).ToList();
            }

            if(selectedStatus != null)
            {
                result = result.Where(a => a.Status == selectedStatus).ToList();
            }

            if(searchTerm != null)
            {
                return result.Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return result;
        }

        public Activity Get(Guid activityId)
        {
            return _activitiesRepository.Get(activityId);
        }

        public ActivityDto GetDetails(Guid? activityId, Guid projectId, string projectTitle)
        {
            var activity = _activitiesRepository.Get(activityId);

            var dto = new ActivityDto
            {
                ProjectId = projectId,
                ProjectTitle = projectTitle,
                Activity = activity,
                Comments = _commentsService.GetActivityComments(activity.Id),
                TotalActiveHours = GetTotalActiveHours(activity),
                TotalExpectedHours = GetTotalExpectedHours(activity)
            };

            return dto;
        }

        private int GetTotalActiveHours(Activity activity)
        {
            if (activity.Status == ActivityStatus.Invalid || activity.Status == ActivityStatus.Completed)
            {
                return (int)(activity.EndDate - activity.StartDate).TotalHours;
            }
            return (int)(DateTime.Now - activity.StartDate).TotalHours;
        }
        private int GetTotalExpectedHours(Activity activity)
        {
            return (int)(activity.EndDate - activity.StartDate).TotalHours;
        }

        public void Create(string userId, Guid projectId, string title, string? description, DateTime startDate, DateTime endDate)
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
                    CreatedOn = DateTime.Now,
                    StartDate = startDate,
                    EndDate = endDate,
                    ConsultantProjectId = consultantProject.Id,
                    Comments = new List<Comment>()
                };

                _activitiesRepository.Insert(activity);

                consultantProject.Activites?.Add(activity);

                _consultantProjectRepository.Update(consultantProject);
            }
        }

        public void Edit(Guid activityId, string title, string? description, ActivityStatus status, DateTime startDate, DateTime endDate)
        {
            var activity = Get(activityId);

            activity.Title = title;
            activity.Description = description;
            activity.Status = status;
            activity.StartDate = startDate;
            activity.EndDate = endDate;

            if (activity.Status == ActivityStatus.Completed)
            {
                activity.CompletedOn = DateTime.Now;
            }
            else
            {
                activity.CompletedOn = null;
            }         

            _activitiesRepository.Update(activity);            
        }

        public void Delete(Activity activity)
        {
            _activitiesRepository.Delete(activity);
        }
    }
}
