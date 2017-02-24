using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.Models;

namespace dc_snoop.DAL
{
    public interface ISnoopRepository
    {
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity;

        TEntity GetById<TEntity>(int id) where TEntity : class, IEntity;
    }
}
