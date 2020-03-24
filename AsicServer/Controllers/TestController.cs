using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : BaseController
    {
        public TestController(ExtensionSettings extensionSettings) : base(extensionSettings)
        {
        }

        [HttpPost("log")]
        public void PostLog(string value)
        {
            Logger.Debug(value);
        }

        [HttpGet("log")]
        public ActionResult<string> GetLog()
        {
            var fs = System.IO.File.ReadAllText(@"DebugLog/debug.log");
            return fs;
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

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
