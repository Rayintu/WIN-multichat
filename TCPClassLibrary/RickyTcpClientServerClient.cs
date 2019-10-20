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
        
        public async void StartTcpServer(int portNumber, int bufferSize, Action<string> showMessageAction, Action<string> showErrorAction)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
                tcpListener.Start();
                showMessageAction("Started server!");
                await Task.Run(() => ListenForConnections(showMessageAction));
            }
            catch (SocketException socketException)
            {
                showErrorAction("There is a server already running on this port!");
            }
            
        }

        private async Task ListenForConnections(Action<string> showMessageAction)
        {
            while (serverRunning)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                connectedTcpClients.Add(client);
                Task.Run( () => ListenForUserConnectMessage(client, showMessageAction));
            }
        }

        private void ListenForUserConnectMessage(TcpClient client, Action<string> showMessageAction)
        {
            int bufferSize = 1024;
            string message = " ";
            byte[] buffer = new byte[bufferSize];
            NetworkStream networkStream = client.GetStream();
            bool connected = false;

            while (!connected)
            {
                int readBytes = networkStream.Read(buffer, 0, bufferSize);
                message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                string decodedMessage = Parser.DecodeProtocolMessage(message);
                MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                ProtocolMessageObject protocolMessageObject = new ProtocolMessageObject(matchCollection);
                if (protocolMessageObject.protocolType == "CONNECT")
                {
                    ChatClient chatClient = new ChatClient(networkStream, protocolMessageObject.username);
                    connectedChatClients.Add(protocolMessageObject.username, chatClient);
                    showMessageAction(Parser.FormatMessage(protocolMessageObject));
                    Task.Run(() => ListenForMessages(chatClient, showMessageAction));
                    connected = true;
                }
            }
        }

        private void ListenForMessages(ChatClient chatClient, Action<string> showMessageAction)
        {
            int bufferSize = 1024;
            string message = " ";
            byte[] buffer = new byte[bufferSize];
            NetworkStream networkStream = chatClient.networkStream;

            while (networkStream.CanRead)
            {
                int readBytes = networkStream.Read(buffer, 0, bufferSize);
                message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                string decodedMessage = Parser.DecodeProtocolMessage(message);
                MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                ProtocolMessageObject protocolMessageObject = new ProtocolMessageObject(matchCollection);
                BroadcastMessages(message, protocolMessageObject.username);
                string messageToShow = Parser.FormatMessage(protocolMessageObject);
                showMessageAction(messageToShow);
            }

            networkStream.Close();
        }

        public void BroadcastMessages(string encodedMessage, string ignoreUsername)
        {
            int bufferSize = 1024;
            byte[] messageToSend = Encoding.ASCII.GetBytes(encodedMessage);
            foreach (KeyValuePair<string, ChatClient> entry in connectedChatClients)
            {
                if (entry.Key != ignoreUsername)
                {
                    entry.Value.networkStream.Write(messageToSend, 0, messageToSend.Length);
                }
            }
        }
    }
}
