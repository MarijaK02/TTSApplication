using AdminApplication.Models;
using AdminApplication.Models.DTO;
using AdminApplication.Models.DTO.API;
using AdminApplication.Models.Enums;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    public class ClientsController : Controller
    {
        public IActionResult Index(string? searchTerm, Industry? selectedIndustry)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetAllClients";

            HttpResponseMessage response = client.GetAsync(url).Result;
            var clients = response.Content.ReadAsAsync<List<Client>>().Result;

            if (!clients.IsNullOrEmpty() && !searchTerm.IsNullOrEmpty())
            {
                clients = clients.Where(c => c.User.FirstName!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || c.User.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if(!clients.IsNullOrEmpty() && selectedIndustry != null)
            {
                clients = clients.Where(c => c.Industry == selectedIndustry).ToList();
            }

            var dto = new ClientsListDto
            {
                Clients = clients,
                SelectedIndustry = selectedIndustry,
                SearchTerm = searchTerm
            };

            return View(dto);
        }

        public IActionResult Details(Guid clientId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44315/api/Admin/GetDetailsForClient";

            var model = new
            {
                Id = clientId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            var data = response.Content.ReadAsAsync<Client>().Result;

            return View(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditClient(Client client)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                string url = "https://localhost:44315/api/Admin/EditClient";

                var dto = new EditClientDto
                {
                    ClientId = client.Id,
                    FirstName = client.User.FirstName ?? "",
                    LastName = client.User.LastName ?? "",
                    Email = client.User.Email,
                    PhoneNumber = client.User.PhoneNumber ?? "",
                    Address = client.Address ?? "",
                    Industry = client.Industry
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;

                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    ModelState.AddModelError("", "Неуспешно ажурирање на клиентот.");

                }
            }

            return RedirectToAction("Details", new { clientId = client.Id });
        }
    }
}
