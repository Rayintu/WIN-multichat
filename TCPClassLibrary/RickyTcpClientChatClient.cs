using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeneralClassLibrary;

namespace TCPClassLibrary
{
    public class RickyTcpClientChatClient : RickyTcpClient
    {
        private TcpClient tcpClient;
        public NetworkStream networkStream { get; set; }
        public string username { get; set; }
        private int bufferSize = 1024;

        public RickyTcpClientChatClient(string username)
        {
            this.username = username;
        }

        public async void ConnectToServer(string ipAddress, int port, int bufferSize, Action<string> showMessageAction,
            Action<string> showErrorDialogAction)
        {
            this.bufferSize = bufferSize;
            try
            {
                if (username.Length == 0)
                {
                    username = GenerateUsername();
                }


                tcpClient = new TcpClient(ipAddress, 9000);
                await Task.Run(() => ConnectToServerAsync(showMessageAction));
                showMessageAction("connected to server!");
                SendMessage("CONNECT", username, "", bufferSize, networkStream, showMessageAction);
                Task.Run(() => ListenForMessages(showMessageAction, showErrorDialogAction));
            }
            catch (SocketException socketException)
            {
                showErrorDialogAction(socketException.Message);
                Debug.WriteLine(socketException.Message);
            }
        }

        private void ConnectToServerAsync(Action<string> showMessageAction)
        {
            networkStream = tcpClient.GetStream();
        }

        private void ListenForMessages(Action<string> showMessageAction, Action<string> showErrorDialogAction)
        {
            string message = "";
            string decodedMessage = "";
            byte[] buffer = new byte[bufferSize];

            while (networkStream.CanRead)
            {
                try
                {
                    while (!decodedMessage.EndsWith("»"))
                    {
                        int readBytes = networkStream.Read(buffer, 0, bufferSize);
                        message += Encoding.ASCII.GetString(buffer, 0, readBytes);
                        decodedMessage = Parser.DecodeProtocolMessage(message);
                    }

                    MatchCollection matchCollection = Parser.ProtocolToMatchesArray(decodedMessage);
                    ProtocolMessageObject protocolMessageObject = new ProtocolMessageObject(matchCollection);
                    string messageToShow = Parser.FormatMessage(protocolMessageObject);
                    showMessageAction(messageToShow);
                    message = "";
                    decodedMessage = "";
                }
                catch (IOException ioException)
                {
                    showMessageAction("Disconnected");
                    showErrorDialogAction("Connection to the server lost!");
                    networkStream.Close();
                }
            }

            networkStream.Close();
        }

        private string GenerateUsername()
        {
            Random random = new Random();
            int id = random.Next(1000, 9999);
            return $"anon{id}";
        }
    }
}