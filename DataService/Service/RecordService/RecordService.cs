using AsicServer.Core.Entities;
using AsicServer.Core.ViewModels;
using AsicServer.Core.Utils;
using AsicServer.Infrastructure;
using DataService.Repository;
using DataService.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataService.Service.RecordService
{
    public interface IRecordService : IBaseService<Record>
    {
        Task ProcessSyncRequestAsync(List<RecordInSyncData> attendanceData);
    }

    public class RecordService : BaseService<Record>, IRecordService
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
