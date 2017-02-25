using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.Models;

namespace dc_snoop.DAL
{
    public class SnoopRepository : ISnoopRepository
    {
        private readonly SnoopContext Context;

        public SnoopRepository(SnoopContext context)
        {
            this.Context = context;
        }
        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity
        {
            return this.Context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetById<TEntity>(int id) where TEntity : class, IEntity
        {
            return this.Context.Set<TEntity>().Where(e => e.Id == id);
        }
    }
}
