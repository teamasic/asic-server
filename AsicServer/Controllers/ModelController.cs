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

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : BaseController
    {
        private readonly ITrainingService trainingService;

        public ModelController(ExtensionSettings extensionSettings,
            ITrainingService trainingService) : base(extensionSettings)
        {
            this.trainingService = trainingService;
        }

        [HttpPost("log")]
        public void PostLog(string value)
        {
            Logger.Debug(value);
        }

        [HttpPost]
        public ResponsePython Train()
        {
            try
            {
                return trainingService.Train();
            }
            catch (Exception e)
            {
                return new ResponsePython
                {
                    Success = false,
                    Errors = e.Message
                };
            }
        }

        [HttpPut]
        public ResponsePython AddEmbeddings(ModifyEmbeddingsViewModel viewModel)
        {
            try
            {
                return trainingService.AddEmbeddings(viewModel.Names);
            }
            catch (Exception e)
            {
                return new ResponsePython
                {
                    Success = false,
                    Errors = e.Message
                };
            }
        }

        [HttpPost("remove")] // HttpDelete doesn't allow body
        public ResponsePython RemoveEmbeddings(ModifyEmbeddingsViewModel viewModel)
        {
            try
            {
                return trainingService.RemoveEmbeddings(viewModel.Names);
            }
            catch (Exception e)
            {
                return new ResponsePython
                {
                    Success = false,
                    Errors = e.Message
                };
            }
        }
    }
}
