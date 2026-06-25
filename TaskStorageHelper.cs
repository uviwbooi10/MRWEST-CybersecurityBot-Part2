using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;


namespace THEPART2
{
    /// <summary>
    /// Handles all reading and writing of the tasks.json file.
    /// All file I/O for tasks lives here and nowhere else.
    /// </summary>
    public class TaskStorageHelper
    {
        private const string FilePath = "tasks.json";

        /// <summary>
        /// Reads tasks.json and deserialises it into a list of CyberTask.
        /// Returns an empty list if the file does not exist or is empty.
        /// </summary>
        public List<CyberTask> LoadTasks()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<CyberTask>();

                string json = File.ReadAllText(FilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<CyberTask>();

                var tasks = JsonConvert.DeserializeObject<List<CyberTask>>(json);
                return tasks ?? new List<CyberTask>();
            }
            catch (Exception)
            {
                // If the file is corrupted or unreadable, start fresh rather than crash
                return new List<CyberTask>();
            }
        }

        /// <summary>Serialises the task list to JSON and writes it to tasks.json.</summary>
        public void SaveTasks(List<CyberTask> tasks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TaskStorageHelper] Failed to save tasks: {ex.Message}");
            }
        }

        /// <summary>Adds a new task with an auto-incremented Id and saves it.</summary>
        public CyberTask AddTask(string title, string description, string reminder)
        {
            var tasks = LoadTasks();
            int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;

            var newTask = new CyberTask
            {
                Id = newId,
                Title = title,
                Description = description,
                Reminder = reminder ?? string.Empty,
                IsComplete = false,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            tasks.Add(newTask);
            SaveTasks(tasks);
            return newTask;
        }

        /// <summary>Marks the task with the given Id as complete.</summary>
        public bool MarkAsComplete(int id)
        {
            var tasks = LoadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.IsComplete = true;
            SaveTasks(tasks);
            return true;
        }

        /// <summary>Removes the task with the given Id.</summary>
        public bool DeleteTask(int id)
        {
            var tasks = LoadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            tasks.Remove(task);
            SaveTasks(tasks);
            return true;
        }
    }
}
