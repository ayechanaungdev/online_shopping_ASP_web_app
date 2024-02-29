using Core;
using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using InfraStructure.Interfaces;
using InfraStructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Order> GetAll()
        {
            return _context.Orders.Where(x => x.IsDelete == false).OrderByDescending(x => x.CreatedAt).ToList();
        }
        public override Order Get(int Id)
        {
            return (_context.Orders.Find(Id));
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orders = await _context.Orders.FindAsync(Id);
                    if (orders == null)
                    {
                        return false;
                    }
                    orders.IsDelete = true;
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
        public List<Order> GetOrderByUserEmailandAllDeliverStatus(string Email, string IsDeliver)
        {
            var EmailDelivers = _context.Orders.Where(x => x.Email.Contains(Email) && x.IsDelete == false).ToList();
            return (EmailDelivers);
        }
        public List<Order> GetOrderByUserEmailandDeliver(string Email,string IsDeliver)
        {
            var EmailDelivers = _context.Orders.Where(x => x.Email.Contains(Email) && x.IsDeliver.Equals(IsDeliver) && x.IsDelete == false).ToList();
            return (EmailDelivers);
        }
        public List<Order> GetOrderByUserEmailandDeliverPending(string Email, string IsDeliver)
        {
            var EmailDelivers = _context.Orders.Where(x => x.Email.Contains(Email) && x.IsDeliver == null && x.IsDelete == false).ToList();
            return (EmailDelivers);
        }
        /// <summary>
        /// 注文リストマッチ注文ステータスを返す
        /// </summary>
        /// <param name="IsDeliver"></param>
        /// <returns></returns>
        public List<Order> GerOrderByDeliver(string IsDeliver)
        {
            var delivers = _context.Orders.Where(x => x.IsDeliver.Equals(IsDeliver) && x.IsDelete == false).ToList();
            return (delivers);
        }
        public List<Order> GerOrderByDeliverPending(string IsDeliver)
        {
            var delivers = _context.Orders.Where(x => x.IsDeliver == null && x.IsDelete == false).ToList();
            return (delivers);
        }

        public async Task<string> DeliverAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orders = await _context.Orders.FindAsync(Id);
                    if (orders == null)
                    {
                        return orders.IsDeliver = "0";
                    }
                    orders.IsDeliver = "1";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return "Delivered";
        }
    }
}
