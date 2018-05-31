using System;
using PvPNet;

namespace PvPNetServer
{
    public class ClientReadEventData : EventArgs
    {
        public ClientReadEventData(ClientRequestData data)
        {
            this.Data = data;
        }
        
        public ClientRequestData Data
        {
            get;
            set;
        }
    }
}