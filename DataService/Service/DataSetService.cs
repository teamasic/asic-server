using AsicServer.Core.Entities;
using DataService.Repository;
using DataService.UoW;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Service
{
    public interface IDataSetService: IBaseService<DataSet>
    {

    }
    public class DataSetService : BaseService<DataSet>, IDataSetService
    {
        private readonly IDataSetRepository dataSetRepository;
        public DataSetService(UnitOfWork unitOfWork, IDataSetRepository dataSetRepository) : base(unitOfWork)
        {
            this.dataSetRepository = dataSetRepository;
        }
    }
}
