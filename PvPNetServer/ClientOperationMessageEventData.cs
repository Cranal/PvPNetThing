using System;

namespace PvPNetServer
{
    public class ClientOperationMessageEventData : EventArgs
    {
        public ClientOperationMessageEventData(ClientMessage data)
        {
            this.Data = data;
        }

        public ClientMessage Data
        {
            get; 
            set;
        }
    }
}