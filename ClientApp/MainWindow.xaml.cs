using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCPClassLibrary;
using GeneralClassLibrary;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected delegate void UpdateClientDisplayDelegate(string message);

        protected delegate void ErrorWindowDisplayDelegate(string message);

        protected delegate void AddMessageClientDelegate(string message);

        private string username = "";

        private RickyTcpClientChatClient tcpClientChat; 

        public MainWindow()
        {
            tcpClientChat = new RickyTcpClientChatClient(username);
            InitializeComponent();
            
        }
        
        private void Connect_onClick(object sender, RoutedEventArgs e)
        {
            if (Validator.ValidateIP(textBox_ip.Text))
            {
                txtBox_Username.IsEnabled = false;
                textBox_ip.IsEnabled = false;
                textBox_Port.IsEnabled = false;
                AddMessage("connecting...");
                tcpClientChat.ConnectToServer(textBox_ip.Text, Parser.StringToInt(textBox_Port.Text), Parser.StringToInt(txtBox_bufferSize.Text), AddMessage, ShowErrorDialog);
                txtBox_Username.Text = tcpClientChat.username;
                btnConnect.Content = "Disconnect";
            }
            else
            {
                ShowErrorDialog("Invalid IP address!");
            }
        }

        private void ShowErrorDialog(string message)
        {
            if (chatClient_window.Dispatcher.CheckAccess())
            {
                ShowClientErrorDialog(message);
            }
            else
            {
                chatClient_window.Dispatcher.Invoke(new ErrorWindowDisplayDelegate(ShowClientErrorDialog),new object[] {message});
            }
        }

        private void ShowClientErrorDialog(string message)
        {
            MessageBox.Show(message, "No connection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AddMessage(string message)
        {
            if (chat_list.Dispatcher.CheckAccess())
            {
                AddClientMessage(message);
            }
            else
            {
                chat_list.Dispatcher.Invoke(new UpdateClientDisplayDelegate(AddClientMessage), new object[] { message });
            }
        }

        private void AddClientMessage(string message)
        {
            chat_list.Items.Add(message);
        }

        private void buttonSend_onClick(object sender, RoutedEventArgs e)
        {
            tcpClientChat.SendMessage("MESSAGE", tcpClientChat.username, txtMessage_textBox.Text, Parser.StringToInt(txtBox_bufferSize.Text), tcpClientChat.networkStream, AddMessage);
            AddMessage($"[You]: {txtMessage_textBox.Text}");
            txtMessage_textBox.Clear();
            txtMessage_textBox.Focus();
        }

        private void txtBox_onTextChange(object sender, TextChangedEventArgs e)
        {
            tcpClientChat.username = txtBox_Username.Text;
        }

        
    }
}
