using AsicServer.Core.Infrastructure;
using DataService.Repository;
using DataService.UoW;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Service
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity
    {

    }

    public class BaseService<TEntity> where TEntity : BaseEntity
    {
        protected readonly UnitOfWork unitOfWork;

        public BaseService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
