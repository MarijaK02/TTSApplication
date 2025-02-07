using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    public class ActivitiesController : Controller
    {
        public IActionResult Index(Guid projectId, string? searchTerm, ActivityStatus? selectedStatus)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetActivitiesForProject";

            var model = new
            {
                Id = projectId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var activities = response.Content.ReadAsAsync<List<Activity>>().Result;
            activities = activities.OrderByDescending(a => a.StartDate).ToList();

            if (!activities.IsNullOrEmpty() && !searchTerm.IsNullOrEmpty())
            {
                activities = activities.Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!activities.IsNullOrEmpty() && selectedStatus != null)
            {
                activities = activities.Where(a => a.Status == selectedStatus).ToList();
            }

            var dto = new ActivityListDto
            {
                ProjectId = projectId,
                Activities = activities,
                SearchTerm = searchTerm,
                SelectedStatus = selectedStatus
            };

            return View(dto);
        }

        public IActionResult Details(Guid projectId, Guid activityId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetActivityDetails";

            var model = new
            {
                Id = activityId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var data = response.Content.ReadAsAsync<Activity>().Result;

            var dto = new ActivityDto
            {
                ProjectId = projectId,
                Activity = data
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditActivity(Guid projectId, [Bind("Id,Title,Description,Status,StartDate,EndDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/EditActivity";

                var dto = new EditActivityDto
                {
                    Id = activity.Id,
                    Title = activity.Title,
                    Description = activity.Description,
                    Status = activity.Status,
                    StartDate = activity.StartDate,
                    EndDate = activity.EndDate
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно ажурирање на клиентот.");

                }
            }

            return RedirectToAction("Details", new { projectId = projectId, activityId = activity.Id });
        }

        public IActionResult Create(Guid projectId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetAllConsultantsWorkingOnProject";

            var model = new
            {
                Id = projectId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            var consultantProjects = response.Content.ReadAsAsync<List<ConsultantProject>>().Result;

            var dto = new CreateActivityFormDto()
            {
                ProjectId = projectId,
                ConsultantProjects = consultantProjects
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Guid projectId, [Bind("Title,Status,Description,StartDate,EndDate,ConsultantProjectId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/CreateActivity";

                var dto = new CreateActivityDto
                {
                    Title = activity.Title,
                    Description = activity.Description,
                    Status = activity.Status,
                    StartDate = activity.StartDate,
                    EndDate = activity.EndDate,
                    ConsultantProjectId = activity.ConsultantProjectId
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно креирање на активноста.");
                    return RedirectToAction(nameof(Create), new { projectId = projectId });
                }

                return RedirectToAction(nameof(Index), new { projectId = projectId} );
            }

            return RedirectToAction(nameof(Create), new { projectId = projectId } );
        }

        public IActionResult Delete(Guid activityId, Guid projectId)
        {

            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/DeleteActivity";

            var model = new
            {
                Id = activityId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction(nameof(Index), new { projectId = projectId });
        }
    }
}
