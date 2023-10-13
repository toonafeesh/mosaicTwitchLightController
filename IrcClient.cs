using System;
using System.IO;
using System.Net.Sockets;

namespace lightApp
{
    public class IrcClient
    {
        private TcpClient tcpClient;
        private StreamWriter outputStream;
        private readonly string _ip;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;


        public IrcClient()
        {
            _ip = "irc.twitch.tv";
            _port = 6667;
            _username = "Username";
            _password = "oauth:xxxxx";

            tcpClient = new TcpClient(_ip, _port);
            outputStream = new StreamWriter(tcpClient.GetStream());
            outputStream.WriteLine($"PASS {_password}");
            outputStream.WriteLine($"NICK {_username}");
            outputStream.WriteLine($"USER {_username} 8 * :{_username}");
        }

        public void SendMessage(string message, string channel)
        {
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();

            var msg = $":{_username}!{_username}@{_username}.tmi.twitch.tv PRIVMSG #{channel} :{message}";
            outputStream.WriteLine(msg);
            outputStream.Flush();
        }

        public void CloseIrcConntection()
        {
            tcpClient.Close();
        }
    }
}
