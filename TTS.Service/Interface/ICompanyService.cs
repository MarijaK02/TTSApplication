using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Service.Interface
{
    public interface ICompanyService
    {
        List<Company> GetAllCompanies();
        void CreateNewCompany(Company company);
        void DeleteCompany(Guid id);
        void EditExistingCompany(Company company);
        Company GetDetailsForExistingCompany(Guid? id);
    }
}
