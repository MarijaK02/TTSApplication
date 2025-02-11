using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Domain.Enum;
using TTS.Domain.Identity;

namespace TTS.Service.Interface
{
    public interface IActivitesService
    {
        public IndexActivitesDto GetAllProjectActivites(Guid projectId, string projectTitle, Guid? selectedConsultantId);
        public ActivityDto GetDetails(Guid? activityId, Guid projectId, string projectTitle);
        public Activity Get(Guid activityId);
        public void Create(string userId, Guid projectId, string title, string? description, DateTime? endDate);
        public void Edit(Guid activityId, string title, string description, ActivityStatus status, DateTime? endDate);
        public void Delete(Activity activity);
    }
}
