using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GeneralClassLibrary;

namespace TCPClassLibrary
{
    public class RickyTcpClientChatClient : RickyTcpClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        public string username { get; set; }

        public RickyTcpClientChatClient(string username)
        {
            
            this.username = username;
            
            
        }

        public async void ConnectToServer(string ipAddress, int port, Action<string> showMessageAction, Action<string> showErrorDialog)
        {
            
            try
            {
                tcpClient = new TcpClient(ipAddress, 9000);
                await Task.Run(() => ConnectToServerAsync(showMessageAction));
                showMessageAction("connected to server!");
                SendMessage("CONNECT", " ", showMessageAction);

            }
            catch (SocketException socketException)
            {
                showErrorDialog(socketException.Message);
                Debug.WriteLine(socketException.Message);
            }
            
        }

        private void ConnectToServerAsync(Action<string> showMessageAction)
        {
            if (username.Length == 0)
            {
                username = GenerateUsername();
            }

            networkStream = tcpClient.GetStream();
        }

        public void SendMessage(string type, string message, Action<string> showMessageAction)
        {
            string protocolMessage = Parser.CreateProtocolMessage(type, username, message);
            string preparedMessage = Parser.PrepareProtocolMessageForTransfer(protocolMessage);
            byte[] messageToBeSent = Encoding.ASCII.GetBytes(preparedMessage);
            networkStream.Write(messageToBeSent, 0, messageToBeSent.Length);
        }

        private string GenerateUsername()
        {
            Random random = new Random();
            int id = random.Next(1000, 9999);
            return $"anon{id}";
        }
    }
}
