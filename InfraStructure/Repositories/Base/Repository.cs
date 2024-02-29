using Core.Entities.Base;
using Core.Repositories.Base;
using InfraStructure.Data;
using InfraStructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly string _connectionString;

        public Repository(ApplicationDbContext context, IConfigService configService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionString = configService.GetConnectionString();
        }

        public virtual List<T> GetAll()
        {
            throw new NotImplementedException();
        }
        public virtual T Get(int Id)
        {
            throw new NotImplementedException();
        }
        public virtual async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual Task<bool> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

    }
}
