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
        public Client GetClient(string userId);
        public Consultant GetConsultant(string userId);
    }
}
