using Microsoft.EntityFrameworkCore;
using VisitService.Data.Context;
using VisitService.Repository.Entities;
using VisitService.Repository.Interfaces;

namespace VisitService.Data.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        private readonly VisitDbContext context;

        public VisitRepository(VisitDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Visit visit)
        {
            await context.AddAsync(visit);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Visit? v = await context.visits.FindAsync(id);
            if (v == null)
                throw new Exception("Property non esistente");
            context.visits.Remove(v);
            await context.SaveChangesAsync();
        }
        public async Task<Visit?> GetByIdAsync(int id)
        {
            Visit? v = await context.visits.FindAsync(id);
            return v;
        }


        public async Task UpdateAsync(Visit visit)
        {
            context.visits.Update(visit);
            await context.SaveChangesAsync();
        }

        public async Task<List<Visit>> GetAllAsync()
        {
            List<Visit> list = await context.visits.ToListAsync();
            return list;            
        }
        public async Task<List<Visit>> GetByUserIdAsync(int userId)
        {
            return await context.visits.Where(v => v.VisitatorId == userId || v.OwnerId == userId).ToListAsync();
        }
    }
}
