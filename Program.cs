using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Server m_Server = new Server();
            HandleData m_HandleData = new HandleData();

            //starta server tråden
            Thread m_ServerThread = new Thread(() => m_Server.Listen());
            m_ServerThread.Start();

            //starta handler tråden
            Thread m_DataHandlerThread = new Thread(() =>
            m_HandleData.SubscribeToEvent(m_Server));

            m_DataHandlerThread.Start();

            while(true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
