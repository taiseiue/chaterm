namespace Chaterm;

public interface IChatClient
{
    public Task ReceiveMessage(Message message);
}
