using Core.Entities;
using Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repositories
{
    public interface ICategoryRepository:IRepository<Category>
    {
        #region<カテゴリから商品を探す>
        public List<Product> GetProductByCategoryID(int? id);
        #endregion
    }
}
