using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Clients;
using NotificationService.Hubs;
using NotificationService.Models.Dto;
using NotificationService.Models.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace NotificationService.Services
{
    public class NotificationServices
    {
        private readonly NotificationServiceContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationServices> _logger;
        private readonly TaskClient _taskClient;

        public NotificationServices(NotificationServiceContext context, IHubContext<NotificationHub> hubContext, ILogger<NotificationServices> logger, TaskClient taskClient)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
            _taskClient = taskClient;
        }

        public async Task<NotificationDto> CreateNotification([FromBody] CreateNotificationDto dto)
        {
            var task = await _taskClient.GetNotificationById(dto.IdTask);
            var type = await _context.NotificationTypes.FindAsync(dto.IdType);
            _logger.LogInformation("Попытка создать уведомление для задачи {TaskId}", dto.IdTask);
            if (type == null)
            {
                _logger.LogWarning("Тип уведомления не существует");
                return null;
            }
            if (task == null)
            {
                _logger.LogWarning("Задачи не существует");
                return null;
            }

            var notification = new Notification
            {
                IdUser = dto.IdUser,
                IdType = dto.IdType,
                IdTask = dto.IdTask,
                Message = dto.Message,
                CreateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var readStatus = new NotificationReadStatus
            {
                IdNotification = notification.Id,
                ReadAt = null
            };
            _context.NotificationReadStatuses.Add(readStatus);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(dto.IdUser.ToString())
                .SendAsync("ReceiveNotification", notification.Message);
            _logger.LogInformation("Уведомление {NotificationId} успешно создано для задачи {TaskId}", notification.Id, dto.IdTask);

            return new NotificationDto
            {
                Id = notification.Id,
                IdType = notification.IdType,
                Type = type.Name,
                Message = notification.Message,
                IsRead = false,
                CreateTime = notification.CreateTime
            };
        }

        public async Task<List<NotificationDto>> GetNotifications(int userId)
        {
            _logger.LogInformation("Получение данных об уведомлениях.");
            var notifications = await _context.Notifications
                .Include(n => n.Type)
                .Include(n => n.NotificationReadStatuses)
                .Where(n => n.IdUser == userId)
                .ToListAsync();

            _logger.LogInformation("Данные об уведомлениях получены.");
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type.Name,
                Message = n.Message,
                IsRead = n.NotificationReadStatuses.FirstOrDefault()?.ReadAt != null,
                CreateTime = n.CreateTime
            }).ToList();
        }

        public async Task<bool> MarkAsRead(int notificationId)
        {
            _logger.LogInformation("Попытка пометить уведомление {NotificationId} как прочитанное", notificationId);
            var notification = await _context.Notifications
                .FindAsync(notificationId);

            if (notification == null)
            {
                _logger.LogWarning("Уведомление {NotificationId} не найдено", notificationId);
                return false;
            }
                
            var status = await _context.NotificationReadStatuses
                .FirstOrDefaultAsync(r => r.IdNotification == notificationId);

            if (status != null && status.ReadAt != null)
            {
                _logger.LogInformation("Уведомление {NotificationId} уже прочитано", notificationId);
                return false;
            }

            if (status == null)
            {
                status = new NotificationReadStatus
                {
                    IdNotification = notificationId,
                    ReadAt = DateTime.Now
                };
                _context.NotificationReadStatuses.Add(status);
            }
            else
            {
                status.ReadAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Уведомление {NotificationId} успешно помечено как прочитано", notificationId);
            return true;
        }

    }
}
