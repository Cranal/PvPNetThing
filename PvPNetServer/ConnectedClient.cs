using System;
using System.Net.Sockets;
using PvPNet;

namespace PvPNetServer
{
    public class ConnectedClient : IDisposable
    {
        private ClientConnectionHandler client;
        
        public event EventHandler ClientMessageOccured;

        public ConnectedClient(TcpClient client)
        {
            this.client = new ClientConnectionHandler(client);
            this.client.ClientDataReceived += ClientOnClientDataReceived;
        }

        public bool IsClientConnected
        {
            get
            {
                return this.client.IsClientConnected;
            }
        }

        public void Start()
        {
            this.client.Start();
        }

        private void ClientOnClientDataReceived(object sender, EventArgs e)
        {
            var eventData = e as ClientReadEventData;
            
            this.OnClientDataReceived(new ClientMessage()
            {
                Client = this,
                Operation = eventData.Data
            });
        }
        
        public void SendResponse(ServerResponse response)
        {
            this.client.SendResponse(response);
        }

        public string ClientIp
        {
            get
            {
                return client.ClientId;
            }
        }

        public string ClientName
        {
            get; 
            set;
        }
        
        private void OnClientDataReceived(ClientMessage dataReceived)
        {
            Console.WriteLine("Client data received. User " + this.ClientName);
            if (this.ClientMessageOccured != null)
                this.ClientMessageOccured(this, new ClientOperationMessageEventData(dataReceived));
            else
                Console.WriteLine("No subscription");
        }

        public void Dispose()
        {
            if (client != null) 
                client.Dispose();
        }
    }
}