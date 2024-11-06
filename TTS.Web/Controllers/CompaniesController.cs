using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Repository;
using TTS.Service.Interface;

namespace TTS.Web.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET: Companies
        public  IActionResult Index()
        {
            return View(_companyService.GetAllCompanies());
        }

        // GET: Companies/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _companyService.GetDetailsForExistingCompany(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCompanyDto createCompanyDto)
        {
            if (ModelState.IsValid)
            {
                Company company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = createCompanyDto.Name,
                    Industry = createCompanyDto.Industry,
                    Address = createCompanyDto.Address,
                    Email = createCompanyDto.Email,
                    ContactPhone = createCompanyDto.ContactPhone
                };

                _companyService.CreateNewCompany(company);
                return RedirectToAction(nameof(Index));
            }
            return View(createCompanyDto);
        }

        // GET: Companies/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _companyService.GetDetailsForExistingCompany(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Address,Name,Industry,ContactPhone,Email,Id")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _companyService.EditExistingCompany(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _companyService.GetDetailsForExistingCompany(id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var company = _companyService.GetDetailsForExistingCompany(id);
            if (company != null)
            {
                _companyService.DeleteCompany(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(Guid id)
        {
            return _companyService.GetDetailsForExistingCompany(id) != null;
        }
    }
}
