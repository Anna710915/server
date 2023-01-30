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

        private void startServer()
        {
            //try
            //{
            //    // Set the TcpListener on port 13000.
            //    int port = 13000;
            //    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            //    // TcpListener server = new TcpListener(port);
            //    server = new TcpListener(localAddr, port);

            //    // Start listening for client requests.
            //    server.Start();

            //    // Enter the listening loop.
            //    while (true)
            //    {
            //        Console.Write("Waiting for a connection... ");

            //        // Perform a blocking call to accept requests.
            //        // You could also use server.AcceptSocket() here.
            //        using TcpClient client = server.AcceptTcpClient();
            //        Console.WriteLine("Connected!");

            //        data = null;

            //        // Get a stream object for reading and writing
            //        NetworkStream stream = client.GetStream();

            //        int i;

            //        // Loop to receive all the data sent by the client.
            //        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            //        {
            //            // Translate data bytes to a ASCII string.
            //            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            //            Console.WriteLine("Received: {0}", data);

            //            // Process the data sent by the client.
            //            data = data.ToUpper();

            //            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

            //            // Send back a response.
            //            stream.Write(msg, 0, msg.Length);
            //            Console.WriteLine("Sent: {0}", data);
            //        }
            //    }
            //}
            //catch (SocketException e)
            //{
            //    Console.WriteLine("SocketException: {0}", e);
            //}
            //finally
            //{
            //    server.Stop();
            //}
            //// Устанавливаем для сокета конечную точку
            //IPHostEntry ipHost = Dns.GetHostEntry("192.168.43.57");
            //IPAddress ipAddr = ipHost.AddressList[0];
            ////IPAddress ipAddr = IPAddress.Any;
            //IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            //// Создаем сокет Tcp/Ip
            //Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //// Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            //try
            //{
            //    sListener.Bind(ipEndPoint);
            //    sListener.Listen(10);

            //    // Начинаем слушать соединения
            //    while (true)
            //    {

            //        // Программа приостанавливается, ожидая входящее соединение
            //        Socket handler = sListener.Accept();
            //        string data = null;

            //        // Мы дождались клиента, пытающегося с нами соединиться

            //        byte[] bytes = new byte[1024];
            //        int bytesRec = handler.Receive(bytes);

            //        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);


            //        // Показываем данные на консоли
            //        this.label1.Invoke((MethodInvoker)(() => this.label1.Text = "Полученный текст: " + data + "\n\n"));




            //       string s = "Спасибо за запрос в " + data.Length.ToString()
            //                + " символов";

            //        byte[] msg = Encoding.UTF8.GetBytes(s);
            //        handler.Send(msg);

            //        //if (data.IndexOf("<TheEnd>") > -1)
            //        //{
            //        //    this.label1.BeginInvoke((MethodInvoker)(() => this.label1.Text = "Сервер завершил соединение с клиентом."));
                       
            //        //    break;
            //        //}

            //        handler.Shutdown(SocketShutdown.Both);
            //        handler.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    this.label1.Invoke((MethodInvoker)(() => this.label1.Text = ex.Message));

            //}
            //finally
            //{
            //    this.label1.Invoke((MethodInvoker)(() => this.label1.Text = "Error"));

            //}
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
            server.Stop();
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
                textStatus.Text += "Waiting for a connection... " + Environment.NewLine;

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = await server.AcceptTcpClientAsync();
               

                if (client.Connected) {
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
                            int indexDelimeter = files[j].LastIndexOf('\u005C');
                            textStatus.Text += "Sending file " + files[j].Substring(indexDelimeter + 1) + Environment.NewLine;
                            all += files[j].Substring(indexDelimeter + 1) + ";";
                        }
                        all.Substring(0, all.Length - 1);
                        sw.WriteLine(all);
                    } finally
                    {
                        sr.Close();
                        sw.Close();
                    }
                    
                    textStatus.Text += "Sending files " + Environment.NewLine;
                }

                    
               



                
            }
        }

    }
}
