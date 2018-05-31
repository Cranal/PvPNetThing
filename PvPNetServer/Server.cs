using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using PvPNet;

namespace PvPNetServer
{
    public class Server
    {
        object lockObject = new object();
        public static X509Certificate serverCertificate = null;
        
        private Hashtable ClientsList
        {
            get; 
            set;
        }
        
        private Queue Messages
        {
            get; 
            set;
        }
        
        private BattleManager BattleManager
        {
            get; 
            set;
        }

        public void Start()
        {
            this.Messages = new Queue();
            Console.WriteLine("Message  queue initialized.");
            
            IPAddress ipAddress = Dns.Resolve(Dns.GetHostName()).AddressList[0];
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 11000);

            serverCertificate = X509Certificate.CreateFromCertFile("TempCert.cer");
            Console.WriteLine("Certificate loaded.");
            
            this.ClientsList = new Hashtable();
            Console.WriteLine("Client list initialized.");
            
            this.BattleManager = new BattleManager();
            this.BattleManager.BattleFoundEvent += BattleManagerOnBattleFoundEvent;
            Console.WriteLine("Battle manager initialized.");

            try
            {
                TcpListener tcpListener = new TcpListener(ipLocalEndPoint);
                tcpListener.Start();
                Console.WriteLine("Server started");
                Console.WriteLine(string.Format("ID:{0}", ipAddress.ToString()));
                
                Thread userOnlineMonitor = new Thread(this.MonitorUserOnline);
                userOnlineMonitor.Start();
                
                Thread messageProcessor = new Thread(this.ProcessMessageQueue);
                messageProcessor.Start();

                while (true)
                {
                    if (tcpListener.Pending())
                    {
                        this.AcceptData(tcpListener);
                    }
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.ToString());
            }
        }

        private void MonitorUserOnline()
        {
            Dictionary<object, ConnectedClient> disconnectedClients = new Dictionary<object, ConnectedClient>();
            while (true)
            {              
                Thread.Sleep(5000);

                lock (this.lockObject)
                {
                    foreach (DictionaryEntry client in this.ClientsList)
                    {
                        if (!(client.Value as ConnectedClient).IsClientConnected)
                        {
                            disconnectedClients.Add(client.Key, (client.Value as ConnectedClient));
                        }
                    }


                    if (disconnectedClients.Any())
                    {
                        //
                        // First end active battles if any.
                        //
                        disconnectedClients.ToList().ForEach(c => this.BattleManager.DisconnectUserFromBattle(c.Value));
                        disconnectedClients.ToList().ForEach(x => this.ClientsList.Remove(x.Key));
                        Console.WriteLine("Clients removed");
                    }
                }

                disconnectedClients.Clear();
            }
        }
        
        private void BattleManagerOnBattleFoundEvent(object sender, BattleCreatedEventData e)
        {
            var combatant = sender as ConnectedClient;
            combatant.SendResponse(new ServerResponse()
            {
                IsSuccess = true,
                Operations = ClientOperationsType.BattleFound,
                ResponseData = new OperationDataHandler().GetBytes(e.BattleData)
            });
        }

        private void ProcessMessageQueue()
        {
            while (true)
            {
                if (this.Messages.Count != 0)
                {
                    Console.WriteLine("Processing messages");
                    var message = this.Messages.Dequeue() as ClientMessage;

                    switch (message.Operation.Operation)
                    {
                        case ClientOperationsType.Login:
                            this.PerformLogin(message);
                            break;
                        case ClientOperationsType.CreateUser:
                            this.CreateUser(message);
                            break;
                        case ClientOperationsType.FindBattle:
                            this.BattleManager.FindBattle(message.Client);
                            break;
                        case ClientOperationsType.SendBattleTurn:
                            this.BattleManager.ProcessBattleMove(message);
                            break;
                    }
                }
            }
        }

        private void CreateUser(ClientMessage message)
        {
            Console.WriteLine("Creating user");
            LoginData data = new OperationDataHandler().TranslateObject<LoginData>(message.Operation.OperationData) as LoginData;
            
            bool result = false;

            DbHandler handler = new DbHandler();

            byte[] passwordHash = new Encryption().EncryptSHA(data.Password, data.Login);

            result = handler.CreateUser(data.Login, passwordHash);
            
            if (result)
                message.Client.ClientName = data.Login;

            message.Client.SendResponse(new ServerResponse()
            {
                Operations = message.Operation.Operation,
                IsSuccess = result,
                ResponseData = new OperationDataHandler().GetBytes(message.Client.ClientName)
            });
            Console.WriteLine("User created");
        }

        private void PerformLogin(ClientMessage message)
        {
            Console.WriteLine("Loggining user");
            
            LoginData data = new OperationDataHandler().TranslateObject<LoginData>(message.Operation.OperationData) as LoginData;

            bool result = false;

            DbHandler handler = new DbHandler();

            result = handler.CheckExistingUser(data.Login, data.Password);

            result = result && this.ClientsList.Values.Cast<ConnectedClient>().All(c => c.ClientName != data.Login);
            
            if (result)
            {
                message.Client.ClientName = data.Login;
            }

            message.Client.SendResponse(new ServerResponse()
            {
                Operations = message.Operation.Operation,
                IsSuccess = result,
                ResponseData = new OperationDataHandler().GetBytes(data.Login)
            });
            
            Console.WriteLine("User logedin");
        }

        private void AcceptData(TcpListener tcpListener)
        {
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptDataAsync), tcpListener);
        }

        private void AcceptDataAsync(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener) ar.AsyncState;

            // End the operation and display the received data on 
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            lock (this.lockObject)
            {
                if (!this.ClientsList.ContainsKey(((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString()))
                {
                    ConnectedClient connectedClient = new ConnectedClient(client);
                    this.ClientsList.Add(connectedClient.ClientIp, connectedClient);

                    connectedClient.ClientMessageOccured += this.ConnectedClientOnClientMessageOccured;
                    connectedClient.Start();

                    Console.WriteLine("Connection received");
                }
                else
                {
                    Console.WriteLine("Client already exists");
                }
            }
        }

        private void ConnectedClientOnClientMessageOccured(object sender, EventArgs e)
        {
            var messageData = e as ClientOperationMessageEventData;
            
            this.Messages.Enqueue(messageData.Data);
            
            Console.WriteLine("Message added");
        }
    }
}