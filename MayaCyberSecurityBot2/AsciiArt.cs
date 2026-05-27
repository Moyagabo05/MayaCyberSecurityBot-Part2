using System;
using System.Collections.Generic;
using System.Text;

namespace MayaCyberSecurityBot2
{
    public static class AsciiArt
    {
        public static string GetLogoForGui()
        {
            return @"
╔────────────────────────────────────────────────────────────────────────────╗
│ _ _                                                                   _ _  │
│( Y ) ║══════════════════════════════════════════════════════════════╗( Y ) │
│ \ /  ║                  .----------------------.                    ║ \ /  │
│  Y   ║                  |MAYA CYBERSECURITY BOT|                    ║  Y   │
│      ═                  '----------------------'                    ═      │
│      ║                                                              ║      │
│      ║                                                              ║      │
│      ║                                                              ║      │
│      ═                [ NETWORK SECURITY ACTIVE!! ]                 ═      │
│ _ _  ║                                                              ║  _ _ │
│( Y ) ║          PROTECTING SOUTH AFRICAN DIGITAL CITIZENS           ║ ( Y )│
│ \ /  ║                                                              ║  \ / │
│  Y   ╚═══════════════════════════════════════════════════════════════   Y  │
╚────────────────────────────────────────────────────────────────────────────╝";
        }

        public static void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(GetLogoForGui());
            Console.ResetColor();
        }
    }
}
