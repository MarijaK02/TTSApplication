using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using AdminApplication.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ConsultantsController : Controller
    {
        private readonly MainAppSettings _mainAppSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public ConsultantsController(IOptions<MainAppSettings> mainAppSettings, IHttpClientFactory httpClientFactory)
        {
            _mainAppSettings = mainAppSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index(string? searchTerm, Expertise? selectedExpertise)
        {
            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "GetAllConsultants";

            HttpResponseMessage response = client.GetAsync(url).Result;
            var consultants = response.Content.ReadAsAsync<List<Consultant>>().Result;

            if (!consultants.IsNullOrEmpty() && !searchTerm.IsNullOrEmpty())
            {
                consultants = consultants.Where(c => c.User.FirstName!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) 
                || c.User.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || c.User.LastName!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!consultants.IsNullOrEmpty() && selectedExpertise != null)
            {
                consultants = consultants.Where(c => c.Expertise == selectedExpertise).ToList();
            }

            var dto = new ConsultantsListDto
            {
                Consultants = consultants,
                SelectedExpertise = selectedExpertise,
                SearchTerm = searchTerm,               
            };

            return View(dto);
        }

        public IActionResult Details(Guid consultantId)
        {
            HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
            string url = "GetDetailsForConsultant";

            var model = new
            {
                Id = consultantId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var data = response.Content.ReadAsAsync<Consultant>().Result;

            var consultantDto = new ConsultantDto
            {
                Consultant = data,
                Projects = data.Projects.Select(p => new ConsultantProjectDto
                {
                    Project = p,
                    TotalConsultantActivites = p.Activites.Count(),
                    TotalConsultantActiveActivites = p.Activites.Where(a => a.Status == ActivityStatus.Active).Count(),
                    TotalConsultantFinishedActivites = p.Activites.Where(a => a.Status == ActivityStatus.Completed).Count(),
                    TotalConsultantInvalidActivities = p.Activites.Where(a => a.Status == ActivityStatus.Invalid).Count(),
                    TotalConsultantNewActivities = p.Activites.Where(a => a.Status == ActivityStatus.New).Count(),
                    TotalHoursWorkingOnProject = TotalHoursWorkingOnProject(p.Activites.ToList())
                }).ToList()
            };

            return View(consultantDto);
        }

        private int TotalHoursWorkingOnProject(List<Activity> activities)
        {
            //zapocnata - kompletirana
            //Activity 1 01.02 - 02.02
            //Activity 2 04.02 - 08.02
            //Activity 3 05.02 - 06.02
            //Activity 4 07.02 - 10.02
            //Activity 5 15.02 - 17.02
            //Intervals: [ (01.02 - 02.02), (04.02 - 10.02) ]
            if (activities == null || activities.Count == 0)
            {
                return 0;
            }

            var sortedActivities = activities.Where(a => a.StartDate < DateTime.Now).OrderBy(a => a.StartDate).ToList();

            if (!sortedActivities.Any())
            {
                return 0;
            }

            var intervals = new List<Interval>();

            intervals.Add(new Interval
            {
                From = sortedActivities[0].StartDate,
                To = sortedActivities[0].CompletedOn ?? DateTime.Now
            });

            for (int i = 1; i < sortedActivities.Count; i++)
            {
                var activity = sortedActivities[i];
                var lastInterval = intervals.Last();

                if (activity.StartDate <= lastInterval.To)
                {
                    if (activity.EndDate > lastInterval.To)
                    {
                        lastInterval.To = sortedActivities[i].CompletedOn ?? DateTime.Now;
                    }
                }
                else
                {
                    intervals.Add(new Interval
                    {
                        From = activity.StartDate,
                        To = sortedActivities[i].CompletedOn ?? DateTime.Now
                    });
                }
            }

            var total = intervals.Sum(i => (i.To - i.From).TotalHours);

            return (int)Math.Round(total);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditConsultant(Consultant consultant)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("MainAppClient");
                string url = "EditConsultant";

                var dto = new EditConsultantDto
                {
                    ConsultantId = consultant.Id,
                    FirstName = consultant.User.FirstName ?? "",
                    LastName = consultant.User.LastName ?? "",
                    Email = consultant.User.Email,
                    PhoneNumber = consultant.User.PhoneNumber ?? "",
                    Expertise = consultant.Expertise
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно ажурирање на клиентот.");

                }
            }

            return RedirectToAction("Details", new { consultantId = consultant.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProjectFromConsultant(Guid consultantId, Guid consultantProjectId)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = _httpClientFactory.CreateClient("MainAppClient");
                string url = "DeleteConsultantProject";

                var model = new
                {
                    Id = consultantProjectId
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно бришење на проектот.");

                }
            }

            return RedirectToAction("Details", new { consultantId = consultantId });
        }
    }
}
