using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Infrastructure
{
    class AutoMapperConfiguration
    {
        public static IMapper GetInstance()
        {
            return Mapper.Configuration.CreateMapper();
        }
    }
}
