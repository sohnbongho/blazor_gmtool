using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.ResponseCompression;
using AdminTool.Services.Ai;
using Library.Helper;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Anthropic.SDK;
using log4net;
using System.Reflection;
using AdminTool.Akka;
using Google.Protobuf.WellKnownTypes;
using OpenAI_API.Chat;
using OpenAI_API;
using OpenAI_API.Models;

namespace AdminTool.Hubs;
public enum AiRole{

    None = 0,

    User  = 1,
    Assist = 2,
}

public class AiChatBotMessage
{
    public AiRole AiRole { get; set; } = AiRole.None;
    public string Message { get; set; } = string.Empty;
}

public class AiChatBotMessageRequest
{
    public string AiModel { get; set; } = string.Empty;    
    public string UserId { get; set; } = string.Empty;    
    public string SystemMessage { get; set; } = string.Empty;
    public int MaxTokens { get; set; }
    public List<AiChatBotMessage> Messages { get; set; } = new();
}

public class AiChatBotMessageResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class AiChatBotHub : Hub
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    private readonly IAiService _service;
    public AiChatBotHub(IAiService service)
    {
        _service = service;
    }

    public async Task ChatGptMessageToServer(AiChatBotMessageRequest message)
    {
        var apiKey = ConstInfo.ChatGptApiKey;
        var api = new OpenAIAPI(apiKey);        
        
        var userMessage = new List<ChatMessage>();
        var systemMessage = message.SystemMessage;

        if(string.IsNullOrEmpty(systemMessage) == false)
        {
            userMessage.Add(new ChatMessage(ChatMessageRole.System, systemMessage));
        }

        foreach(var userMeessage  in message.Messages)
        {
            var userRole = userMeessage.AiRole;
            var chatMessageRole = userRole == AiRole.User ? ChatMessageRole.User : ChatMessageRole.Assistant;
            userMessage.Add(new ChatMessage(chatMessageRole, userMeessage.Message));
        }        

        var chatRequest = new ChatRequest()
        {
            Model = message.AiModel,            
            Temperature = 1.0,
            MaxTokens = message.MaxTokens,
            Messages = userMessage,            
        };

        var result = await api.Chat.CreateChatCompletionAsync(chatRequest);
        var reply = result.Choices[0].Message;

        foreach (var content in result.Choices)
        {
            var text = content.Message.TextContent;
            Console.WriteLine(text);
        }

        var response = new AiChatBotMessageResponse
        {
            UserId = "ChatBot",
            Message = reply?.TextContent ?? string.Empty,
        };

        await Clients.All.SendAsync("MessageToClient", response);
    }

    public async Task Cluade3MessageToServer(AiChatBotMessageRequest message)
    {
        var apiKey = ConstInfo.ClaudeApiKey;
        var client = new AnthropicClient(apiKey);

        var messages = new List<Message>(); 
        foreach (var userMeessage in message.Messages)
        {
            var userRole = userMeessage.AiRole;
            var chatMessageRole = userRole == AiRole.User ? RoleType.User : RoleType.Assistant;
            messages.Add(new Message(chatMessageRole, userMeessage.Message));
        }        
        
        var parameters = new MessageParameters()
        {
            Messages = messages,
            MaxTokens = message.MaxTokens,
            SystemMessage = message.SystemMessage,
            Model = message.AiModel,
            Stream = false,
            Temperature = 1.0m,
        };
        MessageResponse res = await client.Messages.GetClaudeMessageAsync(parameters);

        var response = new AiChatBotMessageResponse
        {
            UserId = "ChatBot",
            Message = res.Message.ToString(),
        };        

        //foreach(var content in res.Content)
        //{
        //    Console.WriteLine(content.Text);            
        //}

        await Clients.All.SendAsync("MessageToClient", response);
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