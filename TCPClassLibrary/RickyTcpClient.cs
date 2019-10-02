using System;
using System.Collections.Generic;
using System.Text;

namespace TCPClassLibrary
{
    class RickyTcpClient
    {
        private Delegate addMessageDelegate;

        public RickyTcpClient(Delegate addMessageDelegate)
        {
            this.addMessageDelegate = addMessageDelegate;
        }

        public void listenForMessages(Action<String> message)
        {

        }
    }
}
