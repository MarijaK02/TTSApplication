using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Repository;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Client GetClient(string userId)
        {
            return _context.Clients.First(c => c.User.Id == userId);
        }

        public Consultant GetConsultant(string userId)
        {
            return _context.Consultants.First(c => c.User.Id == userId);
        }
    }
}
