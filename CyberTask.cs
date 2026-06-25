using System;
using System.Collections.Generic;
using System.Text;

namespace THEPART2
{
    /// <summary>
    /// Represents a single cybersecurity task. Named CyberTask (not Task)
    /// to avoid conflict with System.Threading.Tasks.Task.
    /// </summary>
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
