namespace BeeApps.Common.Repositories;

public interface ICRUDRepository<T>
{
    public Task<T> GetById(int id);
    public Task Insert(T entity);
    public Task Update(T entity);
    public Task<IEnumerable<T>> List();
    public Task<T> Delete();
}