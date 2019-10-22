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

        public async void StartTcpServer(int portNumber, int bufferSize, Action<string> showMessageAction,
            Action<string> showErrorAction)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
                tcpListener.Start();
                showMessageAction("Started server!");
                await Task.Run(() => ListenForConnections(bufferSize, showMessageAction));
            }
            catch (SocketException socketException)
            {
                showErrorAction("There is a server already running on this port!");
            }
        }

        private async Task ListenForConnections(int bufferSize, Action<string> showMessageAction)
        {
            while (serverRunning)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                connectedTcpClients.Add(client);
                Task.Run(() => ListenForUserConnectMessage(client, bufferSize, showMessageAction));
            }
        }

        private void ListenForUserConnectMessage(TcpClient client, int bufferSize, Action<string> showMessageAction)
        {
            string message = "";
            string decodedMessage = "";
            byte[] buffer = new byte[bufferSize];
            NetworkStream networkStream = client.GetStream();
            bool connected = false;

            while (!connected)
            {
                while (!decodedMessage.EndsWith("»"))
                {
                    int readBytes = networkStream.Read(buffer, 0, bufferSize);
                    message += Encoding.ASCII.GetString(buffer, 0, readBytes);
                    decodedMessage = Parser.DecodeProtocolMessage(message);
                }

                MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                ProtocolMessageObject protocolMessageObject = new ProtocolMessageObject(matchCollection);
                if (protocolMessageObject.protocolType == "CONNECT")
                {
                    message = "";
                    decodedMessage = "";
                    ChatClient chatClient = new ChatClient(networkStream, protocolMessageObject.username);
                    connectedChatClients.Add(protocolMessageObject.username, chatClient);
                    showMessageAction(Parser.FormatMessage(protocolMessageObject));
                    Task.Run(() => ListenForMessages(chatClient, showMessageAction));
                    BroadcastMessages(protocolMessageObject, bufferSize, showMessageAction);
                    connected = true;
                }
            }
        }

        private void ListenForMessages(ChatClient chatClient, Action<string> showMessageAction)
        {
            int bufferSize = 2;
            string message = "";
            string decodedMessage = "";
            byte[] buffer = new byte[bufferSize];
            NetworkStream networkStream = chatClient.networkStream;

            while (networkStream.CanRead)
            {
                while (!decodedMessage.EndsWith("»"))
                {
                    int readBytes = networkStream.Read(buffer, 0, bufferSize);
                    message += Encoding.ASCII.GetString(buffer, 0, readBytes);
                    decodedMessage = Parser.DecodeProtocolMessage(message);
                }


                MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                ProtocolMessageObject protocolMessageObject = new ProtocolMessageObject(matchCollection);
                BroadcastMessages(protocolMessageObject, bufferSize, showMessageAction);
                string messageToShow = Parser.FormatMessage(protocolMessageObject);
                showMessageAction(messageToShow);
                message = "";
                decodedMessage = "";
            }

            networkStream.Close();
        }

        public void BroadcastMessages(ProtocolMessageObject protocolMessage, int bufferSize, Action<string> showMessageAction)
        {
            foreach (KeyValuePair<string, ChatClient> entry in connectedChatClients)
            {
                if (entry.Key != protocolMessage.username)
                {
                    SendMessage(
                        protocolMessage.protocolType,
                        protocolMessage.username,
                        protocolMessage.message,
                        bufferSize,
                        entry.Value.networkStream,
                        showMessageAction
                    );
                }
            }
        }
    }
}