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
using System.Threading;
using System.IO;
using System.Reflection;

namespace SimpleTaskClient
{
    public partial class ClientForm1 : Form
    {
        public ClientForm1()
        {
            InitializeComponent();
            txtServerIP.Text = "192.168.1.68";
            txtServerPort.Text = "11000";
            rtbRequest.Text = "to receive a text of a file please type command:\nGet filename.format \nExample:\nGet names.txt";

        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            StartClient();
        }
        public void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];
            Socket sender = null;

            // Connect to a remote device.
            try
            {
              

                IPAddress ipAddress = IPAddress.Parse(txtServerIP.Text);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
               
                    sender.Connect(remoteEP);

                // SEND the data through the socket.
                // Encode the data string into a byte array.
                string message = rtbRequest.Text;
                message.Trim();
                string fileName = String.Empty;
                if (message.Length > 3)
                {
                    if (message[3] == ' ')
                    {
                        fileName = message.Split(' ')[1];
                    }
                    else message = "Error";
                }
                else message = "Error";
               
                
              
                //add an <EOF> to it
                message += "<EOF>";

                    //byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    byte[] msg = Encoding.ASCII.GetBytes(message);

                   
                    int bytesSent = sender.Send(msg);


                    // RECEIVE the response from the remote device.
                    //  int bytesRec = sender.Receive(bytes);

                    string data = String.Empty;

                    while (true)
                    {
                        bytes = new byte[128];
                        int bytesRec = sender.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                        data = data.Remove(data.Length - 5);
                        break;
                        }
                    }
                if (data.IndexOf("Error:") > -1)
                {
                    rtbRequest.Text = data;
                }
                else if (data.IndexOf("<List>") > -1)
                {
                    richTextBox1.Text = data;
                }
                else
                {
                    rtbResponse.Text = data;
                    File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + fileName, data);
                }



            }
                catch(FormatException fe)
                {
                     MessageBox.Show(String.Format("FormatException : {0}", fe.ToString()));
                }
                catch (ArgumentNullException ane)
                {
                    MessageBox.Show(String.Format("ArgumentNullException : {0}", ane.ToString()));
                }
                catch (SocketException se)
                {
                MessageBox.Show(String.Format("SocketException : {0}", se.ToString()));
                }
                catch (Exception e)
                {
                MessageBox.Show(String.Format("Unexpected exception : {0}", e.ToString()));
                }
            finally
            {
                if(sender != null)
                {
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }

            
            
        }

        private void rtbRequest_MouseClick(object sender, MouseEventArgs e)
        {
            rtbRequest.Clear();
        }
    }
}
//Lab Assignemt
//Write a client server application

//client request content of a filename 
//by sending the server the name of the filename
//request could be like: Get filename

//the server looks into its folder where it keeps all of its text files, 
//opens the file, reads it and send back the text to the client

//The client receives the text, creates a text file within its folder and write the text to the file