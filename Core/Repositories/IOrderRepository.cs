using Core.Entities;
using Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IOrderRepository:IRepository<Order>
    {
        /// <summary>
        /// 注文リストマッチ注文ステータスを返す
        /// </summary>
        /// <param name="IsDeliver"></param>
        /// <returns></returns>
        public List<Order> GerOrderByDeliver(string IsDeliver);
        public List<Order> GerOrderByDeliverPending(string IsDeliver);

        /// <summary>
        /// Get order list match from Email and order status
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="IsDeliver"></param>
        public List<Order> GetOrderByUserEmailandAllDeliverStatus(string Email, string IsDeliver);
        public List<Order> GetOrderByUserEmailandDeliver(string Email, string IsDeliver);
        public List<Order> GetOrderByUserEmailandDeliverPending(string Email, string IsDeliver);

        Task<string> DeliverAsync(int Id);
    }
}
