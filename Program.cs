using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SuperWebSocket;
using System.Linq;

namespace ServerCS
{
    class Program
    {
        static int m_DebugImageNr = 0;
        private static WebSocketServer m_wsServer;
        private static string[] m_ImageList = new string[100000];
        private static string[] m_ImageListOutputTest = new string[100000];
        
        private static string[] m_ClientFeedList = new string[20];
        private static int m_NrOfClientsConnected = 0;
        private static WebSocketSession[] m_WsSessions = new WebSocketSession[10];

        static void Main(string[] args)
        {
            //initialise array that will be handling the image data
            for(int i =0;i<100000;i++)
            {
                m_ImageList[i] = "";
                m_ImageListOutputTest[i] = "";
            }
            m_wsServer = new WebSocketServer();

            var m_ServerConfig = new SuperSocket.SocketBase.Config.ServerConfig();
            m_ServerConfig.MaxConnectionNumber = 100;
            var m_List = new List<SuperSocket.SocketBase.Config.ListenerConfig>(10);
            int port = 3000;
            //Initialise ports, these will be used to handle different sessions
            for (int i = 0; i < 10; i++)
            {
                var listener = new SuperSocket.SocketBase.Config.ListenerConfig();
                listener.Port = port;
                port++;
                listener.Backlog = 1000;
                listener.Ip = "Any";
                listener.Security = "None";

                m_List.Add(listener);
            }
            //Setup the server and subscribe the server to the various events
            m_ServerConfig.Listeners = m_List;
            m_wsServer.Setup(m_ServerConfig);
            m_wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            m_wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            m_wsServer.NewDataReceived += WsServer_NewDataReceived;
            m_wsServer.SessionClosed += WsServer_SessionClosed;
            m_wsServer.Start();
            Console.ReadKey();
          
        }

        
        private static void PackageBytesAndSend(WebSocketSession _Session,string _Data,int _sessionId)
        {
            string[] _Payload = new string[_Data.Length/1000];
            _Payload = _Data.Split(',');
            foreach (string _SplitPayload in _Payload)
            {
                m_WsSessions[_sessionId].Send(_SplitPayload);
            }
        }
        private static void WsServer_NewSessionConnected(WebSocketSession _Session)
        {
           m_ClientFeedList[m_NrOfClientsConnected] += _Session.RemoteEndPoint;
            m_NrOfClientsConnected++;
            m_WsSessions[1] = _Session;
            Console.WriteLine("New user connected: " + _Session.RemoteEndPoint);

        }
        private static void WsServer_NewMessageReceived(WebSocketSession _Session, string _Value)
        {
            string _sessionId = _Value.Substring(0,1);
            int _sessionIdInt = Convert.ToInt32(_sessionId);
            string _imageNrString = _Value.Substring(3, 7);
            string _RemoteClient = _Session.RemoteEndPoint.ToString();
            int _imageNrInt = Convert.ToInt32(_imageNrString);
            string _imagevalue = _Value.Substring(9, _Value.Length - 9);
            m_ImageList[_imageNrInt] += _imageNrInt+"_"+ _RemoteClient+';'+_imagevalue+',';

            if (_imagevalue.Length<1000&&_Value!="")
            {
                PackageBytesAndSend(_Session, m_ImageList[_imageNrInt],_sessionIdInt);
                Console.WriteLine(m_ImageList[_imageNrInt] + "                    " +
                    "                     " +
                    "DEBUG IMAGE NR" +
                   m_DebugImageNr + "Client ip: " + _Session.RemoteEndPoint);
                m_DebugImageNr++;
            }

        }
        private static void WsServer_NewDataReceived(WebSocketSession _Session, byte[] _Value)
        {
        }
        private static void WsServer_SessionClosed(WebSocketSession _session, SuperSocket.SocketBase.CloseReason _value)
        {
            _session.Send("Your session has now been closed, thanks for participating!"+ "The reason for the stream closing is the following: "+_value.ToString());
            
            for(int i =0;i<m_NrOfClientsConnected;i++)
            {
                if(m_ClientFeedList[i]==_session.RemoteEndPoint.ToString())
                {
                    m_ClientFeedList[i] = m_ClientFeedList[m_NrOfClientsConnected - 1];
                    m_ClientFeedList[m_NrOfClientsConnected - 1] = "";
                }
            }
            m_NrOfClientsConnected--;
        }
    }
}
