using System.CommandLine;
using Chaterm.Client.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Chaterm.Client;

public class Interactive
{
    private static int consoleHeight;
    private static int logAreaHeight;
    private static int inputAreaY;
    private static List<string> logMessages = new List<string>();
    private static object lockObj = new object();
    private static bool isRunning = true;

    public static async Task Main(string address, string groupName, string password, string userName)
    {
        // コンソールの初期設定
        Console.CursorVisible = true;
        consoleHeight = Console.WindowHeight;
        logAreaHeight = consoleHeight - 3; // 入力エリア用に3行確保
        inputAreaY = logAreaHeight + 1;    // 区切り線の下

        // コンソールをクリア
        Console.Clear();

        // 区切り線を描画
        DrawSeparator();

        // 入力プロンプトを描画
        DrawInputPrompt();

        // ログ出力用のスレッドを開始

        ChatClient client = await ChatClient.ConnectAsync(address, groupName, password, userName, ChatClientMode.Receive);
        client.ChatReceived += (sender, e) =>
        {
            AddLogMessage(e.Message.PrintMessage(userName));
        };

        // ユーザー入力処理
        while (isRunning)
        {
            string input = ReadInput();

            if (!string.IsNullOrEmpty(input))
                await client.SendMessage(input);
        }
    }

    private static void DrawSeparator()
    {
        Console.SetCursorPosition(0, logAreaHeight);
        Console.Write(new string('-', Console.WindowWidth));
    }

    private static void DrawInputPrompt()
    {
        Console.SetCursorPosition(0, inputAreaY);
        Console.Write("> ");
    }

    private static string ReadInput()
    {
        int promptLength = 2; // "> " の長さ
        Console.SetCursorPosition(promptLength, inputAreaY);
        Console.Write(new string(' ', Console.WindowWidth - promptLength));
        Console.SetCursorPosition(promptLength, inputAreaY);

        string input = "";
        int cursorX = promptLength;

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.SetCursorPosition(promptLength, inputAreaY);
                Console.Write(new string(' ', Console.WindowWidth - promptLength));
                Console.SetCursorPosition(promptLength, inputAreaY);
                return input;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                cursorX--;
                Console.SetCursorPosition(cursorX, inputAreaY);
                Console.Write(" ");
                Console.SetCursorPosition(cursorX, inputAreaY);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.SetCursorPosition(cursorX, inputAreaY);
                Console.Write(keyInfo.KeyChar);
                cursorX++;
            }
        }
    }


    private static void AddLogMessage(string message)
    {
        lock (lockObj)
        {
            logMessages.Add(message);

            // 最大表示行数を超えた場合、古いメッセージを削除
            while (logMessages.Count > logAreaHeight)
            {
                logMessages.RemoveAt(0);
            }

            RefreshLogArea();
        }
    }

    private static void RefreshLogArea()
    {
        // 現在のカーソル位置を保存
        int originalX = Console.CursorLeft;
        int originalY = Console.CursorTop;

        // ログエリアをクリア
        for (int i = 0; i < logAreaHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        // ログメッセージを表示
        for (int i = 0; i < logMessages.Count; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(logMessages[i]);
        }

        // 区切り線を再描画
        DrawSeparator();

        // カーソル位置を元に戻す
        Console.SetCursorPosition(originalX, originalY);
    }
}