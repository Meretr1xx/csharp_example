namespace Project_tracker.Models
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string RoleInProject { get; set; } = "Developer"; // TeamLead, Developer, QA и т.д.
    }
}
