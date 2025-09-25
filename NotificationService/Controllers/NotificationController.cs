using Microsoft.AspNetCore.Mvc;
using NotificationService.Clients;
using NotificationService.Models.Dto;
using NotificationService.Models.Entities;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationServices _service;
        private readonly ILogger<NotificationServices> _logger;
        private readonly TaskClient _taskClient;
        public NotificationsController(NotificationServices service, TaskClient taskClient, ILogger<NotificationServices> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto dto)
        {
            _logger.LogInformation("Запрос: создание уведомления.");
            if (dto == null)
            {
                _logger.LogInformation("Уведомление не создано.");
                return BadRequest("Payload не распознан");
            }
            var result = await _service.CreateNotification(dto);
            _logger.LogInformation("Уведомление {NotificationId} создано.", result.Id);
            return Ok(result);
        }

        [HttpGet("{idUser}")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications([FromRoute] int idUser)
        {
            _logger.LogInformation("Запрос: получение уведомлений пользователя {userId}.", idUser);
            var notifications = await _service.GetNotifications(idUser);
            _logger.LogInformation("Уведомления пользователя {userId} получены.", idUser);
            return Ok(notifications);
        }

        [HttpPut("{id}/mark-as-read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            _logger.LogInformation("Запрос: пометить уведомление {NotificationId} как прочитанное.", id);
            var result = await _service.MarkAsRead(id);
            if (!result)
            {
                _logger.LogWarning("Уведомление не найдено {NotificationId} или уже прочитано.", id);
                return NotFound();
            }
            _logger.LogInformation("Уведомление {NotificationId} успешно помечено как прочитанное.", id);
            return NoContent();
        }
    }
}
