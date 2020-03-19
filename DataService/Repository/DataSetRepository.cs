using AsicServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Repository
{
    public interface IDataSetRepository: IBaseRepository<DataSet>
    {
    }
    public class DataSetRepository : BaseRepository<DataSet>, IDataSetRepository
    {
        public DataSetRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
