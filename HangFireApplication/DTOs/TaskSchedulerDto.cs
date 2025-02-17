using System.ComponentModel.DataAnnotations;

namespace HangFireApplication.DTOs
{
    public class TaskSchedulerDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ExecutablePath { get; set; }

        public string? Arguments { get; set; }

        public string? CronExpression { get; set; }

        public int? IntervalInMinutes { get; set; }

        public bool Enable { get; set; }

        public Guid? DependentTaskId { get; set; }
    }
}
