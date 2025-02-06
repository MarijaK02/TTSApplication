using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Enum;
using TTS.Domain.Identity;

namespace TTS.Service.Interface
{
    public interface IActivitesService
    {
        public List<Activity> GetAllProjectActivites(Guid projectId);
        public List<Activity> FilterActivitiesByConsultant(List<Activity> rawActivities, Guid? selectedConsultantId);
        public Activity GetDetails(Guid activityId);
        public void Create(string userId, Guid projectId, string title, string? description, DateTime? endDate);
        public void Edit(Guid activityId, string title, string description, ActivityStatus status, DateTime? endDate);
        public void Delete(Activity activity);

        public int GetTotalActiveHours(Activity activity);
        public int GetTotalExpectedHours(Activity activity);
    }
}
