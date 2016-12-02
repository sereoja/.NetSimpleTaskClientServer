using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;

namespace SimpleTaskServer
{
    [Serializable]
    public class Packet
    {
        public List<string> _GeneralData;
        public int _packetInt;
        public bool _packetBool;
        public string _userName;
        public PacketType packetType;

        public Packet(PacketType type, string userName)
        {
            _GeneralData = new List<string>();
            _userName = userName;
            packetType = type;
        }

        public Packet(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            Packet p = (Packet)bf.Deserialize(ms);
            this._GeneralData = p._GeneralData;
            this._packetInt = p._packetInt;
            this._packetBool = p._packetBool;
            this._userName = p._userName;
            this.packetType = p.packetType;
            ms.Close();
        }
        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }
    }

    public enum PacketType { }
}
