//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ServerCS
//{
//    class HandleData
//    {
//        int m_NrOfImages = 0;
//        List<Byte[]> m_ImageList;
//        string[] somestring = new string[4];
//        int i = 0;
//        public void SubscribeToEvent(Server _server)
//        {
//            _server.DataReceivedEvent += ServerDataReceivedEvent;
//        }
//        void ServerDataReceivedEvent(object sender,ReceivedDataArgs args)
//        {
            
//            //jobba här inne för att hantera data
//            Console.WriteLine("Received data from" + args.m_IpAddress+" "+Convert.ToBase64String(args.GetReceivedBytes()));
//            //somestring[i] = Encoding.ASCII.GetString(args.GetReceivedBytes());
//            //Console.WriteLine("BYTE ARR: " + somestring[i]+"\r\n");
//            Console.WriteLine();
//            i++;
//        }
//    }
//}
