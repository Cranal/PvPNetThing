using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PvPNet
{
    public class OperationDataHandler
    {
        public byte[] GetBytes(object data)
        {
            if (data == null)
                return null;
            
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }
        
        public T TranslateObject<T>(byte[] data)
        {
            T requestData;

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(data, 0, data.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    requestData = (T) binForm.Deserialize(memStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                requestData = default(T);
            }

            return requestData;
        }
    }
}