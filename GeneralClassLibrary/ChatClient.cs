using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace GeneralClassLibrary
{
    public class ChatClient
    {
        private NetworkStream networkStream { get; }
        private string username { get; }

        public ChatClient(NetworkStream networkStream, string username)
        {
            this.username = username;
        }
    }
}
