using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Lab2_1
{
    public partial class Server : Form
    {
        TcpListener server;
        int port;
        IPAddress ipAddress;


        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }

        private void textHost_TextChanged(object sender, EventArgs e)
        {

        }

        private void textPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (server == null)
                return;
            
            server.Stop();
            server = null;
            textStatus.Text += "Server stoped " + Environment.NewLine;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (server != null)
                return;
            
            var directory = Directory.GetCurrentDirectory() + "\\exe";
            if (!IPAddress.TryParse(textHost.Text, out ipAddress))
            {
                textStatus.Text += "Invalid address: " + textHost.Text + Environment.NewLine;
                return;
            }


            if (!int.TryParse(textPort.Text, out port))
            {
                textStatus.Text += "Invalid port: " + textPort.Text + Environment.NewLine;
                return;
            }
            
            try
            {
                server = new TcpListener(ipAddress, port);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                textStatus.Text += "Invalid port: " + ex.Message + Environment.NewLine;
                return;
            }


            try
            {
                // Start listening for client requests.
                server.Start();
                textStatus.Text += "Server started" + Environment.NewLine;
                //Enter the listening loop.
                while (true)
                {
                    textStatus.Text += "Waiting for a connection... " + Environment.NewLine;
                    try
                    {
                        using (TcpClient client = await server.AcceptTcpClientAsync())
                        using (StreamReader sr = new StreamReader(client.GetStream()))
                        using (StreamWriter sw = new StreamWriter(client.GetStream()))
                        {
                            sw.AutoFlush = true;
                            textStatus.Text += "Connected!" + Environment.NewLine;
                            // Get a stream object for reading and writing
                            // Buffer for reading data

                            while (client.Connected)
                            {
                                var serverMethod = await sr.ReadLineAsync();
                                textStatus.Text += "Read client request: " + serverMethod + Environment.NewLine;

                                switch (serverMethod)
                                {
                                    case "listFiles":
                                        var availableExeFiles = "";
                                        foreach (var file in Directory.GetFiles(directory))
                                        {
                                            if (file.EndsWith(".exe"))
                                            {
                                                int indexDelimeter = file.LastIndexOf('\u005C');
                                                textStatus.Text +=
                                                    "Sending file " + file.Substring(indexDelimeter + 1) +
                                                    Environment.NewLine;
                                                availableExeFiles += file.Substring(indexDelimeter + 1) + ";";
                                            }
                                        }

                                        await sw.WriteLineAsync(availableExeFiles);
                                        break;
                                    case "sendFile":
                                        textStatus.Text += "Sending files " + Environment.NewLine;
                                        var currentFile = await sr.ReadLineAsync();

                                        var sendingFile = directory + "\\" + currentFile;
                                        textStatus.Text += "Reading file into bytes " + currentFile +
                                                           Environment.NewLine;

                                        var fileInBytes = File.ReadAllBytes(sendingFile);
                                        using (BinaryWriter bw = new BinaryWriter(client.GetStream(),
                                                   new UTF8Encoding(false, true), leaveOpen: true))
                                        {
                                            bw.Write(fileInBytes);
                                        }

                                        textStatus.Text += "Sending file for execution " + currentFile +
                                                           Environment.NewLine;
                                        break;
                                    default:
                                        textStatus.Text += "Forbidden method " + Environment.NewLine;
                                        break;
                                }
                            }
                        }
                    }
                    catch (SocketException exception)
                    {
                        textStatus.Text += $"Socket ex: {exception.Message} " + Environment.NewLine;
                    }
                }
            }
            catch (Exception exception)
            {
                textStatus.Text += "Error " + exception.Message + Environment.NewLine;
            }
            
            
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }
    }
}
