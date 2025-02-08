using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Service.Interface
{
    public interface IUserService
    {
        public List<Client> GetAllClients();
        public List<Consultant> GetAllConsultants();
        public Client GetClient(string userId);
        public Consultant GetConsultant(string userId);
        public List<Consultant> GetConsultantsForProject(Guid projectId);
    }
}
