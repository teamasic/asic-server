using AsicServer.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Repository
{
    public interface IRecordStagingRepository : IBaseRepository<RecordStaging>
    {
        Task AddRangeAsync(List<RecordStaging> records);
        int MergeRecordStagingInSyncData(DataTable parameters);
        void DeleteRange(List<RecordStaging> records);


    }
    public class RecordStagingRepository : BaseRepository<RecordStaging>, IRecordStagingRepository
    {
        private readonly DbSet<RecordStaging> dbSet;
        public RecordStagingRepository(DbContext dbContext) : base(dbContext)
        {
            this.dbSet = dbContext.Set<RecordStaging>();
        }

        public async Task AddRangeAsync(List<RecordStaging> records)
        {
            await dbSet.AddRangeAsync(records);
            dbContext.SaveChanges();
        }
    
        public int MergeRecordStagingInSyncData(DataTable dataTable)
        {
            SqlParameter parameter = new SqlParameter("@ids", SqlDbType.Structured) { Value = dataTable };
            parameter.TypeName = "[dbo].[IntID]";
            return dbContext.Database.ExecuteSqlRaw("MergeRecordStagingInSyncData @ids", parameter);
        }

        public void DeleteRange(List<RecordStaging> records)
        {
            this.dbSet.RemoveRange(records);
            dbContext.SaveChanges();
        }
    }
}
