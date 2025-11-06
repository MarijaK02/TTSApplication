using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Identity;

namespace TTS.Service.Interface
{
    public interface ICommentsService
    {
        public List<Comment> GetActivityComments(Guid ActivityId);
        public Comment GetDetails(Guid commentId);

        public void Delete(Comment comment);
        public Task CreateAsync(Activity activity, TTSApplicationUser user, string? commentBody, IFormFile[]? files);
    }
}
