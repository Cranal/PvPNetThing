using PvPNet;

namespace PvPNetServer
{
    public class ClientMessage
    {
        public ConnectedClient Client
        {
            get; 
            set;
        }
        
        public ClientRequestData Operation
        {
            get; 
            set;
        }
    }
}