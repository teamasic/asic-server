using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AsicServer.Core.GlobalState;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IGlobalStateService
    {
        public void SetTraining(bool isTraining);
        public bool IsTraining();
    }

    public class GlobalStateService : IGlobalStateService
    {
        private readonly GlobalState globalState;

        public GlobalStateService(GlobalState globalState) : base()
        {
            this.globalState = globalState;
        }

        public void SetTraining(bool isTraining)
        {
            globalState.IsTraining = isTraining;
        }

        public bool IsTraining()
        {
            return globalState.IsTraining;
        }
    }
}
