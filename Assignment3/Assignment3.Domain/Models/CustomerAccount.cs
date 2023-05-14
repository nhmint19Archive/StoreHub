using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models
{
    internal class CustomerAccount : UserAccount
    {
        private bool _active;

        public CustomerAccount(string id, string username, string password) : base(id, username, password)
        {
            _active = true;
        }

        public bool Active
        {
            get => _active; 
            set => _active = value; 
        }

        protected override void Authenticate()
        {
        }

        public void PlaceOrder()
        {
        }

        //public string? searchCatalogue(Catalogue catalogue, string keyword)
        //{
        //    return null;
        //}
    }
}
