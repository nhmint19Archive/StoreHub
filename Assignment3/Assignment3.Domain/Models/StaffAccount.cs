using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models
{
    internal class StaffAccount : UserAccount
    {
        public StaffAccount(string id, string username, string password, string email, string phone, DateTime registryDate) :
            base(id, username, password, email, phone, registryDate)
        { }


        // TODO: create Repository class

        //public Product? GetProductById(Repository<Product> repo, string productId)
        //{
        //    return null;
        //}

        //public List<Product> GetProductsByName(Repository<Product> repo, string name)
        //{
        //    return new List<Product>();
        //}

        //public bool AddProduct(Repository<Product> repo, Product product)
        //{
        //    return false;
        //}

        //public bool UpdateProduct(Repository<Product> repo, Product product)
        //{
        //    return false;
        //}

        //public bool RemoveProduct(Repository<Product> repo, Product product)
        //{
        //    return false;
        //}

        // TODO: create SalesAnalyser
        //public string GetStatistics(SalesAnalyser salesAnalyser)
        //{
        //    return "";
        //}

        // TODO: create Order
        // public bool TriggerRefund(Order order)
        //{
        //    return false;
        //}
    }
}
