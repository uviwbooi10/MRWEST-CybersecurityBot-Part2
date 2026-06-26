using System;
using System.Collections.Generic;
using System.Text;

namespace THEPART2
{
    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }

    /// <summary>
    /// Detects the emotional tone of the user's message and
    /// returns an empathetic opening before the cybersecurity tip.
    /// </summary>
    public class SentimentDetector
    {
        private readonly Dictionary<Sentiment, List<string>> _triggers = new()
        {
            {
                Sentiment.Worried, new List<string>
                {
                    "worried", "scared", "afraid", "anxious", "nervous", "unsafe",
                    "frightened", "concerned", "panic", "terrified", "unsure", "stressed"
                }
            },
            {
                Sentiment.Curious, new List<string>
                {
                    "curious", "wondering", "interested", "want to know", "how does",
                    "can you explain", "tell me more", "what is", "what are", "why does",
                    "how do", "i'd like to know", "could you tell"
                }
            },
            {
                Sentiment.Frustrated, new List<string>
                {
                    "frustrated", "annoyed", "confused", "don't understand", "cant understand",
                    "makes no sense", "difficult", "hard to", "complicated", "stuck",
                    "this is useless", "not working", "fed up"
                }
            },
            {
                Sentiment.Happy, new List<string>
                {
                    "great", "thanks", "helpful", "awesome", "love it", "amazing",
                    "fantastic", "brilliant", "excellent", "perfect", "thank you",
                    "this is good", "nice", "cool"
                }
            }
        };

        private readonly Dictionary<Sentiment, string> _responses = new()
        {
            {
                Sentiment.Worried,
                "It's completely understandable to feel that way. You're not alone — many people worry about online safety. Let me share some tips to help you feel more secure:"
            },
            {
                Sentiment.Curious,
                "Great question! I love the curiosity — that's exactly the mindset that keeps you safe online. Here's what you need to know:"
            },
            {
                Sentiment.Frustrated,
                "I hear you — cybersecurity can feel overwhelming at first. Let me break this down as simply as possible for you:"
            },
            {
                Sentiment.Happy,
                "Glad to hear that! Staying informed is the best defence. Here's some more useful info:"
            },
            {
                Sentiment.Neutral,
                string.Empty
            }
        };

        /// <summary>
        /// Analyses the input and returns the detected sentiment.
        /// </summary>
        public Sentiment Detect(string input)
        {
            string lower = input.ToLower();
            foreach (var pair in _triggers)
            {
                foreach (string trigger in pair.Value)
                {
                    if (lower.Contains(trigger))
                        return pair.Key;
                }
            }
            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns the empathetic opening sentence for the detected sentiment.
        /// Returns empty string for Neutral.
        /// </summary>
        public string GetSentimentResponse(Sentiment sentiment)
        {
            return _responses.TryGetValue(sentiment, out string? response) ? response : string.Empty;
        }
    }
}
