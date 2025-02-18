using Hangfire;
using System.Diagnostics;
using HangFireApplication.Repositories;
using HangFireApplication.DTOs;
using Hangfire.Storage;

namespace HangFireApplication.Services
{
    public class TaskSchedulerService
    {
        private readonly ITaskSchedulerRepository _taskSchedulerRepository;
        private readonly IRecurringJobManager _recurringJobManager;

        public TaskSchedulerService(ITaskSchedulerRepository taskSchedulerRepository, IRecurringJobManager recurringJobManager)
        {
            _taskSchedulerRepository = taskSchedulerRepository;
            _recurringJobManager = recurringJobManager;
        }

        public async Task<Models.TaskScheduler> CreateTaskAsync(TaskSchedulerDto taskDto)
        {
            var taskScheduler = new Models.TaskScheduler
            {
                Id = Guid.NewGuid(),
                Name = taskDto.Name,
                ExecutablePath = taskDto.ExecutablePath,
                Arguments = taskDto.Arguments,
                CronExpression = taskDto.CronExpression,
                IntervalInMinutes = taskDto.IntervalInMinutes,
                Enable = taskDto.Enable,
                DependentTaskId = taskDto.DependentTaskId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _taskSchedulerRepository.AddAsync(taskScheduler);
            return taskScheduler;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExecuteScheduledTask(Guid taskId)
        {
            try
            {
                var task = _taskSchedulerRepository.GetTaskById(taskId);

                var result = RunExecutable(task.ExecutablePath, task.Arguments);

                // Lógica adicional para sucesso
                Console.WriteLine($"Task {task.Name} executed successfully.");
            }
            catch (Exception ex)
            {
                // Log de erro
                Console.Error.WriteLine($"Error executing task {taskId}: {ex.Message}");

                // Aqui o Hangfire já marcará a tarefa como falha se uma exceção for lançada,
                // mas podemos fazer uma customização aqui, se necessário.

                // Por exemplo, podemos usar uma funcionalidade do Hangfire para registrar a falha
                // manualmente, ou apenas deixar o Hangfire tratar automaticamente.
                throw; // Lança novamente para Hangfire registrar o erro corretamente
            }
        }

        public void ScheduleTasksFromDatabase()
        {
            var tasks = _taskSchedulerRepository.GetScheduledTasks();

            foreach (var task in tasks)
            {
                Console.WriteLine($"Scheduling task: {task.Id}");
                if (task.Enable && !string.IsNullOrEmpty(task.CronExpression))
                {
                    _recurringJobManager.AddOrUpdate(
                        task.Name,
                        () => ExecuteScheduledTask(task.Id),
                        task.CronExpression
                    );
                }
            }
        }

        private string RunExecutable(string executablePath, string arguments)
        {
            // Executar o .exe com argumentos
            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception($"Executable failed with error: {error}");
                    }
                }

                using (var reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
