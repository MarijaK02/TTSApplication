using Microsoft.AspNetCore.Http;
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
    public class AttachmentService : IAttachmentService
    {
        public readonly IRepository<Attachment> _attachmentRepository;

        public AttachmentService(IRepository<Attachment> attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public Attachment GetDetails(Guid id)
        {
            return _attachmentRepository.Get(id);
        }

        public void Delete(Attachment attachment) 
        {
            _attachmentRepository.Delete(attachment);
        }      
    }


}
