using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskService.Models.DTO;
using TaskService.Services;
//using TaskService.Services;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly TaskServices _taskServices;
        private readonly ILogger<TaskServices> _logger;
        public TaskController(TaskServices taskServices, ILogger<TaskServices> logger)
        {
            _taskServices = taskServices;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetTasks()
        {
            _logger.LogInformation("Запрос: получение всех задач.");
            var tasks = await _taskServices.GetTasks();
            _logger.LogInformation("Все задачи успешно получены.");
            return Ok(tasks);
        }
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
        {
            _logger.LogInformation("Запрос: создание задачи.");
            var task = await _taskServices.CreateTask(dto);
            _logger.LogInformation("Задача {idTask} успешно создана.", task.Id);
            return Ok(task);
        }
        [HttpGet("{idTask}")]
        public async Task<ActionResult<TaskDto>> GetTaskById([FromRoute] int idTask)
        {
            try
            {
                _logger.LogInformation("Запрос: получение задачи {idUser}.", idTask);
                var task = await _taskServices.GetTaskById(idTask);
                _logger.LogInformation("Задача {idUser} получена.", idTask);
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Задача {idTask} не найдена.", idTask);
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{idTask}")]
        public async Task<ActionResult<TaskDto>> UpdateTask([FromRoute] int idTask, UpdateTaskDto dto)
        {
            try
            {
                _logger.LogInformation("Запрос: обновление задачи {idUser}.", idTask);
                var task = await _taskServices.UpdateTask(idTask, dto);
                _logger.LogInformation("Задача {idUser} обновлена.", idTask);
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Задача {idTask} не найдена.", idTask);
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{idTask}")]
        public async Task<ActionResult<bool>> DeleteTask([FromRoute] int idTask)
        {
            try
            {
                _logger.LogInformation("Запрос: удаление задачи {idUser}.", idTask);
                var task = await _taskServices.DeleteTask(idTask);
                _logger.LogInformation("Задача {idUser} удалена.", idTask);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Задача {idTask} не найдена.", idTask);
                return NotFound(ex.Message);
            }
        }
        [HttpPost("{idTask}/assign")]
        public async Task<ActionResult> AssignUsersToTask([FromRoute] int idTask, AssignTaskDto dto)
        {
            try
            {
                _logger.LogInformation("Запрос: назначение исполнитеоя {idUser} для задачи {idTask}.", dto.IdUser, idTask);
                var result = await _taskServices.AssignTask(idTask, dto.IdUser);
                if (!result)
                {
                    _logger.LogWarning("Пользователь {idUser} уже назначен на задачу {idTask}.", dto.IdUser, idTask);
                    return BadRequest("Пользователь уже назначен на задачу");
                }
                _logger.LogInformation("Пользователь {idUser} назначен на задачу {idTask}", dto.IdUser, idTask);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Задача {idTask} или пользователь {idUser} не найдены.", idTask, dto.IdUser);
                return NotFound(ex.Message);
            }
        }
    }
}
