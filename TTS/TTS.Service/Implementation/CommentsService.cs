using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Enum;
using TTS.Domain.Identity;
using TTS.Repository.Interface;
using TTS.Service.Interface;

namespace TTS.Service.Implementation
{
    public class CommentsService : ICommentsService
    {
        public readonly IRepository<Comment> _commentRepository;
        private readonly IConfiguration _config;

        public CommentsService(IRepository<Comment> commentRepository, IConfiguration config) 
        {
            _commentRepository = commentRepository;
            _config = config;
        }

        public List<Comment> GetActivityComments(Guid ActivityId)
        {
            return _commentRepository.GetAll()
                .Include(c => c.CreatedBy)
                .Include(c => c.Attachments)
                .Where(c => c.ActivityId == ActivityId)
                .ToList()
                .OrderByDescending(c => c.CreatedOn)
                .ToList();
        }

        public Comment GetDetails(Guid commentId)
        {
            return _commentRepository.Get(commentId);
        }

        public async Task CreateAsync(Activity activity, TTSApplicationUser user, string? commentBody, IFormFile[]? files)
        {
            if (activity.Status == ActivityStatus.New)
            {
                activity.Status = ActivityStatus.Active;
            }

            Comment comment = new Comment
            {
                Id = Guid.NewGuid(),
                CommentBody = commentBody,
                ActivityId = activity.Id,
                CreatedById = user.Id,
                CreatedOn = DateTime.UtcNow,
                Attachments = new List<Attachment>()
            };

            if (string.IsNullOrEmpty(_config["AzureBlobStorage:ConnectionString"]))
            {
                if (files != null && files.Any())
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in files)
                    {
                        var fileName = file.FileName + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var attachment = new Attachment
                        {
                            Id = Guid.NewGuid(),
                            FileName = file.FileName,
                            FilePath = filePath,
                            CommentId = comment.Id
                        };

                        comment.Attachments.Add(attachment);
                    }

                }
            }
            else
            {
                var blobServiceClient = new BlobServiceClient(_config["AzureBlobStorage:ConnectionString"]);
                var containerClient = blobServiceClient.GetBlobContainerClient(_config["AzureBlobStorage:ContainerName"]);

                foreach (var file in files)
                {
                    var blobName = $"{Guid.NewGuid()}_{file.FileName}";
                    var blobClient = containerClient.GetBlobClient(blobName);

                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream);
                    }

                    var attachment = new Attachment
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        FilePath = blobName,
                        CommentId = comment.Id
                    };

                    comment.Attachments.Add(attachment);
                }
            }


                _commentRepository.Insert(comment);
        }

        public void Delete(Comment comment)
        {
            _commentRepository.Delete(comment);
        }
    }
}
