using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace THEPART2
{
    /// <summary>
    /// Manages cybersecurity keyword recognition.
    /// Each keyword maps to a list of responses — one is picked randomly
    /// to keep the conversation varied and engaging. Also normalises
    /// common synonyms so varied phrasing still matches (NLP support).
    /// </summary>
    public class KeywordResponder
    {
        private readonly Dictionary<string, List<string>> _responses;
        private readonly Random _random = new();

        private readonly Dictionary<string, string> _synonyms = new()
        {
            ["pw"] = "password",
            ["passcode"] = "password",
            ["pass word"] = "password",
            ["phish"] = "phishing",
            ["scammed"] = "scam",
            ["scamming"] = "scam",
            ["virus"] = "malware",
            ["trojan"] = "malware",
            ["spyware"] = "malware",
            ["2fa"] = "two-factor",
            ["mfa"] = "two-factor",
            ["multi-factor"] = "two-factor",
            ["personal info"] = "privacy",
            ["personal data"] = "privacy",
            ["leaked"] = "data breach",
            ["hacked"] = "data breach",
            ["breach"] = "data breach",
            ["encrypted"] = "encryption",
            ["lock my files"] = "ransomware",
            ["safe browsing"] = "browsing",
            ["public wifi"] = "browsing"
        };

        public KeywordResponder()
        {
            _responses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "Use at least 12 characters mixing uppercase, lowercase, numbers and symbols. Never reuse passwords across sites!",
                    "A strong password is like a strong lock — make it long, random and unique. Consider using a password manager like Bitwarden.",
                    "Never use personal info like your birthday or name in passwords. Attackers guess these first!",
                    "Change your passwords regularly, especially after any data breach notification.",
                    "A passphrase like 'BlueSky$Rain99!' is both memorable and very hard to crack."
                },

                ["phishing"] = new List<string>
                {
                    "Never click links in suspicious emails. Always verify the sender's address — scammers use typos like 'paypa1.com'.",
                    "Phishing emails often create urgency like 'Your account will be closed!' — slow down and verify before clicking.",
                    "Legitimate organisations will NEVER ask for your password via email or SMS.",
                    "When in doubt, go directly to the website by typing the URL yourself instead of clicking any link.",
                    "Look out for poor grammar and spelling in emails — these are common signs of phishing attempts."
                },

                ["malware"] = new List<string>
                {
                    "Keep your antivirus software updated and run regular scans. Don't ignore those update notifications!",
                    "Never download software from unofficial or unknown sources — only use trusted platforms.",
                    "Malware often hides in email attachments. Be very cautious about opening files you weren't expecting.",
                    "Ransomware can encrypt all your files. Regular backups to an external drive are your best protection.",
                    "If your device suddenly slows down or shows strange pop-ups, scan it for malware immediately."
                },

                ["privacy"] = new List<string>
                {
                    "Review your social media privacy settings regularly — limit who can see your posts and personal info.",
                    "Be careful what you share online. Even seemingly harmless info can be used by attackers to target you.",
                    "Use incognito mode and clear cookies when browsing on shared or public computers.",
                    "Read app permissions carefully — a flashlight app doesn't need access to your contacts or location!",
                    "Use encrypted messaging apps like Signal for sensitive conversations."
                },

                ["scam"] = new List<string>
                {
                    "If something sounds too good to be true — a free iPhone, lottery win, or prize — it's almost certainly a scam.",
                    "Never send money or gift cards to someone you haven't met in person, no matter how convincing they seem.",
                    "Scammers often impersonate banks, SARS, or government departments. Call the official number to verify.",
                    "Romance scams are rising in South Africa. Be cautious about people you meet online who quickly ask for money.",
                    "SABRIC (South African Banking Risk Information Centre) reports thousands of scam cases yearly. Stay alert!"
                },

                ["vpn"] = new List<string>
                {
                    "A VPN encrypts your internet traffic so your ISP and others can't see what you're doing online.",
                    "Always use a trusted VPN on public Wi-Fi — coffee shops and airports are prime spots for attackers.",
                    "Choose a reputable paid VPN — free VPNs often sell your data to third parties.",
                    "A VPN masks your IP address, giving you more anonymity and protecting your online identity.",
                    "NordVPN, ExpressVPN, and ProtonVPN are among the most trusted options available."
                },

                ["firewall"] = new List<string>
                {
                    "A firewall monitors and controls traffic between your device and the internet — keep it enabled always.",
                    "Your router acts as a hardware firewall. Make sure its firmware is updated regularly.",
                    "Windows Defender Firewall is built into Windows — check that it's turned on in your security settings.",
                    "Firewalls block unauthorised access attempts — they're your first line of defence against intrusions.",
                    "For businesses, next-generation firewalls (NGFW) provide deeper inspection of network traffic."
                },

                ["two-factor"] = new List<string>
                {
                    "Two-factor authentication (2FA) adds a second verification step — even if your password is stolen, attackers can't get in.",
                    "Use an authenticator app like Google Authenticator or Authy instead of SMS-based 2FA where possible.",
                    "Enable 2FA on all important accounts: email, banking, social media and cloud storage.",
                    "Biometric 2FA like fingerprint or face recognition is both convenient and highly secure.",
                    "Never share your 2FA codes with anyone — not even someone claiming to be from your bank."
                },

                ["social engineering"] = new List<string>
                {
                    "Social engineering manipulates people rather than systems. Always verify identities before sharing any info.",
                    "Attackers may call pretending to be IT support and ask for your password. Legitimate IT staff never do this.",
                    "Pretexting is when scammers create fake scenarios to extract info — be sceptical of unusual requests.",
                    "Tailgating is when someone follows you into a secure building. Always challenge unknown people in secure areas.",
                    "Your biggest cybersecurity risk is human error — slow down, think critically, and verify everything."
                },

                ["encryption"] = new List<string>
                {
                    "Encryption converts your data into unreadable code — only someone with the right key can decode it.",
                    "Always check for HTTPS and the padlock icon before entering personal or payment information on a website.",
                    "Full disk encryption (like BitLocker on Windows) protects your data even if your laptop is stolen.",
                    "End-to-end encryption in apps like WhatsApp means only you and the recipient can read messages.",
                    "Encrypted backups protect your data even if the backup service itself gets breached."
                },

                ["data breach"] = new List<string>
                {
                    "A data breach is when unauthorised people access private information. Check haveibeenpwned.com to see if your email was exposed.",
                    "The Experian South Africa breach in 2020 exposed data of 24 million South Africans — a reminder to monitor your credit.",
                    "If your data is breached, change your passwords immediately and enable 2FA on affected accounts.",
                    "Monitor your bank statements regularly for unusual transactions after any breach notification.",
                    "POPIA (Protection of Personal Information Act) requires South African companies to notify you of data breaches."
                },

                ["ransomware"] = new List<string>
                {
                    "Ransomware encrypts all your files and demands payment to restore them. Never pay — there's no guarantee you'll get your data back.",
                    "Regular offline backups are the best defence against ransomware. If you're backed up, you can restore without paying.",
                    "Ransomware usually enters via phishing emails or infected downloads. Think before you click!",
                    "Keep your OS and software updated — ransomware often exploits known vulnerabilities in outdated software.",
                    "South African organisations have seen a surge in ransomware attacks. Always have an incident response plan."
                },

                ["browsing"] = new List<string>
                {
                    "Always verify HTTPS and the padlock icon before submitting any personal or financial information.",
                    "Install an ad blocker like uBlock Origin — malicious ads (malvertising) can infect your device without clicking.",
                    "Clear your browser history and cookies regularly to reduce tracking and exposure.",
                    "Avoid using public Wi-Fi for banking or any sensitive activities without a VPN.",
                    "Keep your browser updated — outdated browsers have known security vulnerabilities that attackers exploit."
                }
            };
        }

        /// <summary>Normalises common synonyms to their matching keyword before lookup.</summary>
        private string NormaliseSynonyms(string input)
        {
            string result = input;
            foreach (var pair in _synonyms)
            {
                if (result.Contains(pair.Key))
                    result = result.Replace(pair.Key, pair.Value);
            }
            return result;
        }

        /// <summary>
        /// Checks if the user input contains any known keyword (after synonym
        /// normalisation). Returns a randomly selected response from that
        /// keyword's list. Returns null if no keyword is found.
        /// </summary>
        public string? GetResponse(string input)
        {
            string lower = NormaliseSynonyms(input.ToLower());
            foreach (var pair in _responses)
            {
                if (lower.Contains(pair.Key))
                {
                    int index = _random.Next(pair.Value.Count);
                    return pair.Value[index];
                }
            }
            return null;
        }

        /// <summary>Returns the matched keyword from the input (after synonym normalisation), or null.</summary>
        public string? GetMatchedKeyword(string input)
        {
            string lower = NormaliseSynonyms(input.ToLower());
            foreach (string key in _responses.Keys)
            {
                if (lower.Contains(key))
                    return key;
            }
            return null;
        }

        /// <summary>Returns all available keywords for the help display.</summary>
        public List<string> GetAllKeywords() => _responses.Keys.ToList();

        /// <summary>Gets another random response for the given keyword (for 'tell me more').</summary>
        public string? GetAnotherResponse(string keyword)
        {
            if (_responses.TryGetValue(keyword.ToLower(), out List<string>? list))
            {
                int index = _random.Next(list.Count);
                return list[index];
            }
            return null;
        }
    }
}
