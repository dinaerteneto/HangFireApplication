using Hangfire;
using System.Diagnostics;
using HangFireApplication.Repositories;
using HangFireApplication.DTOs;

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

        [AutomaticRetry(Attempts = 3)]
        public void ExecuteScheduledTask(Guid taskId)
        {
            var task = _taskSchedulerRepository.GetTaskById(taskId);
            if (task == null || !task.Enable || string.IsNullOrWhiteSpace(task.ExecutablePath))
                return;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = task.ExecutablePath,
                        Arguments = task.Arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine($"[Hangfire] Executado: {task.Name}");
                if (!string.IsNullOrEmpty(output)) Console.WriteLine($"[Saída] {output}");
                if (!string.IsNullOrEmpty(error)) Console.WriteLine($"[Erro] {error}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro] {task.Name}: {ex.Message}");
                throw;
            }
        }

        public void ScheduleTasksFromDatabase()
        {
            var tasks = _taskSchedulerRepository.GetScheduledTasks();

            foreach (var task in tasks)
            {
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
    }
}
