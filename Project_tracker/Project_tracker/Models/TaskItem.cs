using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Project_tracker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "To Do"; // To Do, In Progress, Done
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Blocker

        public DateTime CreatedAt
        {
            get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

        // Связь с проектом
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Исполнитель (может быть не назначен)
        public int? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }

        // Навигационные свойства
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
