using System.CommandLine;
using Chaterm.Client.Utils;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chaterm.Client;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("コマンド全体の説明");


        var addressOp = new Argument<string>("url", "コマンド1-引数1 の説明");
        var contentOp = new Argument<string>("content", "コマンド1-引数1 の説明");
        var groupOp = new Option<string>("group", "コマンド1-引数1 の説明");
        groupOp.SetDefaultValue("general");
        var passwordOp = new Option<string>("--password", "コマンド1-オプション1 の説明");
        passwordOp.SetDefaultValue(string.Empty);
        var usernameOp = new Option<string>("--username", "コマンド1-オプション1 の説明");
        usernameOp.SetDefaultValue(Environment.UserName);

        var watchCmd = new Command("watch", "SwitchBotの一覧を取得します");
        root.Add(watchCmd);

        watchCmd.AddArgument(addressOp);
        watchCmd.AddOption(groupOp);
        watchCmd.AddOption(passwordOp);
        watchCmd.AddOption(usernameOp);
        watchCmd.SetHandler(Watch, addressOp, groupOp, passwordOp, usernameOp);

        var sendCmd = new Command("send", "SwitchBotの一覧を取得します");
        root.Add(sendCmd);
        sendCmd.AddArgument(addressOp);
        sendCmd.AddOption(groupOp);
        sendCmd.AddOption(passwordOp);
        sendCmd.AddOption(usernameOp);
        sendCmd.AddArgument(contentOp);
        sendCmd.SetHandler(Send, addressOp, contentOp, groupOp, passwordOp, usernameOp);


        var interactiveCmd = new Command("interactive", "SwitchBotの一覧を取得します");
        root.Add(interactiveCmd);
        interactiveCmd.AddArgument(addressOp);
        interactiveCmd.AddOption(groupOp);
        interactiveCmd.AddOption(passwordOp);
        interactiveCmd.AddOption(usernameOp);
        interactiveCmd.SetHandler(Interactive.Main, addressOp, groupOp, passwordOp, usernameOp);

        return await root.InvokeAsync(args);
    }

    static async Task Watch(string address, string groupName, string password = "", string? userName = null)
    {
        userName ??= Environment.UserName;

        ChatClient client = await ChatClient.ConnectAsync(address, groupName, password, userName, ChatClientMode.Receive);
        Console.WriteLine($"{groupName}に接続しました。");

        client.ChatReceived += (sender, e) =>
        {
            Console.WriteLine(e.Message.PrintMessage(userName));
        };

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine();
            Console.WriteLine($"{groupName}から切断します。");
            client.Dispose();
        };

        await Task.Delay(-1);
    }
    static async Task Send(string address, string content, string groupName, string password, string userName)
    {
        ChatClient client = await ChatClient.ConnectAsync(address, groupName, password, userName, ChatClientMode.Send);

        await client.SendMessage(content);
        client.Dispose();
    }
}
