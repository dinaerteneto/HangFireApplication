using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HangFireApplication.Models
{
    public class TaskScheduler
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        public string ExecutablePath { get; set; }

        public string? Arguments { get; set; }

        public string? CronExpression { get; set; }

        public int? IntervalInMinutes { get; set; }
        public bool Enable { get; set; }

        public Guid? DependentTaskId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DependentTaskId")]
        public TaskScheduler? DependentTask { get; set; }
    }
}
