using HangFireApplication.Database;


namespace HangFireApplication.Repositories
{
    public class TaskSchedulerRepository : ITaskSchedulerRepository
    {

        private readonly ApplicationDbContext _context;

        public TaskSchedulerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Models.TaskScheduler task)
        {
            _context.TaskSchedulers.Add(task);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Models.TaskScheduler> GetScheduledTasks()
        {
            return _context.TaskSchedulers.Where(t => t.Enable); // Filtra apenas as tarefas ativas
        }

        public Models.TaskScheduler GetTaskById(Guid taskId)
        {
            return _context.TaskSchedulers.Find(taskId);
        }
    }
}
