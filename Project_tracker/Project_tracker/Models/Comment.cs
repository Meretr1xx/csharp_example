using System.ComponentModel.DataAnnotations;
namespace Project_tracker.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // К какой задаче относится
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;

        // Кто написал
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
