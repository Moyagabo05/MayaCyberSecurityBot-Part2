using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayaCyberSecurityBot2
{
        

    
        //  DELEGATE DEFINITION (Required by rubric: "Use delegates to solve a programming problem")
        // Dynamically formats responses based on sentiment, memory, and user context
        public delegate string ResponseFormatter(string baseResponse, string sentiment, string userName, Dictionary<string, string> memory);

    public class ChatService
    {
        public string UserName { get; set; } = "User";
        private readonly Dictionary<string, string> _definitions;
        private readonly Dictionary<string, string> _userMemory;
        private string _lastTopic = string.Empty;
        private readonly List<string> _conversationHistory;
        private static readonly Random _random = new Random();
        private readonly ResponseFormatter _formatResponse;

        public ChatService(Dictionary<string, string> definitions, ResponseFormatter formatter)
        {
            _definitions = definitions;
            _userMemory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _conversationHistory = new List<string>();
            _formatResponse = formatter;
        }

        public string GetResponse(string userInput)
        {
            try
            {
                string input = userInput.ToLower().Trim();
                _conversationHistory.Add(userInput);
                if (_conversationHistory.Count > 10) _conversationHistory.RemoveAt(0);

                if (input == "help" || input == "menu")
                    return GetHelpMenu();


                // 1️⃣ MEMORY: Save Name
                if (input.Contains("my name is") || input.Contains("call me"))
                {
                    string name = input.Split(new[] { "my name is", "call me" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        UserName = name;
                        _userMemory["name"] = name;
                        return $"Nice to meet you, {UserName}! I'll remember that.";
                    }
                }

                // 2️⃣ MEMORY: Save Interest
                if (input.Contains("interested in") || input.Contains("want to learn about"))
                {
                    string topic = input.Split(new[] { "interested in", "want to learn about" }, StringSplitOptions.RemoveEmptyEntries)
                                        .LastOrDefault()?.Trim();
                    if (!string.IsNullOrWhiteSpace(topic))
                    {
                        _userMemory["interest"] = topic;
                        return $"Great! I've noted you're interested in {topic}. I'll tailor tips accordingly.";
                    }
                }

                // 3️⃣ SENTIMENT DETECTION
                string sentiment = DetectSentiment(input);

                // 4️⃣ CONVERSATION FLOW: Follow-ups
                if (IsFollowUp(input) && !string.IsNullOrEmpty(_lastTopic))
                {
                    string followUp = GetFollowUpContent(_lastTopic);
                    return _formatResponse(followUp, sentiment, UserName, _userMemory);
                }

                // 5️⃣ KEYWORD RECOGNITION (Whole-word, prioritizes longer matches)
                var matchedKeyword = _definitions.Keys
                    .OrderByDescending(k => k.Length)
                    .FirstOrDefault(k => $" {input} ".Contains($" {k} "));

                if (matchedKeyword != null)
                {
                    _lastTopic = matchedKeyword;
                    string rawResponse = _definitions[matchedKeyword];
                    string selected = GetRandomResponse(rawResponse);
                    // Inside ChatService.GetResponse(), RIGHT BEFORE the final return statement:
                    string prompt = "\n\n Type \"read more\", \"explain\", or \"tell me more\" to continue on this topic.";
                    return _formatResponse(selected + prompt, sentiment, UserName, _userMemory);
                    
                }

                // 6️⃣ SMALL TALK & COMMANDS
                if (new[] { "hello", "hi", "hey" }.Any(input.Contains))
                    return $"Hi {UserName}! Type 'help' to see cybersecurity topics.";
                if (input == "how are you")
                    return $"I'm running smoothly and ready to help, {UserName}!";
                if (input.Contains("help") || input.Contains("menu"))
                    return GetHelpMenu();

                // 7️⃣ FALLBACK / ERROR HANDLING
                return $"I didn't quite understand that, {UserName}. Could you rephrase or try a keyword like 'password', 'phishing', or 'privacy'? Type 'help' for topics.";
            }
            catch (Exception ex)
            {
                return $"Oops! Something went wrong: {ex.Message}. Please try again.";
            }
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious") || input.Contains("afraid"))
                return "empathetic";
            if (input.Contains("curious") || input.Contains("interested") || input.Contains("excited"))
                return "enthusiastic";
            if (input.Contains("confused") || input.Contains("don't understand") || input.Contains("frustrated"))
                return "supportive";
            return "neutral";
        }

        private bool IsFollowUp(string input)
        {
            string[] phrases = {
            "read more", "tell me more", "more information", "explain", "explain more",
            "go on", "continue", "what else", "another tip", "more details",
            "elaborate", "give me an example", "how does that work", "keep going"
            };
            return phrases.Any(p => input.Contains(p));
        }

        private string GetFollowUpContent(string topic)
        {
            // Topic-specific follow-ups that actually continue the conversation
            var topicFollowUps = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        { "password", new[] {
            "Since you asked about passwords: Never reuse passwords across sites. If one platform gets breached, attackers will try that password everywhere (credential stuffing).",
            "Another password tip: Use a password manager like Bitwarden or KeePass. They generate and store complex passwords so you only need to remember one master password.",
            "Expand on password safety: Change your password immediately if a service announces a breach. Don't wait for it to affect you."
        }},
        { "strong password", new[] {
            "To strengthen passwords further: Avoid dictionary words, personal info (birthdays, pet names), or predictable patterns like '123456'.",
            "A strong password tip: Use a passphrase with random words + symbols, e.g., 'Coffee!Blue#Sky@7AM' is stronger AND easier to remember.",
            "Password length matters most: A 15-character password with mixed case is exponentially harder to crack than a short complex one."
        }},
        { "2fa", new[] {
            "Continuing with 2FA: Even if your password is stolen, 2FA blocks 99.9% of automated account hacks. Enable it everywhere possible.",
            "2FA tip: Prefer authenticator apps (Google Authenticator, Authy) over SMS codes. SMS can be intercepted via SIM-swapping attacks.",
            "Backup your 2FA codes! If you lose your phone, backup codes are the only way to recover your account."
        }},
        { "phishing", new[] {
            "Continuing with phishing: Always verify the sender's email domain carefully. Scammers use lookalike domains (e.g., support@amaz0n.com) to trick you.",
            "Another phishing tip: Hover over links before clicking to see the actual URL. If it's shortened or suspicious, navigate directly to the official website.",
            "Phishing isn't just email. Smishing (SMS) and vishing (voice calls) use the same psychological tricks: urgency, fear, and fake authority."
        }},
        { "spot phishing", new[] {
            "Red flags for phishing: Generic greetings ('Dear Customer'), urgent threats ('Act now or lose access!'), and requests for passwords via email.",
            "Check the URL carefully: Legitimate sites use HTTPS (padlock icon). If you see 'http://' or a misspelled domain, don't proceed.",
            "When in doubt, don't click. Open a new browser tab and type the official website address directly."
        }},
        { "scam", new[] {
            "Scam warning: If someone you've never met asks for money, gift cards, or crypto, it's a scam. Real relationships don't start with financial requests.",
            "Tech support scams: Microsoft/Apple will NEVER call you unsolicited. If you get such a call, hang up and verify independently.",
            "Investment scams: If it promises guaranteed high returns with 'no risk', it's almost certainly a scam. Research thoroughly before committing money."
        }},
        { "privacy", new[] {
            "Regarding privacy: Review app permissions on your phone. Does a calculator app really need contacts and location? If not, revoke it.",
            "Privacy expands beyond browsers: Use private/incognito mode for sensitive searches, and regularly clear cookies or use tracker blockers.",
            "In South Africa, remember your POPIA rights. You can request companies to show, correct, or delete your personal data."
        }},
        { "popia", new[] {
            "POPIA tip: You have the right to request a copy of your personal data from any organisation. They must respond within a reasonable timeframe.",
            "If a company mishandles your data under POPIA, you can lodge a complaint with the Information Regulator of South Africa.",
            "POPIA requires organisations to collect data lawfully and securely. If you're unsure how your data is used, ask for their privacy policy."
        }},
        { "malware", new[] {
            "Expanding on malware protection: Enable automatic OS and app updates. Most malware exploits known vulnerabilities that patches already fix.",
            "Another malware tip: Avoid pirated software, cracks, or 'free' premium tools. They're the #1 delivery method for trojans and ransomware.",
            "Malware signs to watch for: sudden slowdowns, unknown browser extensions, or files you didn't create. Run a full antivirus scan immediately."
        }},
        { "ransomware", new[] {
            "Ransomware defense: Follow the 3-2-1 backup rule: 3 copies, 2 different media types, 1 stored offline/offsite. Test restores quarterly.",
            "Never pay ransom: Paying doesn't guarantee file recovery and funds criminal activity. Prevention (backups + updates) is your best defense.",
            "Ransomware often spreads via email attachments. Never open attachments from unknown senders, even if they look legitimate."
        }},
        { "vpn", new[] {
            "Regarding VPNs: Free VPNs often log and sell your browsing data. Choose reputable providers with verified no-logs policies and independent audits.",
            "VPNs don't make you anonymous: They encrypt traffic and hide your IP, but websites can still track you via cookies and account logins.",
            "Always enable your VPN before connecting to public WiFi. Without it, your traffic is broadcast openly to anyone on the same network."
        }},
        { "public wifi", new[] {
            "Public WiFi safety: Avoid banking or email on public networks. If you must, use a trusted VPN and ensure sites use HTTPS (padlock icon).",
            "Disable auto-connect on your devices. Attackers can set up fake WiFi hotspots with names like 'Free Airport WiFi' to intercept traffic.",
            "Turn off file sharing on public networks. This prevents other users on the same WiFi from accessing your device."
        }},
        { "backup data", new[] {
            "Backup tip: Test your backups! A backup you can't restore is no backup at all. Practice restoring a file quarterly.",
            "Use the 3-2-1 rule: 3 copies of data, 2 different storage types (e.g., cloud + external drive), 1 stored offline/offsite.",
            "Automate backups where possible. Manual backups are often forgotten. Set calendar reminders if automation isn't available."
        }},
        { "encryption", new[] {
            "Encryption tip: Enable full-disk encryption (BitLocker on Windows, FileVault on Mac). It protects your data if your device is lost or stolen.",
            "Encrypted messaging: Use Signal or WhatsApp for sensitive conversations. They use end-to-end encryption, unlike standard SMS.",
            "Password managers encrypt your vault locally. Even if their servers are breached, your passwords remain secure with a strong master password."
        }},
        { "safe browsing", new[] {
            "Safe browsing tip: Stick to HTTPS sites. If a site shows 'Not Secure' in your browser, don't enter passwords or payment details.",
            "Avoid clicking pop-ups or banner ads. Malvertising (malicious ads) can install malware without you clicking anything.",
            "Use an ad-blocker like uBlock Origin. It blocks annoying ads AND malicious ad networks that deliver malware."
        }}
    };

            // If we have specific follow-ups for this topic, pick one randomly
            if (topicFollowUps.TryGetValue(topic, out var followUps))
            {
                return followUps[_random.Next(followUps.Length)];
            }

            // Generic but context-aware fallback for topics without specific follow-ups
            string interest = _userMemory.TryGetValue("interest", out var i) ? i ?? "" : "";
            string prefix = !string.IsNullOrEmpty(interest) && topic.Contains(interest)
                ? $"Since you're focused on {interest}, here's more: "
                : "Here's some additional info: ";

            return $"{prefix}Always verify information through official channels and enable multi-layered security (updates + backups + strong auth). Cybersecurity is about consistent habits, not one-time fixes.";
        }

        private string GetRandomResponse(string text)
        {
            if (text.Contains("|"))
            {
                string[] options = text.Split('|');
                return options[_random.Next(options.Length)].Trim();
            }
            return text;
        }

        private string GetHelpMenu()
        {
            return "AVAILABLE TOPICS:\n" +
                   "• Passwords & 2FA\n• Phishing & Scams\n• Malware & Ransomware\n" +
                   "• VPNs & Public WiFi\n• Privacy & POPIA\n• Safe Browsing\n\n" +
                   "Just type a keyword (e.g., 'password') or ask a question!";
        }

        public string? Recall(string key) => _userMemory.TryGetValue(key, out var val) ? val : null;
    
    }
    


}
