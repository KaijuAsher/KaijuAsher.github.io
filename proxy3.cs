using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProxyServerApp
{
    public partial class MainWindow : Window
    {
        private TcpListener _listener;
        private TcpClient _client;

        public MainWindow()
        {
            InitializeComponent();
            StartListening();
        }

        private void StartListening()
        {
            try
            {
                // create a new TCP listener on port 8888
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();
                LogMessage("Proxy server started on port 8888.");

                // accept incoming connections and process them asynchronously
                _listener.BeginAcceptTcpClient(AcceptConnection, null);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private void AcceptConnection(IAsyncResult result)
        {
            try
            {
                // end the asynchronous operation and get the client socket
                _client = _listener.EndAcceptTcpClient(result);

                // process the client request
                byte[] buffer = new byte[1024];
                int bytesRead = _client.GetStream().Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string response = SendRequest(request);

                // send the response back to the client
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                _client.GetStream().Write(responseBytes, 0, responseBytes.Length);

                // close the client socket
                _client.Close();

                // accept the next connection
                _listener.BeginAcceptTcpClient(AcceptConnection, null);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private string SendRequest(string request)
        {
            try
            {
                // create a new TCP client to connect to the destination server
                TcpClient destinationClient = new TcpClient("www.example.com", 80);

                // send the request to the destination server
                byte[] requestBytes = Encoding.UTF8.GetBytes(request);
                destinationClient.GetStream().Write(requestBytes, 0, requestBytes.Length);

                // receive the response from the destination server
                byte[] buffer = new byte[1024];
                int bytesRead = destinationClient.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // close the destination client socket
                destinationClient.Close();

                return response;
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                return "";
            }
        }

        private void LogMessage(string message)
        {
            // add a new message to the log
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{DateTime.Now}: {message}\n");
                LogTextBox.ScrollToEnd();
            });
        }

        private void LogError(string error)
        {
            // add a new error message to the log
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{DateTime.Now}: ERROR - {error}\n");
                LogTextBox.ScrollToEnd();
            });
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            // clear the log
            LogTextBox.Document.Blocks.Clear();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            // allow the user to drag the window by clicking and dragging anywhere on it
            DragMove();
        }

        private void
