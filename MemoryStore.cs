using System;
using System.Collections.Generic;
using System.Text;

namespace THEPART2
{
    /// <summary>
    /// Stores user details shared during the conversation
    /// and provides personalised responses using that information.
    /// </summary>
    public class MemoryStore
    {
        public string UserName { get; set; } = string.Empty;
        public string FavouriteTopic { get; set; } = string.Empty;

        private readonly Dictionary<string, string> _memory = new();

        /// <summary>Saves any key-value pair to memory.</summary>
        public void Store(string key, string value)
        {
            _memory[key.ToLower()] = value;
        }

        /// <summary>Retrieves a stored value by key. Returns empty string if not found.</summary>
        public string Recall(string key)
        {
            return _memory.TryGetValue(key.ToLower(), out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// Builds a personalised opening line using whatever the bot currently knows.
        /// </summary>
        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(FavouriteTopic))
                return $"As someone interested in {FavouriteTopic}, {UserName}, here's something useful for you:";

            if (!string.IsNullOrEmpty(UserName))
                return $"Good point, {UserName}!";

            return string.Empty;
        }

        /// <summary>
        /// Detects and stores a favourite topic from the user's message if they express interest.
        /// Returns true if a topic was stored.
        /// </summary>
        public bool TryStoreFavouriteTopic(string input)
        {
            string lower = input.ToLower();
            string[] interestPhrases = { "interested in", "i like", "i love", "care about", "worried about", "want to learn about" };
            string[] topics = { "password", "phishing", "privacy", "malware", "vpn", "firewall",
                                 "two-factor", "2fa", "social engineering", "scam", "encryption",
                                 "ransomware", "data breach", "cybersecurity", "browsing" };

            foreach (string phrase in interestPhrases)
            {
                if (lower.Contains(phrase))
                {
                    foreach (string topic in topics)
                    {
                        if (lower.Contains(topic))
                        {
                            FavouriteTopic = topic;
                            Store("favourite_topic", topic);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasName => !string.IsNullOrEmpty(UserName);
        public bool HasFavouriteTopic => !string.IsNullOrEmpty(FavouriteTopic);
    }
}
