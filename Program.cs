using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web.WebSockets;
using System.Net.WebSockets;
using SuperWebSocket;
using System.Linq;

namespace ServerCS
{
    class Program
    {
        private static WebSocketServer m_wsServer;
        private static string[] m_ImageList = new string[100000];
        private static string[] m_ClientFeedList = new string[20];
        private static int m_NrOfClientsConnected = 0;
        static void Main(string[] args)
        {
            for(int i =0;i<100000;i++)
            {
                m_ImageList[i] = "";
            }
            m_wsServer = new WebSocketServer();
            m_wsServer.Setup(3000);
            m_wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            m_wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            m_wsServer.NewDataReceived += WsServer_NewDataReceived;
            m_wsServer.SessionClosed += WsServer_SessionClosed;
            m_wsServer.Start();
           
            Console.ReadKey();
          
        }
        
        private static void PackageBytesAndSend(WebSocketSession _Session,string _Data)
        {
            string[] _Payload = new string[_Data.Length/1000];
            _Payload = _Data.Split(',');
            foreach (string _SplitPayload in _Payload)
            {
                _Session.Send(_SplitPayload);
            }
           
        }
        private static void WsServer_NewSessionConnected(WebSocketSession _Session)
        {
            m_ClientFeedList[m_NrOfClientsConnected] += _Session.RemoteEndPoint;
            m_NrOfClientsConnected++;
        }
        private static void WsServer_NewMessageReceived(WebSocketSession _Session, string _Value)
        {
            string _imageNrString = _Value.Substring(1, 7);
            string _RemoteClient = _Session.RemoteEndPoint.ToString();
            int _imageNrInt = Convert.ToInt32(_imageNrString);
            string _imagevalue = _Value.Substring(8, _Value.Length - 8);
            m_ImageList[_imageNrInt] += _RemoteClient+';'+_imagevalue+',';

            if (_imagevalue.Length<1000)
            {
                PackageBytesAndSend(_Session, m_ImageList[_imageNrInt]);
                Console.WriteLine(m_ImageList[_imageNrInt]);

            }

        }
        private static void WsServer_NewDataReceived(WebSocketSession _Session, byte[] _Value)
        {
        }
        private static void WsServer_SessionClosed(WebSocketSession _session, SuperSocket.SocketBase.CloseReason _value)
        {
            _session.Send("Stream is now closed");
        }
    }
}
