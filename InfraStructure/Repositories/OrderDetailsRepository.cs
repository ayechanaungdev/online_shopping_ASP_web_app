using Core;
using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using InfraStructure.Interfaces;
using InfraStructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.Repositories
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        public OrderDetailsRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<OrderDetails> GetAll()
        {
            return _context.OrderDetails.Include(m => m.Order).Include(x=>x.Product).Where(x => x.IsDelete == false).OrderByDescending(x => x.CreatedAt).ToList();
        }
        public override OrderDetails Get(int Id)
        {
            return (_context.OrderDetails.Find(Id));
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderDetails = await _context.OrderDetails.FindAsync(Id);
                    if (orderDetails == null)
                    {
                        return false;
                    }
                    orderDetails.IsDelete = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return false;
        }

        public List<OrderDetails> GetOrderDetailByOrderID(int? id)
        {
            var orderDetails = _context.OrderDetails.Include(m => m.Order).Include(x => x.Product).Include(x => x.Product.Category).Where(x => x.OrderId == id && x.IsDelete == false).ToList();
            return (orderDetails);
        }

    }
}
