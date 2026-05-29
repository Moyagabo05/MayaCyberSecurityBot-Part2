using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MayaCyberSecurityBot2
{
    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public string Background { get; set; } = "#0f3460";
        public string Foreground { get; set; } = "White";
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        public bool IsTyping { get; set; } = false;
    }
}
