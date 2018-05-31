using System;

namespace PvPNet
{
    [Serializable]
    public class ServerResponse
    {
        public ClientOperationsType Operations
        {
            get; 
            set;
        }

        public bool IsSuccess
        {
            get; 
            set;
        }

        public byte[] ResponseData
        {
            get; 
            set;
        }
    }
}