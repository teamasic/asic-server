using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSetController : BaseController
    {
        private readonly IDataSetService dataSetService;
        public DataSetController(ExtensionSettings extensionSettings, IDataSetService dataSetService) : base(extensionSettings)
        {
            this.dataSetService = dataSetService;
        }

        [HttpGet]
        public async Task<dynamic> getAll()
        {
            return await ExecuteInMonitoring(async () =>
            {
                return dataSetService.GetAll();
            });
        }

        [HttpPost]
        public async Task<dynamic> create(CreateDataSetViewModel dataSetViewModel)
        {
            return await ExecuteInMonitoring(async () =>
            {
                var addedDataSet = await dataSetService.add(dataSetViewModel);
                return addedDataSet;
            });
        }
    }
}
