using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AsicServer.Core.Models;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Service;
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

        public UserController(IUserService service, ExtensionSettings extensionSettings) : base(extensionSettings)
        {
            this.service = service;
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
        public async Task<dynamic> CreateMultipleUsers(IFormFile zipFile, IFormFile users)
        {
            return await ExecuteInMonitoring(async () =>
            {
                var userWithoutImages = service.CreateMultipleUsers(users, zipFile);
                return userWithoutImages;
            });
        }

        [HttpPost("single")]
        public async Task<dynamic> CreateSingleUsers(IFormFile zipFile, [FromQuery] CreateUser user)
        {
            return await ExecuteInMonitoring(async () =>
            {
                var result = service.CreateSingleUser(zipFile, user).Result;
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

    }
}