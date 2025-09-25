namespace TaskService.Services;

using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TaskService.Clients;
using TaskService.Models.DTO;
using TaskService.Models.Entities;

public class TaskServices
{
    private readonly TaskServiceContext _context;
    private readonly NotificationClient _notificationClient;
    private readonly ILogger<TaskServices> _logger;

    public TaskServices(TaskServiceContext context, NotificationClient notificationClient, ILogger<TaskServices> logger)
    {
        _context = context;
        _notificationClient = notificationClient;
        _logger = logger;
    }
    public async Task<TaskDto> CreateTask(CreateTaskDto dto)
    {
        _logger.LogInformation("Попытка создать задачу.");
        var task = new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            IdStatus = 1,
            CreateTime = DateTime.Now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        TaskDto taskDto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IdStatus = task.IdStatus,
            Status = _context.TaskStatuses.Where(ts => ts.Id == task.IdStatus).Select(ts => ts.Name).FirstOrDefault(),
            CreateTime = task.CreateTime
        };

        var taskHistory = new TaskHistory
        {
            IdTask = task.Id,
            IdOldStatus = null,
            IdNewStatus = 1,
            ChangeTime = DateTime.Now
        };
        _logger.LogInformation("Создание задачи {idTask} успешно сохранено в истории изменений.", taskDto.Id);
        _context.TaskHistories.Add(taskHistory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Задача {idTask} успешно создана.", taskDto.Id);
        return taskDto;
    }

    public async Task<TaskDto> UpdateTask(int idTask, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(idTask);
        _logger.LogInformation("Попытка обновить задачу {idTask}.", task.Id);
        if (task == null)
        {
            _logger.LogWarning("Задача {idTask} не найдена.", task.Id);
            return null;
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IdStatus = 1;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        var assignedUsers = await _context.TaskAssignments
            .Where(ta => ta.IdTask == idTask)
            .Select(ta => ta.IdUser)
            .ToListAsync();

        await _notificationClient.SendTaskNotification(idTask, 2);
        _logger.LogInformation("Уведомление по обновлению задачи успешно отправлено.");

        TaskDto taskDto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IdStatus = task.IdStatus,
            Status = _context.TaskStatuses.Where(ts => ts.Id == task.IdStatus).Select(ts => ts.Name).FirstOrDefault(),
            CreateTime = task.CreateTime
        };

        var taskHistory = new TaskHistory
        {
            IdTask = task.Id,
            IdOldStatus = 1,
            IdNewStatus = 1,
            ChangeTime = DateTime.Now
        };
        _logger.LogInformation("Изменение задачи {idTask} успешно сохранено в истории изменений.", task.Id);
        _context.TaskHistories.Add(taskHistory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Задача {idTask} успешно создана.", task.Id);
        return taskDto;
    }
    public async Task<bool> DeleteTask(int idTask)
    {
        var task = await _context.Tasks.FindAsync(idTask);
        _logger.LogInformation("Попытка удалить задачу {idTask}.", idTask);
        if (task == null)
        {
            _logger.LogWarning("Задача {idTask} не найдена.", idTask);
            return false;
        }

        task.IdStatus = 2;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        var assignedUsers = await _context.TaskAssignments
            .Where(ta => ta.IdTask == idTask)
            .Select(ta => ta.IdUser)
            .ToListAsync();

        foreach (var idUser in assignedUsers)
        {
            _logger.LogInformation("Уведомление по удалению задачи успешно отправлено {idUser}.", idUser);
            await _notificationClient.SendTaskNotification(idTask, 3);
        }

        var taskHistory = new TaskHistory
        {
            IdTask = task.Id,
            IdOldStatus = 1,
            IdNewStatus = 2,
            ChangeTime = DateTime.Now
        };
        _logger.LogInformation("Удаление задачи {idTask} успешно сохранено в истории изменений.", task.Id);
        _context.TaskHistories.Add(taskHistory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Задача {idTask} успешно удалена.", task.Id);
        return true;
    }
    public async Task<List<TaskDto>> GetTasks()
    {
        _logger.LogInformation("Получение данных об задачах.");
        _logger.LogInformation("Данные о задачах получены.");
        return await _context.Tasks
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IdStatus = t.IdStatus
            })
            .ToListAsync();
    }
    public async Task<TaskDto?> GetTaskById(int idUser)
    {
        var task = await _context.Tasks.FindAsync(idUser);
        _logger.LogInformation("Получение данных о задаче {idUser}.", idUser);
        if (task == null)
        {
            _logger.LogWarning("Задача {idUser} не найдена.", idUser);
            return null;
        }

        _logger.LogInformation("Данные о задаче {idUser} успешно получены.", idUser);
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IdStatus = task.IdStatus,
            Status = _context.TaskStatuses.Where(ts => ts.Id == task.IdStatus).Select(ts => ts.Name).FirstOrDefault(),
            CreateTime = task.CreateTime
        };
    }
    public async Task<bool> AssignTask(int idTask, int idUser)
    {
        _logger.LogInformation("Попытка привязать пользователя {userId} к задаче {idTask}", idUser, idTask);
        var exists = await _context.TaskAssignments
            .AnyAsync(ta => ta.IdTask == idTask && ta.IdUser == idUser);
        if (exists)
        {
            _logger.LogWarning("Пользователь {idUser} уже привязан к задачe {idTask}.", idUser, idTask);
            return false;
        }
        var task = await _context.Tasks.FindAsync(idTask);
        if (task == null)
        {
            _logger.LogWarning("Задачи {idTask} не существует.", idTask);
            return false;
        }
        var user = await _context.Users.FindAsync(idUser);
        if (user == null)
        {
            _logger.LogWarning("Пользователя {idUser} не существует.", idUser);
            return false;
        }

        var assignment = new TaskAssignment
        {
            IdTask = idTask,
            IdUser = idUser
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Уведомление по прикреплению задачи {idTask} успешно отправлено пользователю {idUser}.", idTask, idUser);
        await _notificationClient.SendTaskNotification(idTask, 1);

        _logger.LogInformation("Пользователь {userId} успешно привязан к задаче {idTask}", idUser, idTask);
        return true;
    }
}