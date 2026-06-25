using System;
using System.Collections.Generic;
using System.Linq;

// ============================================================
// File: ChatBot.cs - Central routing logic (Part 1+2+3 combined)
// ============================================================

namespace THEPART2
{
    /// <summary>
    /// Central chatbot class. MainWindow only calls ProcessInput() on this class.
    /// Routes input through: name capture -> reminder capture -> quiz mode ->
    /// NLP intents (task/reminder/quiz/log) -> follow-up -> sentiment ->
    /// keywords -> fallback.
    /// </summary>
    public class ChatBot
    {
        private readonly KeywordResponder _keywords;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;
        private readonly ActivityLogger _activityLogger;
        private readonly TaskManager _taskManager;
        private readonly QuizManager _quizManager;

        private bool _awaitingName = true;
        private bool _awaitingReminder = false;
        private bool _quizActive = false;
        private string _lastTopic = string.Empty;
        private string _pendingTaskTitle = string.Empty;

        private readonly List<string> _followUpPhrases = new()
        {
            "tell me more", "explain more", "more info", "give me more",
            "continue", "go on", "and then", "what else", "more please",
            "keep going", "elaborate"
        };

        private readonly List<string> _fallbackResponses = new()
        {
            "I'm not sure I understood that. Try asking about: passwords, phishing, malware, privacy, scams, VPN, firewall, 2FA, encryption, ransomware, or browsing.",
            "Hmm, I didn't quite catch that. Type 'help' to see all the topics I can assist with!",
            "I'm still learning! Could you rephrase that? Or ask me about a specific cybersecurity topic.",
            "That one's a bit outside my knowledge. Try asking about phishing, passwords, or data breaches!",
            "I didn't recognise that — but I'm great at cybersecurity topics! Type 'help' to see what I know."
        };

        private readonly Random _random = new();

        // ─────────────────────────────────────────────────────
        // NLP Intent phrase groups (Part 3)
        // ─────────────────────────────────────────────────────

        private readonly string[] _addTaskPhrases =
        {
            "add task", "add a task", "create task", "create a task",
            "i need to", "new task", "remind me to add"
        };

        private readonly string[] _reminderPhrases =
        {
            "remind me", "reminder", "set a reminder", "remind me to",
            "don't forget", "dont forget"
        };

        private readonly string[] _quizPhrases =
        {
            "start quiz", "take quiz", "test my knowledge", "quiz me",
            "play the game", "start the quiz", "begin quiz"
        };

        private readonly string[] _logPhrases =
        {
            "show activity log", "what have you done", "what did you do",
            "show log", "recent actions", "activity log", "what have you done for me"
        };

        public ChatBot()
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
            _activityLogger = new ActivityLogger();
            _taskManager = new TaskManager(_activityLogger);
            _quizManager = new QuizManager();
        }

        /// <summary>Exposes the TaskManager so the GUI task panel can read/update tasks directly.</summary>
        public TaskManager Tasks => _taskManager;

        /// <summary>Exposes the ActivityLogger so the GUI can display it if needed.</summary>
        public ActivityLogger Logger => _activityLogger;

        /// <summary>Returns the opening greeting message shown on app launch.</summary>
        public string GetGreeting()
        {
            return " Hello! Welcome to the MrWest Cybersecurity Bot!\n\nI'm here to help you stay safe online, manage tasks, and test your knowledge. What's your name?";
        }

        // ─────────────────────────────────────────────────────
        // Public wrappers so the Quiz GUI tab can drive the quiz
        // directly without going through chat text input.
        // ─────────────────────────────────────────────────────

        /// <summary>Starts a new quiz. Used by the Quiz tab's Start button.</summary>
        public string StartQuizFromGui() => StartQuiz();

        /// <summary>Submits an answer from the Quiz tab UI.</summary>
        public string SubmitQuizAnswerFromGui(string answer) => HandleQuizAnswer(answer);

        /// <summary>Exposes the current question for the Quiz tab to render options.</summary>
        public QuizQuestion? GetCurrentQuizQuestion() => _quizManager.GetCurrentQuestion();

        /// <summary>True while a quiz is in progress.</summary>
        public bool IsQuizActive => _quizActive;

        /// <summary>
        /// Main routing method. Takes raw user input and returns the bot's response.
        /// </summary>
        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return " Please type something — I'm here to help!";

            string trimmed = input.Trim();
            string lower = trimmed.ToLower();

            // Step 0: Capture name on first message
            if (_awaitingName)
            {
                _memory.UserName = trimmed;
                _memory.Store("name", trimmed);
                _awaitingName = false;
                _activityLogger.Log($"User '{trimmed}' started a session");
                return $"Nice to meet you, {_memory.UserName}! \n\nI can help with cybersecurity topics, manage tasks, run a quiz, or show my activity log. Type 'help' to see everything I can do!";
            }

            // Step 0.5: Quiz mode takes over input completely while active
            if (_quizActive)
            {
                return HandleQuizAnswer(trimmed);
            }

            // Step 0.6: Awaiting a reminder for the last added task
            if (_awaitingReminder)
            {
                _awaitingReminder = false;
                string reminderResult = _taskManager.SetReminderOnLastTask(trimmed);
                return reminderResult;
            }

            // Help command
            if (lower == "help" || lower == "topics" || lower == "menu")
                return GetHelpMessage();

            // Show more (activity log continuation)
            if (lower == "show more" || lower == "more entries" || lower == "show full log")
            {
                _activityLogger.Log("NLP recognised log intent: 'show more'");
                return _activityLogger.GetFullLog();
            }

            // ── NLP INTENT DETECTION (Part 3) — checked before Part 2 flow ──

            // Intent: Show activity log
            if (_logPhrases.Any(p => lower.Contains(p)))
            {
                _activityLogger.Log($"NLP recognised log intent from: '{trimmed}'");
                return _activityLogger.GetRecentLog(10);
            }

            // Intent: Start quiz
            if (_quizPhrases.Any(p => lower.Contains(p)))
            {
                return StartQuiz();
            }

            // Intent: Set a reminder (standalone, not right after a task)
            if (_reminderPhrases.Any(p => lower.Contains(p)))
            {
                string reminderTarget = ExtractAfterPhrase(trimmed, _reminderPhrases);
                _activityLogger.Log($"NLP recognised reminder intent from: '{trimmed}'");

                if (!string.IsNullOrWhiteSpace(reminderTarget))
                {
                    _activityLogger.Log($"Reminder set: '{reminderTarget}'");
                    return $" Reminder set for '{reminderTarget}'.";
                }
                return " Got it, what would you like me to remind you about?";
            }

            // Intent: Add a task
            if (_addTaskPhrases.Any(p => lower.Contains(p)))
            {
                string taskTitle = ExtractAfterPhrase(trimmed, _addTaskPhrases);
                if (string.IsNullOrWhiteSpace(taskTitle))
                    taskTitle = "New cybersecurity task";

                string description = GenerateTaskDescription(taskTitle);
                _activityLogger.Log($"NLP recognised task intent from: '{trimmed}'");

                string confirmation = _taskManager.AddTask(taskTitle, description);
                _awaitingReminder = true;

                return confirmation;
            }

            // ── End NLP intent detection — fall through to Part 2 flow ──

            // How are you
            if (lower.Contains("how are you") || lower.Contains("how r u"))
                return $"I'm running perfectly and ready to keep you safe, {_memory.UserName}!  What can I help you with today?";

            // Purpose
            if (lower.Contains("what is your purpose") || lower.Contains("what do you do") || lower.Contains("your purpose"))
                return $"My purpose is to raise your cybersecurity awareness, {_memory.UserName}! I can also manage tasks, run a quiz, and log my actions. Just ask me anything!";

            // Who are you
            if (lower.Contains("your name") || lower.Contains("who are you"))
                return "I'm the MrWest Cybersecurity Bot — built by Uviwe Booi to keep YOU safe online! ";

            // Greetings
            if (lower == "hello" || lower == "hi" || lower == "hey" || lower.StartsWith("hi ") || lower.StartsWith("hello "))
                return $"Hey {_memory.UserName}!  What can I help you with today?";

            // Follow-up handling
            if (_followUpPhrases.Any(p => lower.Contains(p)))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    string? more = _keywords.GetAnotherResponse(_lastTopic);
                    if (more != null)
                        return $"Here's more on {_lastTopic}, {_memory.UserName}:\n\n{more}";
                }
                return "I don't have a previous topic to continue on. Ask me about any cybersecurity topic and I'll dive deeper!";
            }

            // Detect and store favourite topic from interest statements
            _memory.TryStoreFavouriteTopic(trimmed);

            // Sentiment detection
            Sentiment detected = _sentiment.Detect(lower);
            string sentimentOpener = _sentiment.GetSentimentResponse(detected);

            // Keyword recognition
            string? keywordResponse = _keywords.GetResponse(lower);
            string? matchedKeyword = _keywords.GetMatchedKeyword(lower);

            if (keywordResponse != null)
            {
                _lastTopic = matchedKeyword ?? string.Empty;
                _activityLogger.Log($"Keyword matched: {matchedKeyword} - response delivered");

                string personalOpener = _memory.HasFavouriteTopic && _memory.FavouriteTopic == matchedKeyword
                    ? _memory.GetPersonalisedOpener() + "\n\n"
                    : (!string.IsNullOrEmpty(_memory.UserName) ? $"Good question, {_memory.UserName}!\n\n" : string.Empty);

                string fullResponse = string.Empty;

                if (!string.IsNullOrEmpty(sentimentOpener))
                    fullResponse += sentimentOpener + "\n\n";

                fullResponse += personalOpener + keywordResponse;
                fullResponse += "\n\n Type 'tell me more' for another tip on this topic.";

                return fullResponse;
            }

            // Fallback
            string name = _memory.HasName ? $", {_memory.UserName}" : string.Empty;
            string fallback = _fallbackResponses[_random.Next(_fallbackResponses.Count)];
            return $"I'm not sure I understood that{name}. {fallback}";
        }

        // ─────────────────────────────────────────────────────
        // Quiz Handling
        // ─────────────────────────────────────────────────────

        private string StartQuiz()
        {
            _quizManager.ResetQuiz();
            _quizActive = true;
            _activityLogger.Log("Quiz started");

            var question = _quizManager.GetCurrentQuestion();
            if (question == null)
                return "The quiz has no questions available right now.";

            return FormatQuestion(question);
        }

        private string HandleQuizAnswer(string answer)
        {
            var currentQuestion = _quizManager.GetCurrentQuestion();
            if (currentQuestion == null)
            {
                _quizActive = false;
                return "The quiz has ended. Type 'start quiz' to play again!";
            }

            // Normalise multiple choice letter answers (allow "A", "A)", "a" etc.)
            string normalisedAnswer = answer.Trim().TrimEnd(')').ToUpper();

            bool correct = _quizManager.SubmitAnswer(normalisedAnswer);
            string feedback = _quizManager.GetFeedback(correct, currentQuestion);

            if (_quizManager.IsFinished())
            {
                _quizActive = false;
                string finalScore = _quizManager.GetFinalScore();
                string finalMessage = _quizManager.GetFinalMessage();
                _activityLogger.Log($"Quiz completed - score: {finalScore}");

                return $"{feedback}\n\n Quiz complete! Your final score: {finalScore}\n{finalMessage}\n\nType 'start quiz' to play again.";
            }

            var nextQuestion = _quizManager.GetCurrentQuestion();
            string nextQuestionText = nextQuestion != null ? FormatQuestion(nextQuestion) : string.Empty;

            return $"{feedback}\n\n{nextQuestionText}";
        }

        private string FormatQuestion(QuizQuestion question)
        {
            int number = _quizManager.GetCurrentQuestionNumber();
            int total = _quizManager.GetTotalQuestions();

            string optionsText = string.Join("\n", question.Options);
            string answerHint = question.IsTrueFalse
                ? "Type 'True' or 'False'."
                : "Type the letter of your answer (A, B, C, or D).";

            return $" Question {number}/{total} [{question.Topic}]\n\n{question.Question}\n\n{optionsText}\n\n{answerHint}";
        }

        // ─────────────────────────────────────────────────────
        // NLP Helper Methods
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the meaningful text after a matched intent phrase, used to
        /// pull out the task title or reminder subject from natural phrasing.
        /// </summary>
        private string ExtractAfterPhrase(string input, string[] phrases)
        {
            string lower = input.ToLower();
            foreach (string phrase in phrases.OrderByDescending(p => p.Length))
            {
                int index = lower.IndexOf(phrase);
                if (index >= 0)
                {
                    string remainder = input.Substring(index + phrase.Length).Trim();
                    remainder = remainder.TrimStart('-', ':', ' ').Trim();

                    // Remove trailing punctuation
                    remainder = remainder.TrimEnd('.', '!', '?').Trim();

                    // Capitalise first letter for a clean task title
                    if (remainder.Length > 0)
                        remainder = char.ToUpper(remainder[0]) + remainder.Substring(1);

                    return remainder;
                }
            }
            return string.Empty;
        }

        /// <summary>Generates a simple description for a task based on its title.</summary>
        private string GenerateTaskDescription(string title)
        {
            string lower = title.ToLower();

            if (lower.Contains("password"))
                return "Update and strengthen your account password for better security.";
            if (lower.Contains("two-factor") || lower.Contains("2fa"))
                return "Set up 2FA on all important accounts for extra protection.";
            if (lower.Contains("privacy"))
                return "Review account privacy settings to ensure your data is protected.";
            if (lower.Contains("antivirus") || lower.Contains("malware"))
                return "Update antivirus software to protect against the latest threats.";
            if (lower.Contains("backup"))
                return "Back up important files to a secure location.";

            return $"Complete the task: {title}.";
        }

        // ─────────────────────────────────────────────────────
        // Help Message
        // ─────────────────────────────────────────────────────

        private string GetHelpMessage()
        {
            var keys = _keywords.GetAllKeywords();
            string topicList = string.Join(" • ", keys);
            return $" Here's what I can help you with, {_memory.UserName}:\n\n" +
                   $" Cybersecurity Topics:\n{topicList}\n\n" +
                   " Task Assistant:\n• 'Add a task to enable 2FA'\n• 'Remind me to update my password tomorrow'\n\n" +
                   " Quiz:\n• 'Start quiz' or 'quiz me'\n\n" +
                   " Activity Log:\n• 'Show activity log' or 'what have you done for me?'\n\n" +
                   " You can also ask:\n• How are you?\n• What is your purpose?\n• Tell me more (continues last topic)";
        }
    }
}
