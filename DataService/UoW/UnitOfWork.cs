﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using DataService.Repository;

namespace DataService.UoW
{
    public class UnitOfWork
    {
        private readonly DbContext _dbContext;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IDbContextTransaction CreateTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        private IRecordStagingRepository recordStagingRepository;

     
        public IRecordStagingRepository RecordStagingRepository
        {
            get
            {
                if (recordStagingRepository == null)
                {
                    recordStagingRepository = new RecordStagingRepository(_dbContext);
                }
                return recordStagingRepository;
            }
        }



    }
}
