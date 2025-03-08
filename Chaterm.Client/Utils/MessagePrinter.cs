using System;
using System.Security.Cryptography;
using System.Text;

namespace Chaterm.Client.Utils;

public static class MessagePrinter
{
    public static string PrintMessage(this Message message, string userName)
    {
        string content = message.Content;
        string mention = $"> {userName}";

        if (content.Contains(mention, StringComparison.OrdinalIgnoreCase))
        {
            content = content.Replace(mention, $"\e[7m{mention}\e[0m\e[1m");
            content = $"\e[1m{content}\e[0m";
        }

        StringBuilder sb = new StringBuilder($"{message.UserName}: {DateTime.Now:HH:mm:ss}");

        foreach (string line in content.Split('\n'))
        {
            sb.Append($"\n {line}");
        }
        return sb.ToString();
    }
}