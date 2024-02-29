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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Category> GetAll()
        {
            return _context.Categories.Where(x=>x.IsDelete==false).OrderByDescending(x=>x.CreatedAt ).ToList();
        }
        public override Category Get(int Id)
        {
            return (_context.Categories.Find(Id));
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var categories = await _context.Categories.FindAsync(Id);
                    if (categories == null)
                    {
                        return false;
                    }
                    categories.IsDelete = true;
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
         #region<カテゴリから商品を探す>
        public List<Product> GetProductByCategoryID(int? id)
        {
            var products = _context.Products.Where(x => x.CategoryId == id && x.IsDelete == false).ToList();
            return (products);
            
        }
        #endregion
    }
}
