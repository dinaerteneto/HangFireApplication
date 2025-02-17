using Hangfire;
using Hangfire.SqlServer;

using HangFireApplication.Database;
using HangFireApplication.Repositories;
using HangFireApplication.Services;
using HangfireBasicAuthenticationFilter;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Hangfire
builder.Services.AddHangfire((sp, config) =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            });
});

builder.Services.AddHangfireServer();

// Injeção de dependência
builder.Services.AddScoped<ITaskSchedulerRepository, TaskSchedulerRepository>();
builder.Services.AddScoped<TaskSchedulerService>();

// Adicionar controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agendar o serviço de agendamento ao iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var taskSchedulerService = scope.ServiceProvider.GetRequiredService<TaskSchedulerService>();

    // Chama ao iniciar a aplicação para carregar os agendamentos
    taskSchedulerService.ScheduleTasksFromDatabase();

    // Agendar a execução periódica do serviço para atualizar os agendamentos
    RecurringJob.AddOrUpdate(
        "update-scheduled-tasks",
        () => taskSchedulerService.ScheduleTasksFromDatabase(),
        "*/5 * * * *" // Roda a cada 5 minutos
    );

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configuração do Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "HangFire Dashboard",
    DisplayStorageConnectionString = false,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "admin"
        }
    }
});

app.Run();
