using AsicServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Repository
{
    public interface IDataSetUserRepository: IBaseRepository<DataSetUser>
    {

    }
    public class DataSetUserRepository : BaseRepository<DataSetUser>, IDataSetUserRepository
    {
        public DataSetUserRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
