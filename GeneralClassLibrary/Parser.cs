using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralClassLibrary
{
    public class Parser
    {
        public static string CreateProtocolMessage(string type, string username, string message)
        {
            return ($"@{type}` @{username}` @{message}`");
        }

        public static string PrepareProtocolMessageForTransfer(string protocolMessage)
        {
            string newMessage;

            newMessage = protocolMessage.Replace("@", "&#64;");
            newMessage = newMessage.Replace("`", "&#96;");

            return newMessage;
        }

        public static string DecodeProtocolMessage(string encodedProtocolMessage)
        {
            string newMessage;
            newMessage = encodedProtocolMessage.Replace("&#64;", "@");
            newMessage = encodedProtocolMessage.Replace("&#96;", "`");

            return newMessage;
        }

        public static MatchCollection ProtocolToMatchesArray(string message)
        {
            Regex regex = new Regex(@"(\@.*?`)");
            return regex.Matches(message);
        }

        public static string FormatMessage(MatchCollection matches)
        {
            string type = matches[0].ToString();
            string username = matches[1].ToString();
            string message = matches[2].ToString();

            switch (type)
            {
                case ("CONNECT"):
                    return $"{username} Connected!";
                    break;
                default:
                    return "Well, this is awkward";
                    break;
            }

        }

        public static int StringToInt(string stringToConvert)
        {
            try
            {
                return System.Convert.ToInt32(stringToConvert);
            }
            catch (FormatException)
            {
                return -1;
            }
            catch (OverflowException)
            {
                return -1;
            }
        }
    }
}
