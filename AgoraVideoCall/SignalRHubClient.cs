using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace AgoraVideoCall
{
    public class SignalRHubClient
    {
        private static SignalRHubClient _instance;

        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public SignalRHubClient(string serverUrl, string hubName)
        {
            _connection = new HubConnection(serverUrl);
            _proxy = _connection.CreateHubProxy(hubName);
        }

        public static SignalRHubClient Instance =>
            _instance ??= new SignalRHubClient("http://192.168.1.2:59062/", "ServerHub");

        public event EventHandler<string> OnReceiveEvent;

        public async Task Connect(string id)
        {
            await _connection.Start();
            var work = _proxy.Invoke("Register", id);
            _proxy.On("SendMessage", (string userId, string content) => { OnReceiveEvent?.Invoke(userId, content); });
        }

        public Task Send(string username, string message)
        {
            var serverMethod = "Send";
            if (_connection.State != ConnectionState.Connected) return null;

            return _proxy.Invoke(serverMethod, username, message);
        }
    }
}