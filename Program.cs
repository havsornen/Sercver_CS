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
        private static string[] m_ImageListOne = new string[100000];
        private static string[] m_ImageListTwo = new string[100000];
        private static string[] m_ImageListOutputTestOne = new string[100000];
        private static string[] m_ImageListOutputTestTwo = new string[100000];
        private static int m_NrOfClientsConnected = 0;
        private static string[] m_ClientIpAddress = new string[2];
        private static string[] m_ClientIpComparisonString = new string[2];
        private static WebSocketSession[] m_WsSessions = new WebSocketSession[10];

        static void Main(string[] args)
        {
            //initialise array that will be handling the image data
            for(int i =0;i<100000;i++)
            {
                m_ImageListOne[i] = "";
                m_ImageListTwo[i] = "";
                m_ImageListOutputTestOne[i] = "";
                m_ImageListOutputTestTwo[i] = "";
            }
            for (int i =0;i<2;i++)
            {
                m_ClientIpAddress[i] = "";
                m_ClientIpComparisonString[i] = "";
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
            while (true) { };
          
        }

        
        private static void PackageBytesAndSend(WebSocketSession _Session,string _Data,int _sessionId)
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
            m_WsSessions[m_NrOfClientsConnected] = _Session;
            m_ClientIpAddress[m_NrOfClientsConnected++] = _Session.RemoteEndPoint.ToString();
            Console.WriteLine("New user connected: " + _Session.RemoteEndPoint+ "Session ID "+_Session.SessionID.ToString());

        }
        private static void WsServer_NewMessageReceived(WebSocketSession _Session, string _Value)
        {
            string _sessionId = _Value.Substring(0,1);
            int _sessionIdInt = Convert.ToInt32(_sessionId);
            string _imageNrString = _Value.Substring(3, 7);
            string _RemoteClient = _Session.RemoteEndPoint.ToString();
            int _imageNrInt = Convert.ToInt32(_imageNrString);
            string _imagevalue = _Value.Substring(9, _Value.Length - 9);
            if (_Session.RemoteEndPoint.ToString() == m_ClientIpAddress[0])
            {
                m_ImageListTwo[_imageNrInt] += _imageNrInt + "_" + _RemoteClient + ';' + _imagevalue + ',';
                m_ImageListOutputTestOne[_imageNrInt] += _imagevalue;
            }
            else
            {
                m_ImageListOne[_imageNrInt] += _imageNrInt + "_" + _RemoteClient + ';' + _imagevalue + ',';
                m_ImageListOutputTestTwo[_imageNrInt] += _imagevalue;
            }
            if (_imagevalue.Length<1000&&_Value!="")
            {
                if(_Session.RemoteEndPoint.ToString()==m_ClientIpAddress[0])
                {
                    PackageBytesAndSend(m_WsSessions[1], m_ImageListOne[_imageNrInt], _sessionIdInt);
                    Console.WriteLine(m_ImageListOutputTestOne[_imageNrInt] + "                    " +
                    "                     " +
                    "DEBUG IMAGE NR" +
                   m_DebugImageNr + "Client ip: " + _Session.RemoteEndPoint + "SESSION ID " + _Session.SessionID.ToString());
                    m_DebugImageNr++;
                }
                else
                {
                    PackageBytesAndSend(m_WsSessions[0], m_ImageListTwo[_imageNrInt], _sessionIdInt);
                    Console.WriteLine(m_ImageListOutputTestTwo[_imageNrInt] + "                    " +
                    "                     " +
                    "DEBUG IMAGE NR" +
                   m_DebugImageNr + "Client ip: " + _Session.RemoteEndPoint + "SESSION ID " + _Session.SessionID.ToString());
                    m_DebugImageNr++;
                }

                
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
                if(m_WsSessions[i].ToString()==_session.SessionID)
                {
                    m_WsSessions[i] = m_WsSessions[m_NrOfClientsConnected - 1];
                    WebSocketSession[] tempArr = new WebSocketSession[m_NrOfClientsConnected];
                    for(int x =0;x<m_NrOfClientsConnected-1;x++)
                    {
                        tempArr[x] = m_WsSessions[x];
                    }
                    m_WsSessions = tempArr;
                }
            }
            m_NrOfClientsConnected--;
        }
    }
}
