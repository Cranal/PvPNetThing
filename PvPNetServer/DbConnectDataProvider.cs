using System;
using System.IO;
using System.Xml.Serialization;

namespace PvPNetServer
{
    public class DbConnectDataProvider
    {
        public DbConnectData GetData()
        {
            StreamReader file = null;
            DbConnectData data = new DbConnectData();
            XmlSerializer writer = new XmlSerializer(typeof(DbConnectData));

            if (!File.Exists("dbConnect.xml"))
                return data;
    
            try
            {
                file = new StreamReader("dbConnect.xml");
                data = (DbConnectData) writer.Deserialize(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            return data;
        }
        
        public void SaveToXml()
        {
            //
            // should be called only once to create defaul data.
            //
            DbConnectData data = new DbConnectData()
            {
                DbName = "pvpnetthing",
                Password = "53170",
                UserName = "postgres"
            };

            StreamWriter file = null;
            XmlSerializer writer = new XmlSerializer(data.GetType());

            try
            {
                file = new StreamWriter("dbConnect.xml");
                writer.Serialize(file, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (file != null)
                    file.Close();;
            }      
        }
        
        public struct DbConnectData
        {
            public string UserName
            {
                get; 
                set;
            }

            public string Password
            {
                get; 
                set;
            }

            public string DbName
            {
                get; 
                set;
            }

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(this.DbName) || string.IsNullOrEmpty(this.Password) ||
                       string.IsNullOrEmpty(this.UserName);
            }
        }
            
            
    }
}