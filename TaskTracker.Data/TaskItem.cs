using System;

namespace TaskTracker.Data
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "[Done]" : "[To do]";
            return $"{status} {Title}";
        }
    }
}