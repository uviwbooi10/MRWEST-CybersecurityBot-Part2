namespace THEPART2
{
    /// <summary>
    /// Records every significant action the chatbot takes (tasks, reminders,
    /// quiz events, NLP matches) with a timestamp. Displayed when the user
    /// asks "show activity log" or "what have you done for me?".
    /// </summary>
    public class ActivityLogger
    {
        private readonly List<string> _log = new();

        /// <summary>Adds a new timestamped entry to the log.</summary>
        public void Log(string action)
        {
            string entry = DateTime.Now.ToString("[HH:mm] ") + action;
            _log.Add(entry);
        }

        /// <summary>
        /// Returns the most recent log entries (default 10) as a numbered list.
        /// If fewer entries exist, returns all of them.
        /// </summary>
        public string GetRecentLog(int count = 10)
        {
            if (_log.Count == 0)
                return "No activity recorded yet. Try adding a task or asking a cybersecurity question!";

            int startIndex = Math.Max(0, _log.Count - count);
            var recent = _log.Skip(startIndex).ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(" Here's a summary of recent actions:");
            for (int i = 0; i < recent.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {recent[i]}");
            }

            if (_log.Count > count)
                sb.Append($"\n There are {_log.Count - count} more entries. Type 'show more' to see the full history.");

            return sb.ToString().TrimEnd();
        }

        /// <summary>Returns the complete log history as a numbered list.</summary>
        public string GetFullLog()
        {
            if (_log.Count == 0)
                return "No activity recorded yet.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($" Full activity history ({_log.Count} entries):");
            for (int i = 0; i < _log.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_log[i]}");
            }
            return sb.ToString().TrimEnd();
        }

        /// <summary>Returns the total number of log entries.</summary>
        public int GetCount() => _log.Count;
    }
}
