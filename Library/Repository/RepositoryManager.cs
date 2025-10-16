namespace Library.Repository;

public interface IRepositoryManager
{
    T Add<T>(T repository) where T : class, IECSRepository;
    T? Get<T>() where T : class, IECSRepository;
    void Remove<T>() where T : class, IECSRepository;
}

public class RepositoryManager : IRepositoryManager, IDisposable
{
    private Dictionary<Type, object>? _repositories = null;
    public static RepositoryManager Of()
    {
        return new RepositoryManager();
    }
    private RepositoryManager()
    {

    }

    public void Dispose()
    {
        if (_repositories != null)
        {
            foreach (var repository in _repositories.Values)
            {
                if (repository is IDisposable disposable)
                {
                    disposable.Dispose(); // 리소스는 해제되지만, GC와는 별개                        
                }
            }
            _repositories.Clear();
            _repositories = null;
        }
    }

    public T Add<T>(T repository) where T : class, IECSRepository
    {
        if (_repositories == null)
        {
            _repositories = new Dictionary<Type, object>();
        }
        _repositories[typeof(T)] = repository;
        return repository;

    }
    public T? Get<T>() where T : class, IECSRepository
    {
        if (_repositories == null)
        {
            _repositories = new Dictionary<Type, object>();
        }
        if (false == _repositories.TryGetValue(typeof(T), out object? repository))
        {
            return null;
        }
        if (repository == null)
        {
            return null;
        }
        return repository as T;
    }
    public void Remove<T>() where T : class, IECSRepository
    {
        T? repository = Get<T>();
        if (repository is IDisposable disposable)
        {
            disposable.Dispose();
        }
        if (_repositories != null)
        {
            _repositories.Remove(typeof(T));
        }
    }
}
