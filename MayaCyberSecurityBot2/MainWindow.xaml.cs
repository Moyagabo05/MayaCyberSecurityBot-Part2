using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MayaCyberSecurityBot2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatService _chatService= null;
        private ObservableCollection<ChatMessage> _messages = null!;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChat();
        }
        private void InitializeChat()
        {
            // 1. Display ASCII Art
            AsciiArtText.Text = AsciiArt.GetLogoForGui();

            // 2. Setup chat collection
            _messages = new ObservableCollection<ChatMessage>();
            ChatMessages.ItemsSource = _messages;

            // 3. Load definitions
            var definitions = CreateDefinitionsDictionary();

            // CREATE THE DELEGATE INSTANCE 
            ResponseFormatter formatResponse = (baseResponse, sentiment, userName, memory) =>
            {
                string prefix = sentiment switch
                {
                    "empathetic" => "It's completely understandable to feel that way. ",
                    "enthusiastic" => "That's a great question! ",
                    "supportive" => "No worries, let me break it down simply: ",
                    _ => ""
                };

                string personalization = memory.TryGetValue("interest", out var interest)
                    ? $" As someone interested in {interest}, {userName}: "
                    : $" {userName}: ";

                return $"{prefix}{personalization}{baseResponse}";
            };

            // PASS BOTH PARAMETERS to ChatService
            _chatService = new ChatService(definitions, formatResponse);

            // 4. Play greeting asynchronously
            _ = AudioPlayer.PlayGreetingAsync();

            // 5. Initial message
            AddBotMessage("I will be assisting you today. I'll give you tips to stay safe online.\n\nPlease enter your name to begin:");
            UserInput.Focus();
        }

        private Dictionary<string, string> CreateDefinitionsDictionary()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // PASSWORDS & AUTHENTICATION (10)
        { "password", "[DEFINITION] A password is a secret string of characters used to verify your identity when logging into an account or system.|" +
                      "[TIPS] Use at least 12 characters. Mix uppercase, lowercase, numbers & symbols. Avoid personal info or common words.|" +
                      "Think of your password like a toothbrush: don't share it, change it regularly, and keep it strong!" },

        { "strong password", "[TIPS] Use at least 12 characters. Mix uppercase, lowercase, numbers & symbols. Avoid personal info.|" +
                             "A strong password is like a fortress wall: the thicker and more complex, the harder it is to break through.|" +
                             "Try a passphrase: 'Coffee!Blue#Sky@7AM' is stronger AND easier to remember than 'P@ssw0rd123'." },

        { "passphrase", "[DEFINITION] A passphrase is a longer sequence of words or a sentence used as a password. It's easier to remember and harder to crack.|" +
                        "Passphrases are like secret sentences only you know. Longer = stronger, and you can actually remember them!|" +
                        "Pro tip: Use 4+ random words with symbols/numbers: 'Turtle$Dances@Moonlight42' is both secure and memorable." },

        { "pin", "[DEFINITION] A Personal Identification Number (PIN) is a short numeric code (usually 4–6 digits) used to verify identity for banking or phones.|" +
                 "Your PIN is like a mini-password for quick access. Keep it secret, keep it safe, and never write it on your card!|" +
                 "Avoid obvious PINs: 1234, 0000, 2580 (vertical line), or your birth year. Choose something only you would know." },

        { "login safety", "[TIPS] Always log out on shared or public devices. Never save passwords on untrusted browsers. Enable multi-step verification.|" +
                          "Logging out is like locking your front door. It takes 2 seconds but prevents hours of headache.|" +
                          "On public computers: use private browsing, log out when done, and never check 'Remember me'." },

        { "credentials", "[DEFINITION] Credentials are the combination of a username/email and password used to prove your identity to a system.|" +
                         "Your credentials are your digital identity. Guard them like you guard your wallet or ID document.|" +
                         "Never share credentials via email, SMS, or phone calls. Legitimate companies will NEVER ask for your password." },

        { "2fa", "[DEFINITION] Two-Factor Authentication (2FA) requires a second verification step after your password. It blocks 99.9% of automated account hacks.|" +
                 "2FA is like having two locks on your door. Even if someone steals your key (password), they still can't get in.|" +
                 "Enable 2FA on email, banking, social media, and cloud storage. It's the single best step to protect your accounts." },

        { "enable 2fa", "[TIPS] Go to account Security/Privacy settings. Find 'Two-Factor' or '2-Step Verification'. Use an authenticator app instead of SMS.|" +
                        "Setting up 2FA takes 2 minutes but protects you for years. Do it today for your most important accounts.|" +
                        "Prefer authenticator apps over SMS: they work offline and can't be intercepted via SIM-swapping attacks." },

        { "mfa", "[DEFINITION] Multi-Factor Authentication (MFA) is the broader term for 2FA. It combines something you know, have, or are.|" +
                 "MFA layers security like an onion: password + phone + fingerprint = very hard to peel back.|" +
                 "The more factors you add, the stronger your security. But even just 2 factors blocks most attacks." },

        { "biometrics", "[DEFINITION] Biometric authentication uses unique physical traits (fingerprint, facial recognition) to verify identity.|" +
                        "Biometrics are convenient but not foolproof. Use them for everyday access, but keep a strong password for sensitive actions.|" +
                        "Your fingerprint can't be changed if stolen. That's why biometrics work best as a second factor, not your only protection." },

        // PHISHING & SCAMS (10) 
        { "phishing", "[DEFINITION] Phishing is a cyber attack where criminals impersonate trusted organisations via email, SMS, or calls to trick you.|" +
                      "Phishing emails often create urgency: 'Your account will close in 24 hours!' Legitimate companies give you time.|" +
                      "Hover over links before clicking. If the URL looks suspicious (amaz0n-support.com), don't click. Go directly to the official website." },

        { "spot phishing", "[TIPS] Hover over links before clicking. Check sender addresses carefully. Beware of urgent threats. Legit companies never ask for passwords via email.|" +
                           "Red flags: generic greetings ('Dear Customer'), mismatched sender domains, and requests for personal info.|" +
                           "Phishers copy logos and design perfectly. The URL is your best clue: check for misspellings, weird domains, or missing 'https://'." },

        { "report phishing", "[TIPS] Forward suspicious emails to reportphishing@apwg.org. In South Africa, report cybercrime to the SAPS Cybercrimes Unit.|" +
                             "Reporting phishing helps protect others. Even if you didn't click, forwarding the email helps authorities track scams.|" +
                             "Keep a record: screenshot the email, note the sender and time, then report. This helps investigators build cases." },

        { "scam", "[DEFINITION] A scam is a fraudulent scheme designed to trick you into sending money, sharing personal data, or downloading malware.|" +
                  "If someone you've never met asks for money, gift cards, or crypto, it's a scam. Real relationships don't start with financial requests.|" +
                  "Tech support scams call claiming your computer is infected. Microsoft/Apple will NEVER call you unsolicited. Hang up and verify." },

        { "avoid scams", "[TIPS] If it sounds too good to be true, it is. Never pay upfront fees for 'prizes'. Verify requests via a separate channel.|" +
                         "Scammers exploit emotions: fear, greed, urgency. Pause, breathe, and verify before acting.|" +
                         "Google the offer + 'scam' before committing. Check reviews, forum discussions, and official warnings from consumer protection agencies." },

        { "smishing", "[DEFINITION] Smishing (SMS phishing) uses text messages to trick you into clicking malicious links or replying with personal information.|" +
                      "Banks and delivery companies won't ask you to click links in SMS to 'verify your account'. Open the official app instead.|" +
                      "Smishing texts often use urgency: 'Your package is delayed, click here'. Don't click. Contact the company via their official website." },

        { "vishing", "[DEFINITION] Vishing (voice phishing) uses phone calls to manipulate you into revealing sensitive data or sending money.|" +
                     "If a caller pressures you to act immediately or threatens consequences, hang up. Legitimate organizations give you time to verify.|" +
                     "Caller ID can be faked. If your 'bank' calls unexpectedly, hang up and call back using the number on your card." },

        { "email security", "[TIPS] Use strong, unique passwords for your email. Enable 2FA. Be cautious with attachments. Don't forward sensitive info to unverified addresses.|" +
                            "Your email is the master key to your digital life. If compromised, attackers can reset passwords for all your other accounts.|" +
                            "Review your email's recovery options regularly. Ensure your backup email and phone number are current and secure." },

        { "fake website", "[TIPS] Check the URL for misspellings (e.g., 'g00gle.com'). Look for 'https://' and a padlock icon. Verify contact info and privacy policies.|" +
                          "Fake websites clone legitimate ones perfectly. The URL is your best defense: check every character before entering credentials.|" +
                          "Bookmark important sites (banking, email) and always use your bookmark instead of clicking links in emails or searches." },

        { "spam", "[DEFINITION] Spam refers to unsolicited, bulk messages (usually email) sent for advertising, scams, or malware distribution.|" +
                  "Spam filters aren't perfect. If an email seems suspicious, don't click anything. Delete it or mark as spam to train your filter.|" +
                  "Never reply to spam or click 'unsubscribe' in suspicious emails. This confirms your address is active and invites more spam." },

        // MALWARE & THREATS (10) 
        { "malware", "[DEFINITION] Malware (malicious software) is any program designed to harm your device, steal data, or gain unauthorised access.|" +
                     "Malware hides in email attachments, fake software updates, and pirated downloads. Only install software from official sources.|" +
                     "Signs of malware: slow performance, unexpected pop-ups, files disappearing, or unusual network activity. Run a full antivirus scan." },

        { "protect from malware", "[TIPS] Keep OS and apps updated. Use reputable antivirus software. Never download from untrusted sites. Avoid pirated software.|" +
                                  "Updates patch security holes that malware exploits. Enable automatic updates to stay protected without thinking about it.|" +
                                  "Antivirus isn't optional—it's essential. Choose reputable solutions (Windows Defender, Bitdefender) and keep them updated." },

        { "ransomware", "[DEFINITION] Ransomware encrypts your files and demands payment for decryption. Never pay the ransom. Maintain offline backups.|" +
                        "Paying ransom doesn't guarantee file recovery and funds criminal activity. Prevention (backups + updates) is your best defense.|" +
                        "Follow the 3-2-1 backup rule: 3 copies, 2 different media, 1 offsite/offline. Test restores periodically to ensure backups work." },

        { "virus", "[DEFINITION] A computer virus is malware that attaches to legitimate programs, replicates when executed, and can corrupt data or slow systems.|" +
                   "Viruses spread when you open infected files or visit compromised sites. Keep your antivirus real-time protection enabled at all times.|" +
                   "Don't disable antivirus to 'speed up' your PC. Modern solutions run efficiently in the background. Security is worth a tiny performance cost." },

        { "trojan", "[DEFINITION] A trojan disguises itself as legitimate software to trick users into installing it. Once inside, it steals data or opens backdoors.|" +
                    "Trojans often pose as free games, cracks, or 'system optimizers'. If it sounds too good to be true, it probably is malware.|" +
                    "Check digital signatures on software downloads. Legitimate publishers sign their code; malware rarely does." },

        { "spyware", "[DEFINITION] Spyware secretly monitors your activity, records keystrokes, captures screenshots, or steals browsing data.|" +
                     "Spyware can capture passwords, banking details, and private messages. Use anti-malware tools that specifically detect spyware.|" +
                     "Review browser extensions regularly. Remove any you don't recognize. Malicious extensions can spy on everything you do online." },

        { "hacker", "[DEFINITION] A hacker is someone who uses technical skills to interact with computer systems. Ethical hackers improve security; malicious hackers exploit vulnerabilities.|" +
                    "Not all hackers are criminals. Ethical hackers (white hats) help companies find and fix security flaws before bad actors exploit them.|" +
                    "Protect yourself from malicious hackers: use strong passwords, enable 2FA, keep software updated, and be cautious with links." },

        { "data breach", "[DEFINITION] A data breach occurs when unauthorised individuals access, steal, or expose sensitive information.|" +
                         "If a service you use has a breach, change your password immediately. If you reused that password elsewhere, change those too.|" +
                         "Monitor accounts after a breach. Check bank statements, credit reports, and enable fraud alerts if identity theft is a risk." },

        { "respond to breach", "[TIPS] Change affected passwords immediately. Enable 2FA on all accounts. Monitor bank/credit statements. Freeze credit if identity is at risk.|" +
                               "After a breach, assume your credentials are exposed. Change passwords, enable 2FA, and watch for suspicious activity.|" +
                               "Use a password manager to generate and store unique passwords. If one account is breached, others remain safe." },

        { "ddos attack", "[DEFINITION] A Distributed Denial of Service (DDoS) attack floods a server or network with traffic to crash it.|" +
                         "DDoS attacks aim to make services unavailable, not steal data. While you can't stop a DDoS, you can report it and wait for mitigation.|" +
                         "If a website you use goes down suddenly, check their social media or status page. They may be experiencing a DDoS or maintenance." },

        // NETWORK & DEVICE SECURITY (10) 
        { "wifi security", "[TIPS] Use WPA3 encryption on your router. Change default admin credentials. Disable WPS. Keep router firmware updated.|" +
                           "Default router passwords are public knowledge. Change them immediately to prevent neighbors or attackers from accessing your network.|" +
                           "WPS (WiFi Protected Setup) has known vulnerabilities. Disable it in your router settings and use WPA3 with a strong password instead." },

        { "vpn", "[DEFINITION] A Virtual Private Network (VPN) encrypts your internet traffic and masks your IP address. Useful on public WiFi.|" +
                 "Public WiFi is like shouting in a crowded room—anyone can listen. A VPN encrypts your conversation so only you and the destination can understand.|" +
                 "Free VPNs often sell your data or inject ads. Choose reputable paid providers (ProtonVPN, Mullvad) with transparent privacy policies." },

        { "firewall", "[DEFINITION] A firewall monitors and filters incoming/outgoing network traffic based on security rules. It acts as a barrier between your device and untrusted networks.|" +
                      "Your OS firewall is your first line of defense. It blocks unsolicited connections and alerts you to suspicious outbound traffic.|" +
                      "Don't disable your firewall to 'fix' a connection issue. Instead, configure allowed apps in firewall settings or consult IT support." },

        { "update software", "[TIPS] Install OS, browser, and app updates promptly. Updates patch known security vulnerabilities. Enable automatic updates where possible.|" +
                             "Updates aren't just new features—they're security patches. Hackers actively exploit known vulnerabilities in outdated software.|" +
                             "Enable automatic updates for your OS, browser, and critical apps. For others, set a monthly reminder to check manually." },

        { "backup data", "[TIPS] Follow the 3-2-1 rule: 3 copies of data, 2 different storage types, 1 offsite/offline backup. Test restores periodically.|" +
                         "Backups are your insurance policy against ransomware, hardware failure, and accidental deletion. Without them, you risk losing everything.|" +
                         "Test your backups! A backup you can't restore is no backup at all. Practice restoring a file quarterly to ensure your process works." },

        { "encryption", "[DEFINITION] Encryption converts readable data into scrambled code that only authorised parties with a key can decrypt.|" +
                        "Full-disk encryption (BitLocker on Windows, FileVault on Mac) protects your data if your device is lost or stolen. Enable it today.|" +
                        "Encrypted messaging apps (Signal, WhatsApp) protect your conversations. Use them for sensitive discussions, not SMS." },

        { "public wifi", "[TIPS] Avoid logging into banking or email on public WiFi. Always use a trusted VPN. Disable auto-connect. Turn off file sharing.|" +
                         "Coffee shop WiFi is convenient but risky. Attackers can intercept unencrypted traffic. Use your phone's hotspot or a VPN for sensitive tasks.|" +
                         "If you must use public WiFi: enable your firewall, avoid financial transactions, and ensure websites use HTTPS (padlock icon)." },

        { "router security", "[TIPS] Change default login credentials. Update firmware regularly. Disable remote management. Use strong WPA3 passwords.|" +
                             "Your router is the gateway to your entire network. Secure it like you secure your front door: strong lock, regular maintenance.|" +
                             "Old routers stop receiving security updates. If yours is 5+ years old, consider upgrading to a modern model with WPA3." },

        { "antivirus", "[DEFINITION] Antivirus software detects, blocks, and removes malware. Modern solutions also include phishing protection and firewall integration.|" +
                       "Antivirus isn't set-and-forget. Keep definitions updated, run weekly scans, and pay attention to alerts.|" +
                       "Windows Defender is built-in and effective for most users. If you need advanced features, consider Bitdefender, Kaspersky, or ESET." },

        { "patch management", "[DEFINITION] Patch management is the process of identifying, testing, and applying software updates to fix security flaws.|" +
                              "Patches fix holes before hackers find them. Delaying updates leaves you vulnerable to known exploits. Install patches promptly.|" +
                              "For businesses: test patches in a staging environment before deploying widely. For individuals: enable automatic updates." },

        // PRIVACY & SAFE BROWSING (10)
        { "privacy", "[DEFINITION] Digital privacy is your right to control how your personal information is collected, used, and shared online.|" +
                     "Privacy isn't about having something to hide—it's about having something to protect: your identity, finances, relationships.|" +
                     "Review app permissions regularly. Does that flashlight app really need access to your contacts and location? If not, revoke it." },

        { "protect privacy", "[TIPS] Review app permissions regularly. Use privacy-focused browsers/search engines. Clear cookies/cache. Limit social media oversharing.|" +
                             "Every app, website, and service collects data. You can't stop all tracking, but you can minimize it: use private browsing, tracker blockers.|" +
                             "Social media oversharing creates a treasure map for scammers. Avoid posting your address, birthdate, pet names, or vacation plans publicly." },

        { "safe browsing", "[TIPS] Stick to HTTPS sites. Avoid clicking pop-ups or banner ads. Don't download files from untrusted sources. Use ad-blockers.|" +
                           "If a website looks suspicious, trust your gut. Close the tab, clear your cache, and run a quick antivirus scan.|" +
                           "Ad-blockers (uBlock Origin) not only remove annoying ads—they also block malicious ads (malvertising) that can install malware." },

        { "https", "[DEFINITION] HTTPS (Hypertext Transfer Protocol Secure) encrypts data between your browser and a website. Look for the padlock icon.|" +
                   "HTTPS doesn't guarantee a site is legitimate, but HTTP guarantees your data is exposed. Always check for the padlock before logging in.|" +
                   "Modern browsers warn you about HTTP sites. If you see 'Not Secure', don't proceed with sensitive actions." },

        { "cookies", "[DEFINITION] Cookies are small files websites store on your device to remember preferences or track activity.|" +
                     "First-party cookies (from the site you're visiting) are usually harmless. Third-party cookies track you across the web. Block them in privacy settings.|" +
                     "Clear cookies periodically to reset tracking. Use private browsing for sensitive searches. Consider privacy-focused browsers like Brave." },

        { "tracking", "[DEFINITION] Online tracking collects data about your browsing habits, location, and device for advertising or analytics.|" +
                      "You can't eliminate all tracking, but you can reduce it: use tracker blockers (Privacy Badger), disable third-party cookies, opt out of personalized ads.|" +
                      "Search engines like DuckDuckGo don't track your queries. Use them for sensitive searches to avoid building a profile of your interests." },

        { "popia", "[DEFINITION] POPIA (Protection of Personal Information Act) is South Africa's data privacy law. It gives you rights over your personal data.|" +
                   "Under POPIA, you can request companies to show you what data they hold, correct inaccuracies, or delete your information.|" +
                   "If a South African organisation mishandles your data, you can complain to the Information Regulator. Keep records of your interactions." },

        { "gdpr", "[DEFINITION] GDPR (General Data Protection Regulation) is the EU's strict data privacy law. It influences global standards.|" +
                  "Even if you're not in the EU, GDPR affects many global services. Use your GDPR rights: request data exports, correct errors, or delete accounts.|" +
                  "GDPR requires explicit consent for data collection. If a website pre-checks boxes or hides opt-outs, they may not be compliant." },

        { "identity theft", "[DEFINITION] Identity theft occurs when criminals steal personal information (ID number, bank details) to impersonate you or commit fraud.|" +
                            "Protect your ID number like your password. Never share it via email, SMS, or phone unless you initiated the contact.|" +
                            "Signs of identity theft: unfamiliar charges, accounts you didn't open, or credit applications you didn't submit. Act immediately." },

        { "secure download", "[TIPS] Only download from official vendor sites or verified app stores. Check file extensions (.exe vs .txt). Scan downloads with antivirus.|" +
                             "Pirated software is a major malware vector. The 'free' version often costs you your data, privacy, or device security.|" +
                             "Check digital signatures on downloads. Right-click the file → Properties → Digital Signatures. Legitimate publishers sign their code." }
    };
        }


        private void AddBotMessage(string text)
        {
            _messages.Add(new ChatMessage { Text = text, Background = "#0f3460", Alignment = HorizontalAlignment.Left });
            ChatScroll.ScrollToEnd();
        }

        private void AddUserMessage(string text)
        {
            _messages.Add(new ChatMessage { Text = text, Background = "#00d4ff", Foreground = "#1a1a2e", Alignment = HorizontalAlignment.Right });
            ChatScroll.ScrollToEnd();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput();
        private void UserInput_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) ProcessInput(); }

        private void ProcessInput()
        {
            string userInput = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                AddBotMessage(" Please enter a message.");
                return;
            }

            AddUserMessage(userInput);
            UserInput.Clear();
            StatusText.Text = "Processing...";

            string response = _chatService.GetResponse(userInput);
            AddBotMessage(response);
            StatusText.Text = "Ready | Type 'help' for topics";
        }
    }
}