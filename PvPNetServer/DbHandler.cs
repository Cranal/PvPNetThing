using System;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace PvPNetServer
{
    public class DbHandler
    {
        private string ConnectionString;

        public DbHandler()
        {
            DbConnectDataProvider dbConnectDataProvider = new DbConnectDataProvider();
            DbConnectDataProvider.DbConnectData connectData = new DbConnectDataProvider().GetData();

            if (connectData.IsEmpty())
            {
                //
                // Create default.
                //
                
                dbConnectDataProvider.SaveToXml();
                
                this.ConnectionString =
                    "Server=127.0.0.1;Port=5432;User Id=postgres;Password=53170;Database=pvpnetthing;";
            }
            else
            {
                this.ConnectionString =
                    string.Format("Server=127.0.0.1;Port=5432;User Id={0};Password={1};Database={2};", 
                        connectData.UserName, connectData.Password, connectData.DbName);
            }
        }
        
        public bool CreateUser(string login, byte[] password)
        {
            if (this.CheckExistingUser(login))
                return false;

            string sql = " INSERT INTO pvpuser (username, userpassword) VALUES (@login, @password)";
            int result;
            
            using (NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString))
            {
                NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                comm.Parameters.AddWithValue("@password", NpgsqlDbType.Bytea, password);
                
                
                conn.Open();
                try
                {
                    result = comm.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
                
                conn.Close(); 
            }

            return result == 1;
        }

        public bool CheckExistingUser(string login)
        {
            string sql = " SELECT COUNT(*) FROM pvpuser WHERE username = @login";

            NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString);

            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
            conn.Open();
            string result = comm.ExecuteScalar().ToString();
            conn.Close();

            return result != "0";
        }
        
        public bool CheckExistingUser(string login, string password)
        {
            var encryption = new Encryption();
            
            string sql = " SELECT userpassword FROM pvpuser WHERE username = @login";

            NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString);

            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
            conn.Open();
            object result = comm.ExecuteScalar();
            conn.Close();

            if (result == null || string.IsNullOrWhiteSpace(result.ToString()))
                return false;
            
            bool res =  encryption.CheckData(password, (byte[])result, login);
            
            return res;
        }
    }
}