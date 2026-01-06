
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.Models;

namespace RealWorldUnitTest.Web.Repositories
{
  public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
  {
    private readonly DbUnitTestContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(DbUnitTestContext dbContext)
    {
      _dbContext = dbContext;
      _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task Create(TEntity entity)
    {
      await _dbSet.AddAsync(entity);
      await _dbContext.SaveChangesAsync();
    }

    public void Delete(TEntity entity)
    {
      _dbSet.Remove(entity);
      _dbContext.SaveChanges();
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
      return await _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetById(int id)
    {
      return await _dbSet.FindAsync(id);
    }

    public void Update(TEntity entity)
    {
      _dbContext.Entry(entity).State = EntityState.Modified;
      _dbContext.SaveChanges();
    }
  }
}
