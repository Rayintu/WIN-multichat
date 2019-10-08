using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralClassLibrary
{
    public class ProtocolMessageObject
    {
        public string protocolType { get; }
        public string username { get; }
        public string message { get; }



        public ProtocolMessageObject(MatchCollection matchCollection)
        {
            this.protocolType = matchCollection[0].ToString().Remove(0,1);
            this.protocolType = protocolType.Remove(protocolType.Length - 1);
            this.username = matchCollection[1].ToString().Remove(0, 1);
            this.username = username.Remove(username.Length - 1);
            this.message = matchCollection[2].ToString().Remove(0, 1);
            this.message = message.Remove(message.Length - 1);

        }
    }
}
