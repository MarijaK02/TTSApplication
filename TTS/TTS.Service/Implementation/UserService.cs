using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.DTO;
using TTS.Repository;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Consultant> _consultantRepository;
        private IRepository<ConsultantProject> _consultantProjectRepository;

        public UserService(ApplicationDbContext context,
            IRepository<Client> clientReposiotry,
            IRepository<ConsultantProject> consultantProjectRepository,
            IRepository<Consultant> consultantRepository)
        {
            _context = context;
            _clientRepository = clientReposiotry;
            _consultantRepository = consultantRepository;
            _consultantProjectRepository = consultantProjectRepository;
        }

        public List<Client> GetAllClients()
        {
            return _clientRepository.GetAll()
                .Include(c => c.Projects)
                .ToList();
        }

        public List<Consultant> GetAllConsultants()
        {
            return _consultantRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .Include("Projects.Project")
                .ToList();
        }

        public Client GetClient(string userId)
        {
            return GetAllClients()
                .First(c => c.UserId == userId);
        }

        public Guid GetClientId(string userId)
        {
            return _clientRepository.GetAll().First(c => c.UserId == userId).Id;
        }

        public Consultant GetConsultant(string userId)
        {
            return GetAllConsultants()
                .First(c => c.UserId == userId);
        }

        public List<Consultant> GetConsultantsForProject(Guid projectId)
        {
            return _consultantProjectRepository.GetAll()
                .Include(cp => cp.Consultant)
                .Include("Consultant.User")
                .Where(c => c.ProjectId == projectId && c.ApplicationStatus == Domain.Enum.ApplicationStatus.Accepted)
                .Select(c => c.Consultant)
                .ToList();
        }

    }
}
