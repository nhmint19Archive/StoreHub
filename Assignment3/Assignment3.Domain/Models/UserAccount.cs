using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models
{
    internal class UserAccount
    {
        private string _id;
        private string _username;
        private string _password;
        private string _email;
        private string _phone;
        private DateTime _registryDate;
        public UserAccount(string id, string username, string password, string email, string phone, DateTime registryDate)
        {
            _id = id;
            _username = username;
            _password = password;
            _email = email;
            _phone = phone;
            _registryDate = registryDate;
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

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public string Phone
        {
            get => _phone;
            set => _phone = value;
        }

        public DateTime? RegistryDate
        {
            get => _registryDate;
        }

        public bool Authenticate(string username, string password)
        {
            // TODO: encrypt password
            return _username == username && _password == password;
        }
    }
}
