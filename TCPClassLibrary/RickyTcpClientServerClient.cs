using System;
using System.Collections.Generic;
using System.Text;

namespace TCPClassLibrary
{
    class RickyTcpClientServerClient : RickyTcpClient
    {
        public RickyTcpClientServerClient(Delegate addMessageDelegate) : base(addMessageDelegate)
        {
        }
    }
}
