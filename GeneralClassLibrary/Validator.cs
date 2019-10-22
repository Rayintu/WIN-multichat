using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralClassLibrary
{
    public class Validator
    {
        public static bool ValidateIP(string ipAdress)
        {
            bool isRegex = Regex.IsMatch(ipAdress, @"(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])");

            return isRegex;
        }

        public static bool ValidatePort(int port)
        {
            if (port >= 0 && port <= 65535)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool validateBufferSize(int bufferSize)
        {
            if (bufferSize >= 0 && bufferSize < 6000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
