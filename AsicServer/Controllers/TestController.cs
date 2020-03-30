using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : BaseController
    {
        private ILogger<TestController> logger;
        public TestController(ExtensionSettings extensionSettings, ILogger<TestController> logger) : base(extensionSettings)
        {
            this.logger = logger;
        }

        [HttpPost("log")]
        public void PostLog()
        {
            logger.LogInformation("This is log infomation");
            logger.LogDebug("This is log debug");
            logger.LogWarning("This is log warning");
            logger.LogError("This is log error");
        }

        [HttpGet("log")]
        public ActionResult<string> GetLog()
        {
            throw new NotImplementedException("Not implement this function");
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize]
        [HttpGet("parseToken")]
        public dynamic parseToken()
        {
            var id = this.CurrentUserId;
            var username = this.CurrentUsername;
            return new
            {
                id = id,
                username = username,
                claims = this.currentUser.Claims.Select(c => new { key = c.Type, value = c.Value })
            };
        }

    }
}
