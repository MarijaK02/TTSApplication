using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    public class ActivitiesController : Controller
    {
        public IActionResult Index(Guid projectId, string? searchTerm, ActivityStatus? selectedStatus, Guid? selectedConsultantId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetActivitiesForProject";

            var model = new
            {
                Id = projectId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var data = response.Content.ReadAsAsync<ActivitiesDto>().Result;
            var activities = data.Activities.OrderByDescending(a => a.StartDate).ToList();

            if (!activities.IsNullOrEmpty() && !searchTerm.IsNullOrEmpty())
            {
                activities = activities.Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!activities.IsNullOrEmpty() && selectedStatus != null)
            {
                activities = activities.Where(a => a.Status == selectedStatus).ToList();
            }

            if(selectedConsultantId != null)
            {
                activities = activities.Where(a => a.ConsultantProjectId == selectedConsultantId).ToList();
            }

            var dto = new ActivityListDto
            {
                ProjectId = projectId,
                Activities = activities,
                ProjectConsultants = data.Consultants,
                SearchTerm = searchTerm,
                SelectedStatus = selectedStatus,
                SelectedConsultantId = selectedConsultantId
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public FileContentResult ExportAllActivities()
        {
            string fileName = "Активности.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using(var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Активности");

                worksheet.Cell(1, 1).Value = "Бр.";
                worksheet.Cell(1, 2).Value = "ИД";
                worksheet.Cell(1, 3).Value = "Активност";
                worksheet.Cell(1, 4).Value = "Проект";
                worksheet.Cell(1, 5).Value = "Почетен Датум";
                worksheet.Cell(1, 6).Value = "Краен рок";
                worksheet.Cell(1, 7).Value = "Статус";
                worksheet.Cell(1, 8).Value = "Дата на комплетирање";
                worksheet.Cell(1, 9).Value = "Задолжен консултант";

                HttpClient client = new HttpClient();
                string url = "https://localhost:44315/api/Admin/GetAllActivities";

                HttpResponseMessage response = client.GetAsync(url).Result;
                var activities = response.Content.ReadAsAsync<List<Activity>>().Result;

                var numActivities = activities.Count;

                for (int i=0; i<numActivities; i++)
                {
                    worksheet.Cell(i+2, 1).Value = i+1;
                    worksheet.Cell(i+2, 2).Value = activities[i].Id.ToString();
                    worksheet.Cell(i+2, 3).Value = activities[i].Title;
                    worksheet.Cell(i+2, 4).Value = activities[i].ConsultantProject?.Project?.Title;
                    worksheet.Cell(i+2, 5).Value = activities[i].StartDate.ToString();
                    worksheet.Cell(i+2, 6).Value = activities[i].EndDate.ToString();
                    worksheet.Cell(i+2, 7).Value = activities[i].Status.ToString();
                    worksheet.Cell(i+2, 8).Value = activities[i].CompletedOn != null  ? activities[i].CompletedOn.ToString() : "N/A";

                    var c = activities[i].ConsultantProject?.Consultant;
                    worksheet.Cell(i+2, 9).Value = c.User.FirstName + " " + c.User.LastName;                   
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }

            }
        }
    }
}
