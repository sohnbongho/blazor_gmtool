using Library.Component;
using Library.Repository;

namespace Library.ECSSystem;

public class ECSEntity : IDisposable, IComponentManager, IECSSystemManager
{
    private static long _totalId = 0;
    public long Id { get; }    
    private ComponentManager? _components = null;
    private ECSSystemManager? _systems = null;
    private RepositoryManager? _repositories = null;

    public static ECSEntity Of()
    {
        var id = Interlocked.Increment(ref _totalId);
        var entity = new ECSEntity(id);
        return entity;
    }

    private ECSEntity(long id)
    {
        Id = id;
    }
    public void Dispose()
    {
        if (_components != null)
        {
            _components.Dispose();            
            _components = null;
        }
        if (_systems != null)
        {
            _systems.Dispose();            
            _systems = null;
        }
        if (_repositories != null)
        {
            _repositories.Dispose();
            _repositories = null;
        }
    }

    /// <summary>
    /// Component
    /// </summary>    
    public T AddComponent<T>(T component) where T : class, IECSComponent
    {
        if (_components == null)
        {
            _components = ComponentManager.Of();
        }

        return _components.AddComponent<T>(component);        
    }


    public void RemoveComponent<T>() where T : class, IECSComponent
    {
        if (_components == null)
            return;

        _components.RemoveComponent<T>();        
    }
    

    public T? GetComponent<T>() where T : class, IECSComponent
    {
        if (_components == null)
            return null;

        return _components.GetComponent<T>();
        
    }

    /// <summary>
    /// system
    /// </summary>    
    public T AddSystem<T>(T system) where T : class, IECSSystem
    {
        if (_systems == null)
        {
            _systems = ECSSystemManager.Of();
        }
        return _systems.AddSystem<T>(system);        
    }
    public T? GetSystem<T>() where T : class, IECSSystem
    {
        if (_systems == null)
            return null;

        return _systems.GetSystem<T>();
    }

    public void RemoveSystem<T>() where T : class, IECSSystem
    {        
        if (_systems != null)
        {
            _systems.RemoveSystem<T>();
        }
    }

    /// <summary>
    /// Repository
    /// </summary>    
    public T AddRepository<T>(T repository) where T : class, IECSRepository
    {
        if (_repositories == null)
        {
            _repositories = RepositoryManager.Of();
        }
        _repositories.Add(repository);
        return repository;
    }
    public T? GetRepository<T>() where T : class, IECSRepository
    {
        if (_repositories == null)
            return null;

        return _repositories.Get<T>();
    }

    public void RemoveRepository<T>() where T : class, IECSRepository
    {
        if (_repositories == null)
            return;

        _repositories.Remove<T>();
    }
}

