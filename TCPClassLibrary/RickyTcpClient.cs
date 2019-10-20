using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using GeneralClassLibrary;

namespace TCPClassLibrary
{
    public class RickyTcpClient
    {
        public void SendMessage(string type, string username, string message, int bufferSize, NetworkStream networkStream, Action<string> showMessageAction)
        {
            string protocolMessage = Parser.CreateProtocolMessage(type, username, message);
            string preparedMessage = Parser.PrepareProtocolMessageForTransfer(protocolMessage);
            byte[] messageToBeSent = Encoding.ASCII.GetBytes(preparedMessage);

            if (messageToBeSent.Length == bufferSize)
            {
                networkStream.Write(messageToBeSent, 0, bufferSize);
            }
            else if (messageToBeSent.Length < bufferSize)
            {
                networkStream.Write(messageToBeSent, 0, messageToBeSent.Length);
            }
            else
            {
                SendMessageChunked(preparedMessage, bufferSize, networkStream);
            }

            
        }

        private void SendMessageChunked(string preparedMessage, int bufferSize, NetworkStream networkStream)
        {
            int nrOfChunks = (int)Math.Ceiling((decimal) preparedMessage.Length / bufferSize);
            for (int index = 0; index < nrOfChunks; index++)
            {
                string substringToSend = preparedMessage.Substring(index * bufferSize, bufferSize);
                byte[] chunkToSend = Encoding.ASCII.GetBytes(substringToSend);
                networkStream.Write(chunkToSend, 0, bufferSize);
            }
        }
    }
}
