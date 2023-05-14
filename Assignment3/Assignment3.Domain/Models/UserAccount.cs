using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models
{
    internal abstract class UserAccount
    {
        private string _id;
        private string _username;
        private string _password;
        private string? _email;
        private string? _phone;
        private DateTime? _registryDate;
        public UserAccount(string id, string username, string password)
        {
            _id = id;
            _username = username;
            _password = password;
            _email = null;
            _phone = null;
            _registryDate = null;
        }

        public string Id 
        {
            get => _id;
        }

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string? Email
        {
            get => _email;
            set => _email = value;
        }

        public string? Phone
        {
            get => _phone;
            set => _phone = value;
        }

        public DateTime? RegistryDate
        {
            get => _registryDate;
        }

        protected abstract void Authenticate();
    }
}
