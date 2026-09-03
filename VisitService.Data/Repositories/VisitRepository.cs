using Microsoft.EntityFrameworkCore;
using VisitService.Data.Context;
using VisitService.Repository.Entities;
using VisitService.Repository.Interfaces;

namespace VisitService.Data.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        private readonly VisitDbContext _context;

        public VisitRepository(VisitDbContext context)
        {
            this._context = context;
        }

        public async Task AddAsync(Visit visit, OutboxEvent? outboxEvent = null)
        {
            await _context.Visits.AddAsync(visit);

            if(outboxEvent != null) 
                await _context.OutboxEvents.AddAsync(outboxEvent);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, OutboxEvent? outboxEvent = null)
        {
            Visit? v = await _context.Visits.FindAsync(id);
            if (v == null)
                throw new Exception("Visit non esistente");
            _context.Visits.Remove(v);

            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent);

            await _context.SaveChangesAsync();
        }
        public async Task<Visit?> GetByIdAsync(int id)
        {
            Visit? v = await _context.Visits.FindAsync(id);
            return v;
        }


        public async Task UpdateAsync(Visit visit, OutboxEvent? outboxEvent = null)
        {
            _context.Visits.Update(visit);

            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Visit>> GetAllAsync()
        {
            List<Visit> list = await _context.Visits.ToListAsync();
            return list;            
        }
        public async Task<List<Visit>> GetByUserIdAsync(int userId)
        {
            return await _context.Visits.Where(v => v.VisitatorId == userId || v.OwnerId == userId).ToListAsync();
        }
    }
}
