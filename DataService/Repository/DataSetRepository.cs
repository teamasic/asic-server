using AsicServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataService.Repository
{
    public interface IDataSetRepository: IBaseRepository<DataSet>
    {
        IEnumerable<DataSet> GetAllWithUser();
    }
    public class DataSetRepository : BaseRepository<DataSet>, IDataSetRepository
    {
        public DataSetRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<DataSet> GetAllWithUser()
        {
            return Get(includeProperties: "DataSetUser", orderBy: ds => ds.OrderByDescending(x => x.Id));
        }
    }
}
