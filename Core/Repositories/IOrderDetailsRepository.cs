using Core.Entities;
using Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        public List<OrderDetails> GetOrderDetailByOrderID(int? id);

    }
}
