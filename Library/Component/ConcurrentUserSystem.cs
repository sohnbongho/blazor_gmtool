using Library.ECSSystem;
using Library.Logger;
using Library.Repository.ConcurrentUser;
using log4net;
using System.Reflection;

namespace Library.Component;

public class ConcurrentUserSystem : IECSSystem
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public static ConcurrentUserSystem Of(IConcurrentUserRepo repo)
    {
        return new ConcurrentUserSystem(repo);
    }

    private readonly IConcurrentUserRepo _repo;
    private ConcurrentUserSystem(IConcurrentUserRepo repo)
    {
        _repo = repo;
    }
    public void Save(int serverId, ConcurrentUserComponent? component)
    {
        if(component == null)
        {
            return;
        }

        var users = component.ConcurrentUsers;
        _logger.WarnEx(() => $"ConcurrenuUser serverId:{serverId} users:{users}");
        _repo.Insert(serverId, users);
    }
}
