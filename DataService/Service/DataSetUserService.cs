using AsicServer.Core.Entities;
using DataService.Repository;
using DataService.UoW;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Service
{
    public interface IDataSetUserService: IBaseService<DataSetUser>
    {
        Task<DataSetUser> add(DataSetUser dataSetUser);
    }
    public class DataSetUserService : BaseService<DataSetUser>, IDataSetUserService
    {
        private readonly IDataSetUserRepository dataSetUserRepository;
        public DataSetUserService(UnitOfWork unitOfWork, IDataSetUserRepository dataSetUserRepository) : base(unitOfWork)
        {
            this.dataSetUserRepository = dataSetUserRepository;
        }

        public async Task<DataSetUser> add(DataSetUser dataSetUser)
        {
            await dataSetUserRepository.AddAsync(dataSetUser);
            return dataSetUser;
        }
    }
}
