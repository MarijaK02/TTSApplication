using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Service.Interface
{
    public interface IAttachmentService
    {
        public Attachment GetDetails(Guid id);
        public void Delete(Attachment attachment);
    }
}
