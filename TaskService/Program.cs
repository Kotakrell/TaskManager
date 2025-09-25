using Microsoft.EntityFrameworkCore;
using TaskService.Clients;
using TaskService.Models.Entities;
using TaskService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TaskServices>();
builder.Services.AddHttpClient<NotificationClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5002");
});
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskServiceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TaskDatabase")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
