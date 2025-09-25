namespace NotificationService.Clients;
using NotificationService.Models.Dto;
public class TaskClient
{
    private readonly HttpClient _httpClient;

    public TaskClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TaskDto?> GetNotificationById(int idTask)
    {
        var response = await _httpClient.GetAsync($"/api/tasks/{idTask}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<TaskDto>();
    }
}