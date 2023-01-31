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
        StreamReader sr;
        StreamWriter sw;
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

            server.Server.Close();
            textStatus.Text += "Server stoped " + Environment.NewLine; 
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            ipAddress = IPAddress.Parse(textHost.Text);
            port = int.Parse(textPort.Text);
            // TcpListener server = new TcpListener(port);
            server = new TcpListener(ipAddress, port);

            // Start listening for client requests.
            server.Start();
            textStatus.Text += "Server started" + Environment.NewLine;
            //Enter the listening loop.
            while (true)
            {
                TcpClient client = null;
                textStatus.Text += "Waiting for a connection... " + Environment.NewLine;
                try
                {
                    client = await server.AcceptTcpClientAsync();
                } catch (Exception ex)
                {
                    textStatus.Text += "Server shutdown... " + Environment.NewLine;
                    break;
                }


                BinaryWriter bw = null;
                if (client != null && client.Connected) {
                    textStatus.Text += "Connected!" + Environment.NewLine;
                    try
                    {
                        sr = new StreamReader(client.GetStream());
                        sw = new StreamWriter(client.GetStream());
                        sw.AutoFlush = true;
                        // Get a stream object for reading and writing
                        // Buffer for reading data

                        string data = await sr.ReadLineAsync();

                        textStatus.Text += "Read client request: " + data + Environment.NewLine;

                        string directory = Directory.GetCurrentDirectory() + "\\exe";
                        string[] files = Directory.GetFiles(directory);
                        string all = "";
                        for (int j = 0; j < files.Length; j++)
                        {
                            if (files[j].EndsWith(".exe"))
                            {
                                int indexDelimeter = files[j].LastIndexOf('\u005C');
                                textStatus.Text += "Sending file " + files[j].Substring(indexDelimeter + 1) + Environment.NewLine;
                                all += files[j].Substring(indexDelimeter + 1) + ";";
                            }

                        }
                        all.Substring(0, all.Length - 1);
                        await sw.WriteLineAsync(all);
                        textStatus.Text += "Sending files " + Environment.NewLine;
                        String currentFile = await sr.ReadLineAsync();
                        
                        String sendingFile = directory + "\\" + currentFile;
                        textStatus.Text += "Reading file into bytes " + currentFile + Environment.NewLine;

                        byte[] fileInBytes = File.ReadAllBytes(sendingFile);
                        bw = new BinaryWriter(client.GetStream());

                        bw.Write(fileInBytes);
                        textStatus.Text += "Sending file for execution " + currentFile + Environment.NewLine;

                    }
                    finally
                    {
                        bw.Close();
                        sr.Close();
                        sw.Close();
                    }
                }
                
            }
        }

    }
}
