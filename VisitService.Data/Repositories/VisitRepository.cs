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

        public async Task AddAsync(Visit visit, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            await _context.Visits.AddAsync(visit, ct);

            if(outboxEvent != null) 
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            Visit? v = await _context.Visits.FindAsync(id, ct);
            if (v == null)
                throw new Exception("Visit non esistente");
            _context.Visits.Remove(v);

            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }
        public async Task<Visit?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            Visit? v = await _context.Visits.FindAsync(id, ct);
            return v;
        }


        public async Task UpdateAsync(Visit visit, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            _context.Visits.Update(visit);

            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Visit>> GetAllAsync(CancellationToken ct = default)
        {
            List<Visit> list = await _context.Visits.ToListAsync(ct);
            return list;            
        }
        public async Task<List<Visit>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Visits.Where(v => v.VisitatorId == userId || v.OwnerId == userId).ToListAsync(ct);
        }
    }
}
