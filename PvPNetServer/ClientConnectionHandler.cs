using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using PvPNet;

namespace PvPNetServer
{
    public class ClientConnectionHandler : IDisposable
    {
        private readonly TcpClient client;   
        public event EventHandler ClientDataReceived;
        private Thread thread;
        private SslStream sslStream;
        private object sslStreamLockobject = new object();
        
        public string ClientId
        {
            get;
            private set;
        }

        public ClientConnectionHandler(TcpClient client)
        {
            this.client = client;
            var clientAddress = ((IPEndPoint) client.Client.RemoteEndPoint);
            this.ClientId = clientAddress.Address.ToString()+clientAddress.Port.ToString();
        }
        
        public bool IsClientConnected
        {
            get
            {
                this.SendResponse(new ServerResponse()
                {
                    Operations = ClientOperationsType.Ping
                });
                
                return this.client.Connected;
            }
        }
        
        public void Start()
        {
            this.thread = new Thread(HandleCommunications);
            thread.Start(this.client);     
        }
        
        private void HandleCommunications(object obj)
        {
            TcpClient asynClient = obj as TcpClient;

            NetworkStream stream = asynClient.GetStream();
            
            this.sslStream = new SslStream(stream, false);
            try
            {
                this.sslStream.AuthenticateAsServer(Server.serverCertificate, true, SslProtocols.Tls12, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Client cerificate bad.");
                Console.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
            
            //this.sslStream = asynClient.GetStream();

            while (true) 
            {
                ArrayList responseData = new ArrayList();

                int bytes = -1;

                lock (this.sslStreamLockobject)
                {
                    while (stream.DataAvailable)
                    {
                        var data = new Byte[2048];

                        bytes = this.sslStream.Read(data, 0, data.Length);

                        responseData.AddRange(data);
                    }
                }

                if (responseData.Count != 0)
                {
                    Console.WriteLine("Data received");
                    
                    this.OnClientDataReceived(this.TranslateMessage(responseData));
                    responseData.Clear();
                }
            }         
        }

        private ClientRequestData TranslateMessage(ArrayList message)
        {
            byte[] data = (byte[])message.ToArray(typeof(byte));
            
            ClientRequestData requestData =
                new OperationDataHandler().TranslateObject<ClientRequestData>(data) as ClientRequestData;
            
            return requestData;
        }

        public void SendResponse(ServerResponse response)
        {
            lock (this.sslStreamLockobject)
            {
                var stream = this.sslStream;
                byte[] data = new OperationDataHandler().GetBytes(response);
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        private void OnClientDataReceived(ClientRequestData dataReceived)
        {
            if (dataReceived == null)
                return;
            
            if (this.ClientDataReceived != null)
                this.ClientDataReceived(this, new ClientReadEventData(dataReceived));

        }

        public void Dispose()
        {
            if (this.sslStream != null)
            {
                this.sslStream.Close();
            }
            
            if (client != null)
            {
                this.client.Close();
                ((IDisposable) client).Dispose();
            }

            if (this.thread != null)
            {
                this.thread.Abort();
            }

        }
    }
}