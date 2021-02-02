using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Net.Security;

namespace PartyPlaylistBattle.HTTPServer
{
    public class ThisTcpClient
    {
        public TcpClient myclient;
        public ThisTcpClient()
        {
            myclient = new TcpClient();
        }

        public ThisTcpClient(TcpClient client)
        {
            myclient = client;
        }

        public StreamReader GetStreamReader()
        {
            return new StreamReader(myclient.GetStream());
        }

        public StreamWriter GetStreamWriter()
        {
            return new StreamWriter(myclient.GetStream()) { AutoFlush = true };
        }

        public void End()
        {
            myclient.Close();
        }
    }
}
