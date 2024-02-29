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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.Repositories
{
    public class ProductRepository:Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Product> GetAll()
        {
            return _context.Products.Include(m => m.Category).Where(x=>x.IsDelete == false ).OrderByDescending(x => x.CreatedAt).ToList();
        }
        public override Product Get(int Id)
        {
            return (_context.Products.Find(Id));
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var products = await _context.Products.FindAsync(Id);
                    if (products == null)
                    {
                        return false;
                    }
                    products.IsDelete = true;
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

        public Product GetAllByProductId(int id)
        {
            var data = _context.Products.Include(m => m.Category).Where(x => x.Id == id && x.IsDelete == false).ToList();
           return data.Count > 0 ? data[0] : null;
        }
    }
}
