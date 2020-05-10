using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AsicServer.Core.Training;
using AsicServer.Core.ViewModels;
using AttendanceSystemIPCamera.Services.RecordService;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : BaseController
    {
        private readonly ITrainingService trainingService;
        private readonly IGlobalStateService globalStateService;

        public ModelController(ExtensionSettings extensionSettings,
            ITrainingService trainingService, IGlobalStateService globalStateService) : base(extensionSettings)
        {
            this.trainingService = trainingService;
            this.globalStateService = globalStateService;
        }

        [HttpPost("log")]
        public void PostLog(string value)
        {
            Logger.Debug(value);
        }

        [HttpPost]
        public BaseResponse<ResponsePython> Train()
        {
            return ExecuteInMonitoring(() =>
            {
                /*
                if (globalStateService.IsTraining())
                {
                    throw new Exception("Already training model");
                }
                */
                try
                {
                    globalStateService.SetTraining(true);
                    return trainingService.Train();
                }
                finally
                {
                    globalStateService.SetTraining(false);
                }
            });
        }

        [HttpPut]
        public BaseResponse<ResponsePython> AddEmbeddings(ModifyEmbeddingsViewModel viewModel)
        {
            return ExecuteInMonitoring(() =>
            {
                return trainingService.AddEmbeddings(viewModel.Names);
            });
        }

        [HttpDelete]
        public BaseResponse<ResponsePython> RemoveEmbeddings(ModifyEmbeddingsViewModel viewModel)
        {
            return ExecuteInMonitoring(() =>
            {
                return trainingService.RemoveEmbeddings(viewModel.Names);
            });
        }

        [HttpGet("last-result")]
        public BaseResponse<TrainResultViewModel> GetLastTrainingResult()
        {
            return ExecuteInMonitoring(() =>
            {
                return trainingService.GetLastTrainingResult();
            });
        }

        [HttpGet("is-training")]
        public BaseResponse<bool> IsTraining()
        {
            return ExecuteInMonitoring(() =>
            {
                return globalStateService.IsTraining();
            });
        }
    }
}
