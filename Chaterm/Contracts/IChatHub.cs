namespace Chaterm;

public interface IChatHub
{
    public Task SendMessage(Message message);
    public Task JoinGroup(string groupSecret);
    public Task LeaveGroup(string groupSecret);
}
