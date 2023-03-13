using BlazorSignalRApp.Server.Data;
using BlazorSignalRApp.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Server.Hubs;

public class ChatHub : Hub
{
    private readonly AppDataContext _db;

    public ChatHub(AppDataContext db)
    {
        _db = db;
    }


    public async Task PostMessage(ChatMessage msg)
    {
        var notification = new ChatMessageNotification
        {
            Message = msg.Message,
            User = msg.User,
            Id = Guid.NewGuid(),
            MessageTime = DateTime.Now
        };

        _db.Messages.Add(notification);
        await _db.SaveChangesAsync();

        await Clients.All.SendAsync("MessageNotification", notification);
    }
}