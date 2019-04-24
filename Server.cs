using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ServerCS
{
    class Server
    {
        NetworkStream m_Stream = null;
        TcpListener m_Listener = new TcpListener(IPAddress.Any, 3000);
        byte[] m_Data = new byte[1500];
        IPEndPoint m_ServerEP = new IPEndPoint(IPAddress.Any, 3000);
        int m_NrOfReceivedBytes = 0;
        public void Listen()
        {
            m_Listener.Start();
            TcpClient m_Client = m_Listener.AcceptTcpClient();
            while(true)
            {
                m_Stream = m_Client.GetStream();
                m_Stream.Read(m_Data, 0, m_Data.Length);
                m_NrOfReceivedBytes = m_Data[1];
                RaiseDataReceived(new ReceivedDataArgs(m_ServerEP.Address,
                    m_ServerEP.Port, m_Data));
            }
        }

        public delegate void DataReceived(object _sender, ReceivedDataArgs _args);
        public event DataReceived DataReceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs _args)
        {
            DataReceivedEvent?.Invoke(this, _args);
        }

    }
    public class ReceivedDataArgs
    {
        public IPAddress m_IpAddress { get; set; }
        public int m_Port { get; set; }
        public byte[] m_ReceivedBytes;

        public string GetReceivedBytes()
        {
            string data="";
            for(int i =0;i<m_ReceivedBytes.Length; i++)
            {
                data += m_ReceivedBytes[i];
            }
            return data;
        }
        public ReceivedDataArgs(IPAddress _ip,int _port, byte[] _data)
        {
            m_IpAddress = _ip;
            m_Port = _port;
            m_ReceivedBytes = _data;
        }
    }
}
