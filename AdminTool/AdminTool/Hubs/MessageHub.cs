using AdminTool.Akka;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace AdminTool.Hubs;

public class MessageHub : Hub
{
    private readonly ActorSystem _actorSystem;
    private readonly IHubContext<MessageHub> _hubContext;
    private static readonly ConcurrentDictionary<string, IActorRef> _userActors = new ConcurrentDictionary<string, IActorRef>();

    public MessageHub(ActorSystem actorSystem, IHubContext<MessageHub> hubContext)
    {
        _actorSystem = actorSystem;
        _hubContext = hubContext;
    }

    public async Task MessageToServer(byte[] messageBytes)
    {
        await Clients.All.SendAsync("MessageToClient", messageBytes);
    }

    //ST추가된 코드
    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        var userActor = _actorSystem.ActorOf(Props.Create(()
            => new UserActor(connectionId, _hubContext)), $"userActor-{connectionId}");

        _userActors[connectionId] = userActor;

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        if (!string.IsNullOrEmpty(connectionId) && _userActors.TryRemove(connectionId, out var userActor))
        {
            _actorSystem.Stop(userActor);
        }

        await base.OnDisconnectedAsync(exception);
    }
    public async Task Send(string message, string userId)
    {
        if (false == string.IsNullOrEmpty(userId))
        {
            await Clients.All.SendAsync("receive", message, userId);
        }
        else
        {
            if (userId != null)
            {
                // 나 이외에 다른 유저들에게 메시지를 보낸다.
                await Clients.Others.SendAsync("notify", "채팅방에서 1명이 퇴장했습니다.");
            }
        }


        //string connectionId = Context.ConnectionId;
        //if (_userActors.TryGetValue(connectionId, out var userActor))
        //{
        //    userActor.Tell(new UserMessage(userId, message));
        //}
        //await Clients.All.SendAsync("receive", message, userId);
        //await Clients.Client(connectionId).SendAsync("receive", message, userId);
    }

    public async Task SendToUser(string message, string targetUserId)
    {
        if (_userActors.TryGetValue(targetUserId, out var userActor))
        {
            userActor.Tell(new UserMessage(targetUserId, message));
        }

        await Clients.Client(targetUserId).SendAsync("receive", message, targetUserId);
    }
}
