using System.Collections.Generic;

namespace AdminTool.Repositories;
public interface IDataRepository<T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    Task TruncateAndInsertAsync(IEnumerable<T> entities);
}
