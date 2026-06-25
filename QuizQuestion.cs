using System;
using System.Collections.Generic;
using System.Text;

namespace THEPART2
{
    /// <summary>
    /// Represents one quiz question, either multiple choice or true/false.
    /// </summary>
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty; // e.g. "C" or "True"
        public string Explanation { get; set; } = string.Empty;
        public bool IsTrueFalse { get; set; }
        public string Topic { get; set; } = string.Empty;
    }
}
