using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected delegate void UpdateServerDisplayDelegate(string message);

        protected delegate void ErrorWindowDisplayDelegate(string message);

        private RickyTcpClientServerClient rickyTcpClient = new RickyTcpClientServerClient();

        private bool serverNotRunning = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowErrorDialog(string message)
        {
            if (serverApp_window.Dispatcher.CheckAccess())
            {
                ShowClientErrorDialog(message);
            }
            else
            {
                serverApp_window.Dispatcher.Invoke(new ErrorWindowDisplayDelegate(ShowClientErrorDialog), new object[] { message });
            }
        }

        private void ShowClientErrorDialog(string message)
        {
            RunServer_Button.Content = "Start server";
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AddMessage(string message)
        {
            
            if (messages_list.Dispatcher.CheckAccess())
            {
                AddServerMessage(message);
            }
            else
            {
                messages_list.Dispatcher.Invoke(new UpdateServerDisplayDelegate(AddServerMessage), new object[] { message });
            }
        }

        private void AddServerMessage(string message)
        {
            messages_list.Items.Add(message);
        }

        private void StartStopServer_onClick(object sender, RoutedEventArgs e)
        {
            if (serverNotRunning)
            {
                int port = Parser.StringToInt(Port_Input.Text);
                int bufferSize = Parser.StringToInt(BufferSize_Input.Text);

                RunServer_Button.Content = "Stop server";
                Port_Input.IsEnabled = false;
                serverNotRunning = false;
                rickyTcpClient.StartTcpServer(port, bufferSize, AddMessage, ShowErrorDialog);
            }
            else
            {

            }
            
        }
    }
}
