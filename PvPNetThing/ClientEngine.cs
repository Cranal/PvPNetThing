using System;
using System.Collections;
using System.ComponentModel;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using PvPNet;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class ClientEngine : INotifyPropertyChanged, IDisposable
    {
        private TcpClient client;
        object lockObject = new object();

        private LoginStatus loginStatus;
        private BattleStatus battleStatus;
        private Thread listener;

        public event EventHandler<BattleUpdateEventArgs> BattleStatusChanged;

        private SslStream sslStream;

        public ClientEngine()
        {
            this.LoginStatus = LoginStatus.NotLoggedId;
            this.BattleStatus = BattleStatus.NoBattle;
        }
        
        private SslStream SslStream
        {
            get
            {
                //return this.Client.GetStream();
                lock (this.lockObject)
                {
                    if (this.sslStream == null)
                    {
                        this.sslStream = new SslStream(
                            this.Client.GetStream(),
                            false,
                            (sender, cert, chain, err) => true,
                            null);

                        try
                        {
                            //this.sslStream.AuthenticateAsClient("PvPNetThing");
                            X509Certificate2Collection xc = new X509Certificate2Collection();
                            xc.Add(new X509Certificate("TempCert.cer"));
                            this.sslStream.AuthenticateAsClient("PvPNetThing", xc, SslProtocols.Tls12, false);
                        }
                        catch (AuthenticationException e)
                        {
                            Console.WriteLine("Exception: {0}", e.Message);
                            if (e.InnerException != null)
                            {
                                Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                            }

                            Console.WriteLine("Authentication failed - closing the connection.");
                            client.Close();
                            return null;
                        }
                    }
                }

               return this.sslStream;
            }
        }
        
        public TcpClient Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new TcpClient("PvpNetThing", 11000);

                    //
                    // Create listener thread.
                    //
                    this.listener = new Thread(this.HandleServerResponse);
                    listener.Start();
                }
                
                return this.client;
            }
        }
        
        public string ClientName
        {
            get; 
            private set;  
        }

        public Guid BattleId
        {
            get; 
            set;
        }

        private void HandleServerResponse()
        {
            ArrayList responseData = new ArrayList();
            while (true)
            {
                lock (this.lockObject)
                {
                    Int32 bytes = -1;
                    while (this.Client.GetStream().DataAvailable)
                    {
                        var data = new Byte[2048];

                        bytes = this.SslStream.Read(data, 0, data.Length);
                        responseData.AddRange(data);
                    }
                }

                if (responseData.Count != 0)
                {
                    ServerResponse response = new OperationDataHandler()
                        .TranslateObject<ServerResponse>((byte[])responseData.ToArray(typeof(byte)));
                    this.HandleOperationResult(response);
                    responseData.Clear();
                }
            }
        }

        private void HandleOperationResult(ServerResponse response)
        {
            switch (response.Operations)
            {
                case ClientOperationsType.Login:
                    if (response.IsSuccess)
                    {
                        this.LoginStatus = LoginStatus.LoggedIn;
                        this.ClientName = new OperationDataHandler().TranslateObject<string>(response.ResponseData);
                    }
                    else
                        this.LoginStatus = LoginStatus.LoginError;
                    break;
                case ClientOperationsType.CreateUser:
                    if (response.IsSuccess)
                    {
                        this.LoginStatus = LoginStatus.LoggedIn;
                        this.ClientName = new OperationDataHandler().TranslateObject<string>(response.ResponseData);
                    }
                    else
                        this.LoginStatus = LoginStatus.LoginError;
                    break;
                case ClientOperationsType.BattleFound:
                    this.HandleBattleFound(response);
                    break;
                case ClientOperationsType.SendBattleTurn:
                    this.HandleBattleTurn(response);
                    break;
            }
        }

        private void HandleBattleTurn(ServerResponse response)
        {
            BattleResponseData moveResult = new OperationDataHandler().TranslateObject<BattleResponseData>(response.ResponseData);

            if (!moveResult.IsBattleContinue)
            {
                this.BattleStatus = response.IsSuccess ? BattleStatus.Victory : BattleStatus.Defeat;
                return;
            }

            this.BattleStatus = BattleStatus.Battle;
            
            this.OnBatteStatusChanged(new BattleStatusData()
            {
                BattleStatus = BattleStatus.Battle,
                ResponseData = moveResult
            });
        }

        private void HandleBattleFound(ServerResponse response)
        {
            this.BattleStatus = BattleStatus.Battle;
            
            var opHandler = new OperationDataHandler();

            NewBattleData battleData = opHandler.TranslateObject<NewBattleData>(response.ResponseData);
            this.BattleId = battleData.BattleId;
            this.OnBatteStatusChanged(new BattleStatusData()
            {
                BattleStatus = BattleStatus.Battle,
                NewBattleData = battleData
            });
        }

        public void MakeBattleMove(BattleMoveData moveData)
        {
            moveData.BattleId = this.BattleId;
            
            var opHandler = new OperationDataHandler();

            byte[] data = opHandler.GetBytes(new ClientRequestData(ClientOperationsType.SendBattleTurn, opHandler.GetBytes(moveData)));
            
    
            // Send the message to the connected TcpServer. 
            this.WriteData(data);

            this.BattleStatus = BattleStatus.Wait;
        }

        public void CreateUser(string login, string password)
        {
            var opHandler = new OperationDataHandler();
            byte[] data = opHandler.GetBytes(new ClientRequestData(ClientOperationsType.CreateUser,
                opHandler.GetBytes(new LoginData(login, password))));
                
    
            // Send the message to the connected TcpServer. 
            this.WriteData(data);

            this.LoginStatus = LoginStatus.Loggining;
        }

        public void PerformLogin(string login, string password)
        {
            var opHandler = new OperationDataHandler();
            byte[] data = opHandler.GetBytes(new ClientRequestData(ClientOperationsType.Login,
                opHandler.GetBytes(new LoginData(login, password))));
            
            // Send the message to the connected TcpServer. 
            this.WriteData(data);

            this.LoginStatus = LoginStatus.Loggining;
        }

        private void WriteData(byte[] data)
        {
            lock (this.lockObject)
            {
                this.SslStream.Write(data, 0, data.Length);
            }
        }

        public void FindBatle()
        {
            var opHandler = new OperationDataHandler();
            byte[] data = opHandler.GetBytes(new ClientRequestData(ClientOperationsType.FindBattle, null));
            
    
            // Send the message to the connected TcpServer. 
            this.WriteData(data);
        }

        public LoginStatus LoginStatus
        {
            get
            {
                return this.loginStatus;
                
            }
            set
            {
                if (this.LoginStatus != value)
                {
                    this.loginStatus = value;
                    this.OnPropertyChanged("LoginStatus");                   
                }
            }
        }

        public BattleStatus BattleStatus
        {
            get
            {
                return this.battleStatus;
                
            }
            set
            {
                if (this.BattleStatus != value)
                {
                    this.battleStatus = value;
                    this.OnPropertyChanged("BattleStatus");                   
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnBatteStatusChanged(BattleStatusData data)
        {
            if (this.BattleStatusChanged != null)
            {
                this.BattleStatusChanged(this, new BattleUpdateEventArgs(data));
            }
        }

        public void Dispose()
        {
            if (this.listener != null && this.listener.IsAlive)
                this.listener.Abort();
            
            if (this.client != null)
            {
                this.Client.Close();
            }
        }
    }
}