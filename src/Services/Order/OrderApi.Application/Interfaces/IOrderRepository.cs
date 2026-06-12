using BuildingBlocks.Core.Interfaces;
using OrderApi.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace OrderApi.Application.Interfaces
{
    public interface IOrderRepository : IGenericInterface<Order>
    {
        Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate);
    }
}
