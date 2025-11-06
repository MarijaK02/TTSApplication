using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ActivitiesController : Controller
    {

        private readonly MainAppSettings _mainAppSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public ActivitiesController(IOptions<MainAppSettings> mainAppSettings, IHttpClientFactory httpClientFactory)
        {
            _mainAppSettings = mainAppSettings.Value;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index(Guid projectId, string? searchTerm, ActivityStatus? selectedStatus, Guid? selectedConsultantId)
        {
            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "GetActivitiesForProject";

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
            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "GetActivityDetails";

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
                Activity = data,
                TotalHours = data.EndDate != null ? (int)(data.EndDate - data.StartDate).TotalHours : (int)(DateTime.Now - data.StartDate).TotalHours
            };


            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditActivity(Guid projectId, [Bind("Id,Title,Description,Status,StartDate,EndDate,CompletedOn")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("MainAppClient");
                string url = "EditActivity";

                var dto = new EditActivityDto
                {
                    Id = activity.Id,
                    Title = activity.Title,
                    Description = activity.Description,
                    Status = activity.Status,
                    StartDate = activity.StartDate,
                    EndDate = activity.EndDate,
                    CompletedOn = activity.CompletedOn
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
            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "GetAllConsultantsWorkingOnProject";

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
                HttpClient httpClient = _httpClientFactory.CreateClient("MainAppClient");
                string url = "CreateActivity";

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

            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "DeleteActivity";

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
                HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
                string url = "GetAllActivities";

                HttpResponseMessage response = client.GetAsync(url).Result;
                var activities = response.Content.ReadAsAsync<List<Activity>>().Result;

                var activituesByProject = activities
                    .GroupBy(a => a.ConsultantProject.Project.Id)
                    .OrderBy(g => g.Key);
                    

                foreach (var group in activituesByProject)
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add(group.First().ConsultantProject.Project.Title);

                    worksheet.Cell(1, 1).Value = "Бр.";
                    worksheet.Cell(1, 2).Value = "ИД";
                    worksheet.Cell(1, 3).Value = "Активност";
                    worksheet.Cell(1, 4).Value = "Проект";
                    worksheet.Cell(1, 5).Value = "Почетен Датум";
                    worksheet.Cell(1, 6).Value = "Краен рок";
                    worksheet.Cell(1, 7).Value = "Статус";
                    worksheet.Cell(1, 8).Value = "Дата на комплетирање";
                    worksheet.Cell(1, 9).Value = "Задолжен консултант";



                    int row = 2;
                    foreach (var activity in group)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = activity.Id.ToString();
                        worksheet.Cell(row, 3).Value = activity.Title;
                        worksheet.Cell(row, 4).Value = activity.ConsultantProject?.Project?.Title;
                        worksheet.Cell(row, 5).Value = activity.StartDate.ToString();
                        worksheet.Cell(row, 6).Value = activity.EndDate.ToString();
                        worksheet.Cell(row, 7).Value = activity.Status.ToString();
                        worksheet.Cell(row, 8).Value = activity.CompletedOn?.ToString() ?? "N/A";

                        var consultant = activity.ConsultantProject?.Consultant?.User;
                        worksheet.Cell(row, 9).Value = consultant != null ? consultant.FirstName + " " + consultant.LastName : "N/A";

                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    worksheet.Row(1).Style.Font.Bold = true;                    
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
