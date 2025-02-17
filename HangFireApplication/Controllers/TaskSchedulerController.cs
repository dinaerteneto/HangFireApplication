using HangFireApplication.DTOs;
using HangFireApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangFireApplication.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TaskSchedulerController : ControllerBase
    {
        private readonly TaskSchedulerService _taskSchedulerService;

        public TaskSchedulerController(TaskSchedulerService taskSchedulerService)
        {
            _taskSchedulerService = taskSchedulerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskSchedulerDto dto)
        {
            var task = await _taskSchedulerService.CreateTaskAsync(dto);
            return Ok(new { message = "Task scheduled successfully!", taskId = task.Id });
        }
    }
}
