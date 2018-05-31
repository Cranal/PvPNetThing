using System;
using System.Net.Sockets;
using System.Threading;

namespace PvPNetServer
{
    public class ClientHandler
    {
        public event EventHandler ClientDataReceived;
        
        private TcpClient client;

        public int Id 
        { 
            get; 
            set; 
        }

        public ClientHandler(TcpClient client, int id)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            this.client = client;
            this.Id = id;
        }

        public void Start()
        {
            Thread thread = new Thread(HandleCommunications);
            thread.Start(this.client);
        }

        private void HandleCommunications(object obj)
        {
            TcpClient asynClient = obj as TcpClient;

            var stream = asynClient.GetStream();

            while (true)
            {
                if (stream.DataAvailable)
                {
                    var data = new Byte[256];
                    String responseData = String.Empty;

                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    
                    this.OnClientDataReceived(responseData);
                }
            }         
        }

        protected void OnClientDataReceived(string dataReceived)
        {
            if (this.ClientDataReceived != null)
                this.ClientDataReceived(this, new ClientReadEventData(dataReceived+this.Id));
        }
    }
}