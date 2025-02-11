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
    public class ConsultantsController : Controller
    {
        public IActionResult Index(string? searchTerm, Expertise? selectedExpertise)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetAllConsultants";

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
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetDetailsForConsultant";

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
                    TotalHoursWorkingOnProject = (int) p.Activites
                        .Sum(a =>
                            (a.Status == ActivityStatus.Invalid || a.Status == ActivityStatus.Completed
                                ? (a.EndDate - a.StartDate)?.TotalHours
                                : (DateTime.Now - a.StartDate).TotalHours) ?? 0)
                }).ToList()
            };

            return View(consultantDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditConsultant(Consultant consultant)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/EditConsultant";

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
                HttpClient client = new HttpClient();
                string url = "https://localhost:44315/api/Admin/DeleteConsultantProject";

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
