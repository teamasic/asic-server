using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Core.Models;
using AsicServer.Infrastructure;
using DataService.Service;
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
        public dynamic Login(UserAuthentication user)
        {
            return ExecuteInMonitoring(() =>
            {
                var result = service.Authenticate(user);
                return result;
            });
        }

        [HttpPost("register")]
        public async Task<dynamic> Register(RegisteredUser user)
        {
            return await ExecuteInMonitoring(async () =>
            {
                return await this.service.Register(user);
            });
        }

        [HttpPost("registerExternal")]
        public async Task<dynamic> RegisterWithFirebase(FirebaseRegisterExternal external)
        {
            return await ExecuteInMonitoring(async () =>
            {
                return await service.RegisterExternalUsingFirebaseAsync(external);
            });
        }

    }
}