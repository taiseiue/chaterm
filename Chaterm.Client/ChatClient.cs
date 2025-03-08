using System.CommandLine;
using Chaterm.Client.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Chaterm.Client;

public class ChatClient : IChatClient, IHubConnectionObserver, IDisposable
{
    private ChatClient(IChatHub hub)
    {
        Hub = hub;
    }
    public IChatHub Hub { get; }
    public HubConnection Connection { get; private set; }
    public string GroupSecret { get; private set; }
    public string UserName { get; private set; }
    public event ChatReceivedEventHandler ChatReceived;
    public static async Task<ChatClient> ConnectAsync(string address, string groupName, string password, string userName, ChatClientMode clientMode)
    {
        HubConnection connection = new HubConnectionBuilder().WithUrl(address).Build();
        var hub = connection.CreateHubProxy<IChatHub>();

        ChatClient client = new(hub)
        {
            UserName = userName,
            Connection = connection
        };

        connection.Register<IChatClient>(client);

        await connection.StartAsync();
        string groupSecret = CreateGroupSecret(groupName, password);

        if (clientMode.HasFlag(ChatClientMode.Receive))
            await client.Hub.JoinGroup(groupSecret);

        client.GroupSecret = groupSecret;

        return client;
    }
    private static string CreateGroupSecret(string groupName, string password)
    {
        return SecretGenerator.ComputeHash($"{groupName}:{password}");
    }
    public async Task SendMessage(Message message)
    {
        message.GroupSecret = GroupSecret;
        message.UserName = UserName;
        await Hub.SendMessage(message);
    }
    public async Task SendMessage(string content)
    {
        await SendMessage(new Message { Content = content });
    }

    public async Task ReceiveMessage(Message message)
    {
        ChatReceived?.Invoke(this, new ChatReceivedEventArgs(message));
    }

    public Task OnClosed(Exception? exception)
    {
        this.Dispose();
        return Task.CompletedTask;
    }

    public Task OnReconnected(string? connectionId)
    {
        throw new NotImplementedException();
    }

    public Task OnReconnecting(Exception? exception)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        Hub.LeaveGroup(GroupSecret).Wait();
        Connection.DisposeAsync().GetAwaiter().GetResult();
    }
}

public delegate void ChatReceivedEventHandler(ChatClient sender, ChatReceivedEventArgs e);
public class ChatReceivedEventArgs : EventArgs
{
    public ChatReceivedEventArgs(Message message)
    {
        Message = message;
    }
    public Message Message { get; }
}

[Flags]
public enum ChatClientMode
{
    Receive = 0b01,
    Send = 0b10
}