using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.Models;
using AsicServer.Core.Utils;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Repository;
using DataService.UoW;
using DataService.Validation;
using FirebaseAdmin.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Service.RecordService
{
    public interface IRecordService : IBaseService<Records>
    {
        Task ProcessSyncRequestAsync(List<RecordInSyncData> attendanceData);
    }

    public class RecordService : BaseService<Records>, IRecordService
    {
        private readonly IRecordStagingRepository recordStagingRepository;
        //private readonly IServiceScopeFactory serviceScopeFactory;
        public RecordService(UnitOfWork unitOfWork,
            //IServiceScopeFactory serviceScopeFactory,
            IRecordStagingRepository recordStagingRepository) : base(unitOfWork)
        {
            this.recordStagingRepository = recordStagingRepository;
            //this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ProcessSyncRequestAsync(List<RecordInSyncData> attendanceData)
        {
            using (var trans = unitOfWork.CreateTransaction())
            {
                try
                {
                    var recordStagings = attendanceData.Select(data =>
                    {
                        return new RecordStaging()
                        {
                            AttendeeCode = data.Attendee.Code,
                            AttendeeName = data.Attendee.Name,
                            SessionName = data.Session.Name,
                            SessionStartTime = data.Session.StartTime,
                            SessionEndTime = data.Session.EndTime,
                            RoomName = data.Session.RoomName,
                            RtspString = data.Session.RtspString,
                            GroupCode = data.Group.Code,
                            GroupName = data.Group.Name,
                            GroupCreateTime = data.Group.DateTimeCreated,
                            MaxSessionCount = data.Group.MaxSessionCount,
                            Present = data.Present,
                            IsEnrollInClass = data.IsEnrollInClass
                        };
                    }).ToList();
                    await recordStagingRepository.AddRangeAsync(recordStagings);

                    //merge statement
                    var ids = recordStagings.Select(r => r.Id);
                    int rowsAffected = recordStagingRepository.MergeRecordStagingInSyncData(Utils.GetTableType(ids));
                    Logger.Debug($"{rowsAffected} is affected");

                    //remove 
                    //recordStagingRepository.DeleteRange(recordStagings);
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw e;
                }
            }
        } 
    }
}
