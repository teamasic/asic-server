using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Core.Training;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService service;
        private readonly ITrainingService trainingService;

        public UserController(IUserService service,
            ITrainingService trainingService,
            ExtensionSettings extensionSettings) : base(extensionSettings)
        {
            this.service = service;
            this.trainingService = trainingService;
        }

        [HttpPost("login")]
        public async Task<dynamic> Login(UserAuthentication user)
        {
            return await ExecuteInMonitoring(async () =>
            {
                var result = await service.Authenticate(user);
                return result;
            });
        }

        [HttpPost("login/admin")]
        public async Task<dynamic> LoginAsAdmin(UserAuthentication user)
        {
            return await ExecuteInMonitoring(async () =>
            {
                var result = await service.AuthenticateAsAdmin(user);
                return result;
            });
        }

        [HttpPost("multiple")]
        public dynamic CreateMultipleUsers(IFormFile zipFile, IFormFile users, [FromQuery] bool isAppendTrain)
        {
            return ExecuteInMonitoring(() =>
            {
              var userWithImages = service.CreateMultipleUsers(users, zipFile);
              if (isAppendTrain)
                {
                    var alreadyHasModel = trainingService.HasExistingModel();
                    if (alreadyHasModel)
                    {
                        var codes = userWithImages.Select(u => u.Code).ToList();
                        trainingService.AddEmbeddings(codes);
                    } else
                    {
                        trainingService.Train();
                    }
                }
              return userWithImages;
            });
        }

        [HttpPost("single")]
        public dynamic CreateSingleUsers(IFormFile zipFile, [FromQuery] CreateUser user, [FromQuery] bool isAppendTrain)
        {
            return ExecuteInMonitoring(async () =>
           {
               var result = await service.CreateSingleUser(zipFile, user);
               if (isAppendTrain)
               {
                   var alreadyHasModel = trainingService.HasExistingModel();
                   if (alreadyHasModel)
                   {
                       trainingService.AddEmbeddings(new string[] { user.Code });
                   } else
                   {
                       trainingService.Train();
                   }
               }
               return result;
           });
        }

        [HttpGet]
        public BaseResponse<UserViewModel> GetByEmail([FromQuery] string email)
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetByEmail(email);
            });
        }

        [HttpGet("image")]
        public BaseResponse<List<UserViewModel>> GetByCodes([FromQuery] string codes)
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetByCodes(codes);
            });
        }

        [HttpPost("train")]
        public dynamic TrainMore([FromQuery] string code)
        {
            return ExecuteInMonitoring(() =>
            {
                service.NotifyToTrainMore(code);
                return new AttendeeViewModel
                {
                    Code = code
                };
            });
        }

        [HttpGet("train")]
        public BaseResponse<List<UserViewModel>> GetUsersFromTrainMoreList()
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetUsersFromTrainMoreList();
            });
        }
    }
}