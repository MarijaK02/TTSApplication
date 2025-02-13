using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AdminApplication.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index(string? searchTerm, Expertise? selectedExpertise, ProjectStatus? selectedStatus)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetAllProjects";

            HttpResponseMessage response = client.GetAsync(url).Result;
            var projects = response.Content.ReadAsAsync<List<Project>>().Result;

            projects = projects.OrderByDescending(p => p.StartDate).ToList();

            if (!projects.IsNullOrEmpty() && !searchTerm.IsNullOrEmpty())
            {
                projects = projects.Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!projects.IsNullOrEmpty() && selectedExpertise != null)
            {
                projects = projects.Where(c => c.Expertise == selectedExpertise).ToList();
            }

            if (!projects.IsNullOrEmpty() && selectedStatus != null)
            {
                projects = projects.Where(c => c.Status == selectedStatus).ToList();
            }

            var dto = new ProjectsIndexDto
            {
                Projects = projects,
                SelectedExpertise = selectedExpertise,
                SelectedStatus = selectedStatus,
                SearchTerm = searchTerm
            };

            return View(dto);
        }

        public IActionResult Details(Guid projectId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetDetailsForProject";

            var model = new
            {
                Id = projectId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var data = response.Content.ReadAsAsync<DetailsProjectDto>().Result;

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/EditProject";

                var dto = new EditProjectDto
                {
                    ProjectId = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    Expertise = project.Expertise,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно ажурирање на проектот.");

                }
            }

            return RedirectToAction("Details", new { projectId = project.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConsultantFromProject(Guid projectId, Guid consultantProjectId)
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

            return RedirectToAction("Details", new { projectId = projectId });
        }

        public IActionResult Create()
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetAllClients";

            HttpResponseMessage response = client.GetAsync(url).Result;
            var clients = response.Content.ReadAsAsync<List<Client>>().Result;

            var dto = new CreateProjectFormDto()
            {
                Clients = clients
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Expertise,Status,Description,StartDate,EndDate,CreatedById")]Project project)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/CreateProject";

                var dto = new CreateProjectDto
                {
                    Title = project.Title,
                    Description = project.Description,
                    Expertise = project.Expertise,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    CreatedById = project.CreatedById
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно креирање на проектот.");

                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddConsultantToProject(Guid consultantId, Guid projectId)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                string url = "https://localhost:44315/api/Admin/AddConsultantToProject";

                var dto = new AddConsultantToProjectDto
                {
                    ConsultantId = consultantId,
                    ProjectId = projectId
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                return RedirectToAction(nameof(Details), new { projectId = projectId });
            }

            return RedirectToAction(nameof(Details), new { projectId = projectId });
        }

        public IActionResult AcceptApplication(Guid applicationId, Guid projectId)
        {

            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/AcceptApplication";

            var model = new
            {
                Id = applicationId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction(nameof(Details), new { projectId = projectId });
        }

        public IActionResult RejectApplication(Guid applicationId, Guid projectId)
        {

            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/RejectApplication";

            var model = new
            {
                Id = applicationId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction(nameof(Details), new { projectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid projectId)
        {

            if(ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                string url = "https://localhost:44315/api/Admin/DeleteProject";

                var model = new
                {
                    Id = projectId
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
