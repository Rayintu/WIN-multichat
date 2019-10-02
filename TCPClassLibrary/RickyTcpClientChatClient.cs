using System;
using System.Collections.Generic;
using System.Text;

namespace TCPClassLibrary
{
    class RickyTcpClientChatClient : RickyTcpClient
    {
        public RickyTcpClientChatClient(Delegate addMessageDelegate) : base(addMessageDelegate)
        {
        }
    }
}
