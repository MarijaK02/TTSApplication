namespace AdminApplication.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string CreatedById { get; set; }
        public TTSApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CommentBody { get; set; }

        public virtual List<Attachment>? Attachments { get; set; }
    }
}
