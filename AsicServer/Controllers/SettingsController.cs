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

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly FilesConfiguration filesConfig;
        private readonly MyConfiguration myConfiguration;

        public SettingsController(ExtensionSettings extensionSettings, 
            FilesConfiguration filesConfig,
            MyConfiguration myConfiguration) : base(extensionSettings)
        {
            this.filesConfig = filesConfig;
            this.myConfiguration = myConfiguration;
        }

        [HttpPost("log")]
        public void PostLog(string value)
        {
            Logger.Debug(value);
        }

        private IActionResult GetFile(string name)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, name);
            var mimeType = "application/....";
            try
            {
                var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }

        private string GetModelRecognizer()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fileDest = Path.Join(parentDirectory,
                myConfiguration.RecognitionServiceName,
                myConfiguration.ModelOutputFolderName,
                filesConfig.RecognizerModelFile);
            return fileDest;
        }


        [HttpGet("model")]
        public IActionResult DownloadModelRecognizer()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fileDest = Path.Join(parentDirectory,
                myConfiguration.RecognitionServiceName,
                myConfiguration.ModelOutputFolderName,
                filesConfig.RecognizerModelFile);
            return GetFile(fileDest);
        }
        [HttpGet("room")]
        public IActionResult DownloadRoomConfig()
        {
            return GetFile(filesConfig.RoomConfigFile);
        }
        [HttpGet("unit")]
        public IActionResult DownloadUnitConfig()
        {
            return GetFile(filesConfig.UnitConfigFile);
        }
        [HttpGet("others")]
        public IActionResult DownloadOtherSettingsConfig()
        {
            return GetFile(filesConfig.SettingsConfigFile);
        }
        [HttpGet("last-updated")]
        public SettingsViewModel GetLastUpdated()
        {
            return new SettingsViewModel
            {
                Model = System.IO.File.GetLastWriteTime(GetModelRecognizer()),
                Room = System.IO.File.GetLastWriteTime(filesConfig.RoomConfigFile),
                Others = System.IO.File.GetLastWriteTime(filesConfig.SettingsConfigFile),
                Unit = System.IO.File.GetLastWriteTime(filesConfig.UnitConfigFile)
            };
        }
    }
}
