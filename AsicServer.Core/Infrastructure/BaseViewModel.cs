using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Infrastructure
{
    public partial class BaseViewModel
    {
        public TDestination ToEntity<TDestination>() where TDestination : BaseEntity
        {
            return AutoMapperConfiguration.GetInstance().Map<TDestination>(this);
        }
    }
}
