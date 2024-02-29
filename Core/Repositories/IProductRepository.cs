using Core.Entities;
using Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        public Product GetAllByProductId(int id);
    }
}
