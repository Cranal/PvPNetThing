using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PvPNet
{
    [Serializable]
    public class LoginData
    {
        public string Login
        {
            get; 
            private set;
        }

        public LoginData(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        public string Password
        {
            get; 
            private set;
        }  
    }
}