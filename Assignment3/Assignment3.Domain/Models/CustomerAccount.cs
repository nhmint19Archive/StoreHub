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

        public CustomerAccount(string id, string username, string password, string email, string phone, DateTime registryDate) : 
            base(id, username, password, email, phone, registryDate)
        {
            _active = true;
        }

        public bool Active
        {
            get => _active; 
            set => _active = value; 
        }

        // TODO: create Order classes
        //public bool PlaceOrder(Order order)
        //{
        //}

        // TODO: create Catalogue class
        //public List<Product> SearchCatalogue(Catalogue catalogue, string keyword)
        //{
        //    return new List<Product>();
        //}
    }
}
