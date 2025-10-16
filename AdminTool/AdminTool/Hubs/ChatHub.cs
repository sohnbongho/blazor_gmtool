using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.ResponseCompression;

namespace AdminTool.Hubs;

public class ChatHubMessage
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ChatHub : Hub
{
    public async Task MessageToServer(ChatHubMessage message)
    {
        await Clients.All.SendAsync("MessageToClient", message);
    }

    //ST추가된 코드
    public override async Task OnConnectedAsync()
    {
        var name = Context.User?.Identity?.Name ?? string.Empty;

        await Clients.All.SendAsync("Enter", $"{name} 채팅방에 입장");        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var name = Context.User?.Identity?.Name ?? string.Empty;

        await Clients.All.SendAsync("Exit", $"{name} 채팅방에서 나감");        
        await base.OnDisconnectedAsync(exception);
    }
}