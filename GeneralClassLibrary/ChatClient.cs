using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace GeneralClassLibrary
{
    public class ChatClient
    {
        public NetworkStream networkStream { get; }
        public string username { get; }

        public ChatClient(NetworkStream networkStream, string username)
        {
            this.networkStream = networkStream;
            this.username = username;
        }
    }
}
