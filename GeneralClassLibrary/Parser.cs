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
            return ($"@{type}` @{username}` @{message}`»");
        }

        public static string PrepareProtocolMessageForTransfer(string protocolMessage)
        {
            string newMessage;

            newMessage = protocolMessage.Replace("@", "&#64;");
            newMessage = newMessage.Replace("`", "&#96;");
            newMessage = newMessage.Replace("»", "&#187");

            return newMessage;
        }

        public static string DecodeProtocolMessage(string encodedProtocolMessage)
        {
            string newMessage;
            newMessage = encodedProtocolMessage.Replace("&#64;", "@");
            newMessage = newMessage.Replace("&#96;", "`");
            newMessage = newMessage.Replace("&#187", "»");

            return newMessage;
        }

        public static MatchCollection ProtocolToMatchesArray(string message)
        {
            Regex regex = new Regex(@"(\@.*?`)");
            return regex.Matches(message);
        }

        public static string FormatMessage(ProtocolMessageObject messageObject)
        {
            
            switch (messageObject.protocolType)
            {
                case ("CONNECT"):
                    return $"[{messageObject.username}] Connected!";
                    break;
                case ("MESSAGE"):
                    return $"[{messageObject.username}]: {messageObject.message}";
                    break;
                default:
                    return $"Something went wrong with a message [{messageObject.username}] sent.";
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
