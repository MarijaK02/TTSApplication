namespace AdminApplication.Models.DTO
{
    public class ActivityDto
    {
        public Guid ProjectId { get; set; }
        public Activity Activity { get; set; }
        public int TotalHours { get; set; }
    }
}
