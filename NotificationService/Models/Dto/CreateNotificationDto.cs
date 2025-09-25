namespace NotificationService.Models.Dto
{
    public class CreateNotificationDto
    {
        public int IdUser { get; set; }
        public int IdTask { get; set; }
        public int IdType { get; set; }
        public string Message { get; set; }
    }
}
