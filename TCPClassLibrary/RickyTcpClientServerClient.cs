using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeneralClassLibrary;


namespace TCPClassLibrary
{
    public class RickyTcpClientServerClient : RickyTcpClient
    {
       

        private bool serverRunning = true;
        private TcpListener tcpListener;
        private Dictionary<string, ChatClient> connectedChatClients = new Dictionary<string, ChatClient>();
        private List<TcpClient> connectedTcpClients = new List<TcpClient>();
        
        public async void StartTcpServer(int portNumber, int bufferSize, Action<string> showMessageAction)
        {
            showMessageAction("Started server!");

            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
            tcpListener.Start();
            await Task.Run(() => ListenForConnections(showMessageAction));
            
        }

        private async Task ListenForConnections(Action<string> showMessageAction)
        {
            while (serverRunning)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                connectedTcpClients.Add(client);
                ListenForMessages(client, showMessageAction);
                showMessageAction("someone connected");
            }
        }

        private void ListenForMessages(TcpClient client, Action<string> showMessageAction)
        {
            int bufferSize = 1024;
            string message = " ";
            byte[] buffer = new byte[bufferSize];
            NetworkStream networkStream = client.GetStream();

            while (networkStream.CanRead)
            {
                int readBytes = networkStream.Read(buffer, 0, bufferSize);
                message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                string decodedMessage = Parser.DecodeProtocolMessage(message);
                MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                string messageToShow = Parser.FormatMessage(matchCollection);
                showMessageAction(messageToShow);
            }

            networkStream.Close();
            client.Close();
        }

        public void BroadcastMessages()
        {

        }
    }
}
