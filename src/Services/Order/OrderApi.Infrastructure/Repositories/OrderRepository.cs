using BuildingBlocks.Core.Responses;
using BuildingBlocks.Web.Logging;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = _context.Orders.Add(entity).Entity;
                var rows_affected = await _context.SaveChangesAsync();

                return rows_affected > 0
                    ? new Response(true, "Order placed successfully")
                    : new Response(false, "Error occurred while placing order");
            }
            catch(Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);
                
                throw;
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var order = await FindByIdAsync(id);
                if(order is null)
                {
                    return new Response(false, "Order not found");
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return new Response(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);
                
                throw;
            }
        }

        public async Task<Order?> FindByIdAsync(int id)
        {
            try
            {
                return await _context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                return await _context.Orders.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw;
            }
        }

        public async Task<Order?> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                return await _context.Orders.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                return await _context.Orders.Where(predicate).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                throw;
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);

                if (order is null)
                    return new Response(false, "Order not found");

                _context.Entry(order).State = EntityState.Detached;

                _context.Orders.Update(entity);

                await _context.SaveChangesAsync();

                return new Response(true, "Order updated");
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                throw;
            }
        }
    }
}
