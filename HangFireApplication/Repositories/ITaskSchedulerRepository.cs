namespace HangFireApplication.Repositories
{
    public interface ITaskSchedulerRepository
    {
        Task AddAsync(Models.TaskScheduler task);
        IQueryable<Models.TaskScheduler> GetScheduledTasks();
        Models.TaskScheduler GetTaskById(Guid taskId);
    }
}
