using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TaskService.Models.DTO
{
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
