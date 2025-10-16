using AdminTool.Hubs;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;

namespace AdminTool.Akka;

public class UserMessage
{
    public string UserId { get; }
    public string Message { get; }

    public UserMessage(string userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}

public class UserActor : ReceiveActor
{
    private readonly string _connectionId;
    private readonly IHubContext<MessageHub> _hubContext;

    public UserActor(string connectionId, IHubContext<MessageHub> hubContext)
    {
        _connectionId = connectionId;
        _hubContext = hubContext;

        Receive<UserMessage>(async message =>
        {
            // 메시지 처리 로직
            Console.WriteLine($"Received message for {_connectionId}: {message.Message}");

            // 일반 유저에게 알림
            //await _hubContext.Clients.Client(_connectionId).SendAsync("receive", message.Message, message.UserId);

            // 전체 유저에게 알림
            await _hubContext.Clients.All.SendAsync("receive", message.Message, message.UserId);
        });
    }
}