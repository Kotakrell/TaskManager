using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Clients;
using NotificationService.Hubs;
using NotificationService.Models.Entities;
using NotificationService.Models.Entities;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<NotificationServices>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddDbContext<NotificationServiceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TaskServiceDb")));
builder.Services.AddHttpClient<TaskClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/hubs/notifications");

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
