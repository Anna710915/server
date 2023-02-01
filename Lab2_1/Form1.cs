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
        List<TcpClient> clients = new List<TcpClient>();
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
           
           // clients.ForEach(c => c.Close());
            textStatus.Text += clients.Count();
            if(server != null)
            {
                server.Stop();
                textStatus.Text += "Server stoped " + Environment.NewLine;
                server = null;
            } 
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ipAddress = IPAddress.Parse(textHost.Text);
            } catch
            {
                textStatus.Text += "Invalid address: " + textHost.Text + Environment.NewLine;
            }
            
            try
            {
                port = int.Parse(textPort.Text);
            } catch
            {
                textStatus.Text += "Invalid port: " + textPort.Text + Environment.NewLine;
            }
            
            if(ipAddress != null && port != 0)
            {
                
                if(server == null)
                {
                    // TcpListener server = new TcpListener(port);
                    server = new TcpListener(ipAddress, port);
                    // Start listening for client requests.
                    server.Start();
                    textStatus.Text += "Server started" + Environment.NewLine;
                    //Enter the listening loop.
                    while (true)
                    {
                        TcpClient client = null;
                        if(client == null || !client.Connected)
                        {
                            try
                            {
                                if (server != null)
                                {
                                    textStatus.Text += "Waiting for a connection... " + Environment.NewLine;
                                    client = await server.AcceptTcpClientAsync();
                                    textStatus.Text += "Connected!" + Environment.NewLine;
                                    // clients.Add(client);
                                }
                                else
                                {
                                    textStatus.Text += "Server shutdown... " + Environment.NewLine;
                                }

                            }
                            catch (Exception ex)
                            {
                                textStatus.Text += "Server shutdown... " + Environment.NewLine;
                                break;
                            }
                        }


                        sr = new StreamReader(client.GetStream());
                        sw = new StreamWriter(client.GetStream());
                        sw.AutoFlush = true;
                        while (client != null && client.Connected)
                        {
                            try
                            {
                                // Get a stream object for reading and writing
                                // Buffer for reading data

                                String currentFile = null;
                                while (true)
                                {
                                    string data = await sr.ReadLineAsync();

                                    if (data != "true")
                                    {
                                        currentFile = data;
                                        break;
                                    }
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
                                }

                                if (currentFile != null)
                                {


                                    textStatus.Text += "Current file " + currentFile + Environment.NewLine;
                                    String sendingFile = Directory.GetCurrentDirectory() + "\\exe\\" + currentFile;

                                    NetworkStream stream = client.GetStream();
                                    byte[] fileInBytes = File.ReadAllBytes(sendingFile);

                                    textStatus.Text += "Reading file into bytes " + fileInBytes.Length + Environment.NewLine;

                                    await stream.WriteAsync(fileInBytes, 0, fileInBytes.Length);


                                    textStatus.Text += "Sending file for execution " + currentFile + Environment.NewLine;

                                    stream.Flush();

                                }

                            } catch (Exception ex)
                            {
                                textStatus.Text += "Error " + ex.Message + Environment.NewLine;
                            } finally
                            {
                                sr.Close();
                                sw.Close();
                            }

                        }

                    }
                }
                
            }
            
        }

    }
}
