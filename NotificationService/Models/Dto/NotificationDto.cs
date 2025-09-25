namespace NotificationService.Models.Dto;
public class NotificationDto
{
    public int Id { get; set; }
    public int? IdType { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public int IdUser { get; set; }
    public DateTime CreateTime { get; set; }
}
