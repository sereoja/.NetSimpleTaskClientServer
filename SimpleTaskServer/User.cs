using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SimpleTaskServer
{
    public class User
    {
      //  private string _firstName;
      //  private string _lastName;
        private string _userName;
        // private DateTime _DOB;
        private Socket _client;
        private Thread _userThread;
        // private IPEndPoint _userEndP;
        // private IPAddress _IPaddress;
        //// private IPHostEntry _ipHostInfor;
        // private int _cliendPort;
        // private string _clientName;
        
        public User(/*string userName,*/ Socket client/*, Thread userThread*/)
        {
            _userThread = new Thread(ServerForm1.ProcessClient);
            _userThread.Start(_client);
           // _userName = userName;
            _client = client;
           // _userThread = userThread;
            //_userEndP = (IPEndPoint)client.RemoteEndPoint;
            //_IPaddress = _userEndP.Address;
            //_cliendPort = _userEndP.Port;
            //_clientName = Dns.GetHostEntry(_IPaddress).HostName;
        }

        public string UserName { get { return _userName; } }
        public Socket Client { get { return _client; } }
        public Thread UserThread { get { return _userThread; } }
    }
}
