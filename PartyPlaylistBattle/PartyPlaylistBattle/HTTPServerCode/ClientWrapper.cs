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

namespace PartyPlaylistBattle.HTTPServerCode
{
    public class ClientWrapper
    {
        
        private TcpClient wrapperClient;
        public ClientWrapper()
        {
            this.wrapperClient = new TcpClient();
        }
        public ClientWrapper(TcpClient client)
        {
            this.wrapperClient = client;
        }
        public Stream GetStream()
        {
            return wrapperClient.GetStream();
        }
        public void Close()
        {
            this.wrapperClient.Close();
        }
    }
}
