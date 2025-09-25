using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskService.Models.DTO;
using TaskService.Models.Entities;
using TaskService.Services;

namespace TaskService.Clients
{
    public class NotificationClient
    {
        private readonly HttpClient _client;
        private readonly TaskServiceContext _context;
        private readonly ILogger<TaskServices> _logger;

        public NotificationClient(HttpClient client, TaskServiceContext context, ILogger<TaskServices> logger)
        {
            _client = client;
            _context = context;
            _logger = logger;
        }

        public async Task SendTaskNotification(int idTask, int idType)
        {
            var task = await _context.Tasks.Include(t => t.TaskAssignments).FirstOrDefaultAsync(t => t.Id == idTask);
            if (task.TaskAssignments == null || !task.TaskAssignments.Any())
            {
                _logger.LogWarning("Нет пользователей для уведомления по задаче {TaskId}", idTask);
                return;
            }
            foreach (var assignment in task.TaskAssignments)
            {
                var dto = new CreateNotificationDto
                {
                    IdUser = assignment.IdUser,
                    IdTask = idTask,
                    IdType = idType,
                    Message = task.Title
                };

                _logger.LogInformation("Отправка уведомления: TaskId={TaskId}, UserId={UserId}, Type={Type}, Message={Message}", idTask, assignment.IdUser, idType, task.Title);
                var response = await _client.PostAsJsonAsync("/api/notifications", dto);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Ошибка при отправке уведомления для задачи {TaskId}", idTask);
                    throw new HttpRequestException(
                        $"Ошибка при отправке уведомления. StatusCode: {response.StatusCode}"
                    );
                }
            }
        }
    }
}
