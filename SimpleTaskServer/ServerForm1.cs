using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace SimpleTaskServer
{
    public partial class ServerForm1 : Form
    {
        public ServerForm1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayFilesInDebugFolder();

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetIPV4(ipHostInfo.AddressList);

           // label1.Text = ipAddress.ToString();

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            label1.Text = String.Format("Server Listening at ip address {0}:{1}", ipAddress, 11000);
          
            Task.Factory.StartNew(()=> StartListening2(listener));
        }
        //public void StartListening(Socket listener)
        //{
        //    string data = null;
        //    // Data buffer for incoming data.
        //    byte[] bytes = new Byte[1024];

        //    try
        //    {

        //        while (true)
        //        {

        //            // Program is suspended while waiting for an incoming connection.
        //            SetText("Waiting for the next request...");
        //            Socket handler = listener.Accept();

        //            //display client data
        //            DisplayClientInfo(handler);
        //            data = null;

        //            // An incoming connection needs to be processed.
        //            //RECEIVE: request from client
        //            //********
        //            while (true)
        //            {
        //                bytes = new byte[128];
        //                int bytesRec = handler.Receive(bytes);
        //                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
        //                if (data.IndexOf("<EOF>") > -1)
        //                {
        //                    break;
        //                }
        //            }
        //            //display client request               
        //            SetText(String.Format("Text received : {0}", data));
        //            SetText("");


        //            //SEND: response
        //            //*****
        //            // Echo the data back to the client.
        //            byte[] msg = Encoding.ASCII.GetBytes(data);

        //            handler.Send(msg);
        //            handler.Shutdown(SocketShutdown.Both);
        //            handler.Close();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }



        //}//end of StartListening() method

        string executableLocation = String.Empty;
        ConcurrentBag<string> fileNameBag;
        private void DisplayFilesInDebugFolder() {
           
            string[] filePaths;
            executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            fileNameBag = new ConcurrentBag<string>();
            try
            {
                filePaths = Directory.GetFiles(executableLocation, "*");
                filePaths = filePaths.Distinct().ToArray();
                
                SetText("Files Found:");
                foreach (string v in filePaths)
                {
                    SetText(Path.GetFileName(v));
                }


                Parallel.For(0, filePaths.Length, i =>
                {
                    fileNameBag.Add(Path.GetFileName(filePaths[i]));
                });

                SetText("\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("The process failed: {0}", ex.ToString());
            }
        }
        private void StartListening2(Socket listener)
        {
          
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            try
            {

                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.
                    SetText("Handling next request...");
                    Socket handler = listener.Accept();

                    Task.Factory.StartNew(() =>
                    {
                        Socket client = handler; //cache it
                        //display client data
                        DisplayClientInfo(client);

                        ProcessClient(client);

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }//end of StartListening2
        //cross threading
        private void SetText(string text)
        {
            if (listBox1.InvokeRequired)
            {
                Action<string> action = SetText;
                this.Invoke(action, text);
            }
            else
            {
                listBox1.Items.Add(text);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        //helper method
        private IPAddress GetIPV4(IPAddress[] ipaddresses)
        {
            foreach(IPAddress address in ipaddresses)
            {
                if (address.GetAddressBytes().Length == 4)
                    return address;
            }
            return null;
        }
        private void DisplayClientInfo(Socket client)
        {
            //get the client endpoint using the RemoteEndPoint property
            IPEndPoint clientEndP = (IPEndPoint)client.RemoteEndPoint;
            //now we can extract its IP Address and port number
            IPAddress clientIPAddress = clientEndP.Address;
            //client port
            int clientPort = clientEndP.Port;
            //get the client domain name
            string clientName = Dns.GetHostEntry(clientIPAddress).HostName;
            //save to a file or display 
            SetText(String.Format("\nClient Request from {0} @ {1}:{2}",
                            clientName, clientIPAddress, clientPort));
        }

   
        //method to process a client, this method will run in a Task
        private void ProcessClient(Socket client)
        {
            //RECEIVE CLIENT REQUEST
            string request = String.Empty;
            byte[] bytes;
            //get client request
            while (true)
            {
                bytes = new byte[128];
                int bytesRec = client.Receive(bytes);
                request += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (request.IndexOf("<EOF>") > -1)
                    break;
            }
            //--------------------------------------------------------------------------
            //PROCESS CLIENT REQUEST
            string fileName = String.Empty;
            string response = String.Empty;
            request.ToLower();
            request.Trim();
            request = request.Remove(request.Length - 5);
            if (request.IndexOf("get") == 0)
            {
                fileName = request.Split(' ')[1];
                foreach (string file in fileNameBag)
                {
                if (file.ToLower() == fileName.ToLower())
                {
                    response = File.ReadAllText(executableLocation + "/" + fileName);
                }
                
                }
                if (response == String.Empty)
                {
                    if (fileName == "filenames")
                    {
                        response = "<List>";
                        foreach (string item in fileNameBag)
                        {
                            response += item + "\n";
                        }
                        
                    }else
                    response = "Error: File Not Found";
                }
            }
            else
            {
                response = "Error: Ivalid Command \nGet fileName.fileExtenison";
            }
            
           
            //display client request               
            SetText(String.Format("Text received : {0}", response));
            SetText("");
            //TO BE COMPLETED


            //-----------------------------------------------------------------------------
            //SEND RESPONSE BACK TO CLIENT
            response = response + "<EOF>";
            byte[] buffer = Encoding.ASCII.GetBytes(response);
            client.Send(buffer);
        }

        private void btnDisplayFilesinDebug_Click(object sender, EventArgs e)
        {
            DisplayFilesInDebugFolder();
        }
    }

}
