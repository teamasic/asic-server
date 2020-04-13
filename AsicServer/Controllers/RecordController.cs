using System.Collections.Generic;
using System.Threading.Tasks;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Service.RecordService;
using Microsoft.AspNetCore.Mvc;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : BaseController
    {
        private readonly IRecordService service;

        public RecordController(IRecordService service, ExtensionSettings extensionSettings) : base(extensionSettings)
        {
            this.service = service;
        }

        [HttpPost("sync")]
        public async Task<dynamic> ReceiveAttendanceDataFromSupervisor(List<RecordInSyncData> attendanceData)
        {
            return await ExecuteInMonitoring(async () =>
            {
                await service.ProcessSyncRequestAsync(attendanceData);
                return "success";
            });
        }



    }
}