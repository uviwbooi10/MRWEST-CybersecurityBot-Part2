using THEPART2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THEPART2
{
    /// <summary>
    /// Sits between the GUI/ChatBot and TaskStorageHelper.
    /// Handles task operations and logs each action via ActivityLogger.
    /// </summary>
    public class TaskManager
    {
        private readonly TaskStorageHelper _storage;
        private readonly ActivityLogger _logger;

        public TaskManager(ActivityLogger logger)
        {
            _storage = new TaskStorageHelper();
            _logger = logger;
        }

        /// <summary>Adds a task, logs the action, and returns a confirmation message.</summary>
        public string AddTask(string title, string description, string reminder = "")
        {
            var task = _storage.AddTask(title, description, reminder);

            string reminderText = string.IsNullOrEmpty(reminder)
                ? "(no reminder set)"
                : $"(Reminder set: {reminder})";

            _logger.Log($"Task added: '{task.Title}' {reminderText}");

            return string.IsNullOrEmpty(reminder)
                ? $"Task added with the description '{description}'. Would you like to set a reminder?"
                : $"Task added: '{title}'. {reminderText}";
        }

        /// <summary>Returns all tasks currently stored.</summary>
        public List<THEPART2.CyberTask> GetAllTasks() => _storage.LoadTasks();

        /// <summary>Marks a task complete and logs the action.</summary>
        public bool MarkAsComplete(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            bool success = _storage.MarkAsComplete(id);

            if (success && task != null)
                _logger.Log($"Task marked complete: '{task.Title}'");

            return success;
        }

        /// <summary>Deletes a task and logs the action.</summary>
        public bool DeleteTask(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            bool success = _storage.DeleteTask(id);

            if (success && task != null)
                _logger.Log($"Task deleted: '{task.Title}'");

            return success;
        }

        /// <summary>Attaches a reminder to the most recently added task.</summary>
        public string SetReminderOnLastTask(string reminder)
        {
            var tasks = _storage.LoadTasks();
            if (tasks.Count == 0)
                return "There's no task to set a reminder for yet. Add a task first!";

            var lastTask = tasks.OrderByDescending(t => t.Id).First();
            lastTask.Reminder = reminder;
            _storage.SaveTasks(tasks);

            _logger.Log($"Reminder set: '{lastTask.Title}' — {reminder}");

            return $"Got it! I'll remind you: {reminder}";
        }
    }
}
