using Microsoft.EntityFrameworkCore;
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
    public class AdminService : IAdminService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ConsultantProject> _consultantProjectRepository;
        private readonly IRepository<Activity> _activityRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IRepository<Consultant> _consultantRepository;
        private readonly IRepository<Client> _clientRepository;

        public AdminService(IRepository<Project> projectRepository, 
            IRepository<ConsultantProject> consultantProjectRepository, 
            IRepository<Activity> activityRepository,
            IRepository<Comment> commentRepository,
            IRepository<Attachment> attachmentRepository,
            IRepository<Consultant> consultantRepository,
            IRepository<Client> clientRepository)
        {
            _projectRepository = projectRepository;
            _consultantProjectRepository = consultantProjectRepository;
            _activityRepository = activityRepository;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
            _consultantRepository = consultantRepository;
            _clientRepository = clientRepository;
        }

        public List<Client> GetAllClients()
        {
            return _clientRepository.GetAll()
                .Include(c => c.User)
                .Include(c => c.Projects)
                .ToList();
        }

        public List<Consultant> GetAllConsultants()
        {
            return _consultantRepository.GetAll()
                .Include(c => c.Projects)
                .Include("Projects.Project")
                .ToList();
        }
    }
}
