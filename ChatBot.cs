using PART2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PART2
{
    /// <summary>
    /// Central chatbot class. MainWindow only calls ProcessInput() on this class.
    /// All routing logic — sentiment, keywords, memory, follow-up — lives here.
    /// </summary>
    public class ChatBot
    {
        private readonly PART2.KeywordResponder _keywords;
        private readonly PART2.SentimentDetector _sentiment;
        private readonly MemoryStore _memory;

        private bool _awaitingName = true;
        private string _lastTopic = string.Empty;

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

        public ChatBot()
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
        }

        /// <summary>Returns the opening greeting message shown on app launch.</summary>
        public string GetGreeting()
        {
            return " Hello! Welcome to the Mr West Cybersecurity Bot!\n\nI'm here to help you stay safe online. What's your name?";
        }

        /// <summary>
        /// Main routing method. Takes raw user input and returns the bot's response.
        /// Order of checks follows the brief exactly.
        /// </summary>
        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return " Please type something — I'm here to help!";

            string trimmed = input.Trim();

            // Step 1: Capture name on first message
            if (_awaitingName)
            {
                _memory.UserName = trimmed;
                _memory.Store("name", trimmed);
                _awaitingName = false;
                return $"Nice to meet you, {_memory.UserName}! \n\nI can help you with cybersecurity topics like passwords, phishing, malware, privacy, scams, VPN, firewall, 2FA, encryption, ransomware, browsing, and more.\n\nType 'help' to see all topics, or just ask me anything!";
            }

            string lower = trimmed.ToLower();

            // Help command
            if (lower == "help" || lower == "topics" || lower == "menu")
                return GetHelpMessage();

            // How are you
            if (lower.Contains("how are you") || lower.Contains("how r u"))
                return $"I'm running perfectly and ready to keep you safe, {_memory.UserName}!  What cybersecurity topic can I help you with today?";

            // Purpose
            if (lower.Contains("what is your purpose") || lower.Contains("what do you do") || lower.Contains("your purpose"))
                return $"My purpose is to raise your cybersecurity awareness, {_memory.UserName}! I'll teach you how to protect yourself from threats like phishing, malware, weak passwords, scams, and more. Just ask me anything!";

            // Who are you
            if (lower.Contains("your name") || lower.Contains("who are you"))
                return "I'm the Mr West Cybersecurity Bot — built by Uviwe Booi to keep YOU safe online! ";

            // Greetings
            if (lower == "hello" || lower == "hi" || lower == "hey" || lower.StartsWith("hi ") || lower.StartsWith("hello "))
                return $"Hey {_memory.UserName}! What cybersecurity topic would you like to explore today?";

            // Step 2: Follow-up handling
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

            // Step 3: Detect and store favourite topic from interest statements
            _memory.TryStoreFavouriteTopic(trimmed);

            // Step 4: Sentiment detection
            Sentiment detected = _sentiment.Detect(lower);
            string sentimentOpener = _sentiment.GetSentimentResponse(detected);

            // Step 5: Keyword recognition
            string? keywordResponse = _keywords.GetResponse(lower);
            string? matchedKeyword = _keywords.GetMatchedKeyword(lower);

            if (keywordResponse != null)
            {
                _lastTopic = matchedKeyword ?? string.Empty;

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

            // Step 6: Fallback
            string name = _memory.HasName ? $", {_memory.UserName}" : string.Empty;
            string fallback = _fallbackResponses[_random.Next(_fallbackResponses.Count)];
            return $"I'm not sure I understood that{name}. {fallback}";
        }

        private string GetHelpMessage()
        {
            var keys = _keywords.GetAllKeywords();
            string topicList = string.Join(" • ", keys);
            return $" Here's what I can help you with, {_memory.UserName}:\n\n Topics:\n{topicList}\n\n You can also ask:\n• How are you today?\n• What is your purpose?\n• Tell me more (continues last topic)\n\nType 'exit' to close the chatbot.";
        }
    }
}