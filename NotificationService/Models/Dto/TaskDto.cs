namespace NotificationService.Models.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public DateTime? CreateTime { get; set; }
        public List<int> AsignedUserIds { get; set; }
    }
}
