using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;

        public CompanyService(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public void CreateNewCompany(Company company)
        {
            _companyRepository.Insert(company);
        }

        public void DeleteCompany(Guid id)
        {
            Company company = _companyRepository.Get(id);
            _companyRepository.Delete(company);
        }

        public void EditExistingCompany(Company company)
        {
            _companyRepository.Update(company);
        }

        public List<Company> GetAllCompanies()
        {
            return _companyRepository.GetAll().ToList();
        }

        public Company GetDetailsForExistingCompany(Guid? id)
        {
            Company company = _companyRepository.Get(id);
            return company;
        }
    }
}
