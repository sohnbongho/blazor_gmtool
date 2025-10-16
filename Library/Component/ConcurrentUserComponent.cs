namespace Library.Component;

public class ConcurrentUserComponent : IECSComponent
{
    public int ConcurrentUsers => _concurrentUsers;
    private int _concurrentUsers = 0;

    public void Increment() => ++_concurrentUsers;
    public void Decrement()
    {
        _concurrentUsers--;
        _concurrentUsers = Math.Max(0, _concurrentUsers);
    }

}
