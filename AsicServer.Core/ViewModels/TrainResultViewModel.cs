using AsicServer.Core.Infrastructure;
using AsicServer.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class TrainResultViewModel : BaseViewModel
    {
        public DateTime TimeFinished { get; set; }
        public int AttendeeCount { get; set; }
        public int ImageCount { get; set; }
    }
}
