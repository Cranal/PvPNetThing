using System;

namespace PvPNet
{
    [Serializable]
    public class ClientRequestData
    {
        public ClientOperationsType Operation
        {
            get;
            private set;
        }

        public byte[] OperationData 
        {
            get; 
            private set; 
        }

        public ClientRequestData(ClientOperationsType operation, byte[] operationData)
        {
            this.Operation = operation;
            this.OperationData = operationData;
        }
    }
}