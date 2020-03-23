using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsicServer.Core.Models;
using AsicServer.Infrastructure;
using DataService.Service;
using DataService.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : BaseController
    {
        private readonly IUserService service;

        public RecordController(IUserService service, ExtensionSettings extensionSettings) : base(extensionSettings)
        {
            this.service = service;
        }





    }
}