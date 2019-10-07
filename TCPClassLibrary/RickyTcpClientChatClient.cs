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
        private string username;

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
            networkStream = tcpClient.GetStream();
        }

        public void SendMessage(string type, string message, Action<string> showMessageAction)
        {
            string protocolMessage = Parser.CreateProtocolMessage(type, username, message);
            string preparedMessage = Parser.PrepareProtocolMessageForTransfer(protocolMessage);
            byte[] messageToBeSent = Encoding.ASCII.GetBytes(preparedMessage);
            networkStream.Write(messageToBeSent, 0, messageToBeSent.Length);
            showMessageAction(message);
        }
    }
}
