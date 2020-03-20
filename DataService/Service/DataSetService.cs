using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Repository;
using DataService.UoW;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Service
{
    public interface IDataSetService: IBaseService<DataSet>
    {
        Task<DataSetViewModel> add(CreateDataSetViewModel dataSetViewModel);
        List<DataSetViewModel> GetAll();
    }
    public class DataSetService : BaseService<DataSet>, IDataSetService
    {
        private readonly IDataSetRepository dataSetRepository;
        private readonly IUserService userService;
        private readonly IDataSetUserService dataSetUserService;
        public DataSetService(UnitOfWork unitOfWork, IDataSetRepository dataSetRepository, 
            IUserService userService, IDataSetUserService dataSetUserService) : base(unitOfWork)
        {
            this.dataSetRepository = dataSetRepository;
            this.userService = userService;
            this.dataSetUserService = dataSetUserService;
        }

        public async Task<DataSetViewModel> add(CreateDataSetViewModel dataSetViewModel)
        {
            var usersInDb = CheckListUserExisted(dataSetViewModel.RollNumbers);
            if(usersInDb.Count > 0)
            {
                var dataSet = new DataSet() { Name = dataSetViewModel.Name };
                await dataSetRepository.AddAsync(dataSet);
                //List<DataSetUser> dataSetUsers = new List<DataSetUser>();
                foreach (var user in usersInDb)
                {
                    var dataSetUser = new DataSetUser()
                    {
                        UserId = user.Id,
                        DataSetId = dataSet.Id
                    };
                    await dataSetUserService.add(dataSetUser);
                    //dataSetUsers.Add(dataSetUser);
                }
                return AutoMapper.Mapper.Map<DataSetViewModel>(dataSet);
            }
            throw new BaseException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_DATASET);
        }

        public List<DataSetViewModel> GetAll()
        {
            var dataSets = dataSetRepository.GetAllWithUser();
            var results = new List<DataSetViewModel>();
            foreach (var dataset in dataSets)
            {
                var viewModel = AutoMapper.Mapper.Map<DataSetViewModel>(dataset);
                results.Add(viewModel);
            }
            return results;
        }

        private List<User> CheckListUserExisted(List<CreateDataSetUserViewModel> rollnumbers)
        {
            var results = new List<User>();
            foreach (var item in rollnumbers)
            {
                var user = userService.GetByRollNumber(item.RollNumber);
                if(user != null)
                {
                    results.Add(user);
                } else
                {
                    return new List<User>();
                }
            }
            return results;
        }
    }
}
