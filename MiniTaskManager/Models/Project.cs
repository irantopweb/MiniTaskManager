namespace MiniTaskManager.Models;

public class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
