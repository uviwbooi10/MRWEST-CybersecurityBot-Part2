using System;
using System.Collections.Generic;
using System.Text;
using THEPART2;

namespace THEPART2
{
    /// <summary>
    /// Manages the cybersecurity quiz: question flow, scoring, and feedback.
    /// Questions cover phishing, password safety, safe browsing, social
    /// engineering, 2FA, malware/ransomware, privacy, and data backup.
    /// </summary>
    public class QuizManager
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;

        public QuizManager()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectAnswer = "C",
                    Explanation = "Correct! Reporting phishing emails helps prevent scams and alerts your provider.",
                    IsTrueFalse = false,
                    Topic = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "Which of these is a common sign of a phishing email?",
                    Options = new List<string> { "A) Perfect grammar", "B) Urgent threatening language", "C) Sent from a known colleague", "D) No links included" },
                    CorrectAnswer = "B",
                    Explanation = "Phishing emails often create false urgency to pressure you into acting without thinking.",
                    IsTrueFalse = false,
                    Topic = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "True or False: A strong password should include uppercase, lowercase, numbers, and symbols.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Correct! Mixing character types makes passwords much harder to crack.",
                    IsTrueFalse = true,
                    Topic = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "Which of these is the safest password practice?",
                    Options = new List<string> { "A) Reusing one password everywhere", "B) Using a password manager", "C) Writing passwords on a sticky note", "D) Using your birthday" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! Password managers generate and store unique strong passwords for every account.",
                    IsTrueFalse = false,
                    Topic = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "Which symbol in a browser address bar indicates a secure HTTPS connection?",
                    Options = new List<string> { "A) A red flag", "B) A padlock icon", "C) A question mark", "D) A star" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! The padlock icon confirms the connection is encrypted via HTTPS.",
                    IsTrueFalse = false,
                    Topic = "Safe Browsing"
                },
                new QuizQuestion
                {
                    Question = "True or False: It's safe to do online banking on public Wi-Fi without a VPN.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Correct! Public Wi-Fi is often unsecured, making it easy for attackers to intercept your data.",
                    IsTrueFalse = true,
                    Topic = "Safe Browsing"
                },
                new QuizQuestion
                {
                    Question = "True or False: Social engineering relies on manipulating people rather than hacking systems directly.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Correct! Social engineering exploits human trust and psychology, not just technical flaws.",
                    IsTrueFalse = true,
                    Topic = "Social Engineering"
                },
                new QuizQuestion
                {
                    Question = "A caller claims to be IT support and asks for your password to 'fix an issue'. What should you do?",
                    Options = new List<string> { "A) Give them the password", "B) Hang up and verify through official channels", "C) Ask them to call back later", "D) Give a fake password" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! Legitimate IT support never needs your actual password. Always verify independently.",
                    IsTrueFalse = false,
                    Topic = "Social Engineering"
                },
                new QuizQuestion
                {
                    Question = "What does Two-Factor Authentication (2FA) add to the login process?",
                    Options = new List<string> { "A) A second password", "B) A second independent verification step", "C) A longer wait time", "D) Nothing extra" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! 2FA adds a second layer like a code or biometric scan, even if your password is stolen.",
                    IsTrueFalse = false,
                    Topic = "Two-Factor Authentication"
                },
                new QuizQuestion
                {
                    Question = "True or False: Ransomware encrypts your files and demands payment to unlock them.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Correct! Ransomware locks your files. Regular backups are the best defence, not paying the ransom.",
                    IsTrueFalse = true,
                    Topic = "Malware and Ransomware"
                },
                new QuizQuestion
                {
                    Question = "Which of these is the best way to protect against malware?",
                    Options = new List<string> { "A) Disabling antivirus to speed up your PC", "B) Downloading from unofficial sites", "C) Keeping antivirus software updated", "D) Opening all email attachments" },
                    CorrectAnswer = "C",
                    Explanation = "Correct! Updated antivirus software detects and blocks the latest malware threats.",
                    IsTrueFalse = false,
                    Topic = "Malware and Ransomware"
                },
                new QuizQuestion
                {
                    Question = "Why should you regularly review your social media privacy settings?",
                    Options = new List<string> { "A) To get more followers", "B) To limit who can see your personal information", "C) It's not necessary", "D) To change your profile picture" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! Limiting visibility of personal info reduces your risk of being targeted by attackers.",
                    IsTrueFalse = false,
                    Topic = "Privacy Settings"
                },
                new QuizQuestion
                {
                    Question = "True or False: A flashlight app needing access to your contacts list is a normal permission request.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Correct! Apps should only request permissions relevant to their function. Unusual requests are a red flag.",
                    IsTrueFalse = true,
                    Topic = "Privacy Settings"
                },
                new QuizQuestion
                {
                    Question = "What is the best defence against losing data to a ransomware attack?",
                    Options = new List<string> { "A) Paying the ransom quickly", "B) Regular offline backups", "C) Ignoring antivirus warnings", "D) Disconnecting your antivirus" },
                    CorrectAnswer = "B",
                    Explanation = "Correct! Regular offline backups let you restore your files without ever paying attackers.",
                    IsTrueFalse = false,
                    Topic = "Data Backup"
                }
            };
        }

        /// <summary>Returns the current question, or null if the quiz is finished.</summary>
        public QuizQuestion? GetCurrentQuestion()
        {
            return IsFinished() ? null : _questions[_currentIndex];
        }

        /// <summary>
        /// Submits the user's answer, scores it, and advances to the next question.
        /// Returns true if the answer was correct.
        /// </summary>
        public bool SubmitAnswer(string answer)
        {
            var current = GetCurrentQuestion();
            if (current == null) return false;

            bool isCorrect = string.Equals(answer.Trim(), current.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
            if (isCorrect) _score++;

            _currentIndex++;
            return isCorrect;
        }

        /// <summary>Returns the explanation text for the question just answered.</summary>
        public string GetFeedback(bool correct, QuizQuestion question)
        {
            string prefix = correct ? " Correct! " : $" Incorrect. The correct answer was {question.CorrectAnswer}. ";
            return prefix + question.Explanation;
        }

        public bool IsFinished() => _currentIndex >= _questions.Count;

        public int GetCurrentQuestionNumber() => Math.Min(_currentIndex + 1, _questions.Count);

        public int GetTotalQuestions() => _questions.Count;

        public string GetFinalScore() => $"{_score} out of {_questions.Count}";

        public int GetScoreValue() => _score;

        /// <summary>Returns a motivational message based on the final score percentage.</summary>
        public string GetFinalMessage()
        {
            double percentage = (double)_score / _questions.Count * 100;

            if (percentage >= 80)
                return "Excellent work! You really know your cybersecurity basics!";
            if (percentage >= 60)
                return "Great job! You have a solid grasp of cybersecurity — keep learning!";
            if (percentage >= 40)
                return "Good effort! Review some topics and try again to boost your score.";

            return "Keep learning! Cybersecurity awareness takes practice — try the quiz again soon.";
        }

        /// <summary>Resets the quiz to start from the beginning.</summary>
        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
        }
    }
}
