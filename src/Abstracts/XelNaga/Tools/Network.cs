using System.Net;
using System.Net.Sockets;

namespace Aiursoft.XelNaga.Tools
{
    public static class Network
    {
        private static readonly IPEndPoint DefaultLoopbackEndpoint = new(IPAddress.Loopback, port: 0);

        public static int GetAvailablePort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(DefaultLoopbackEndpoint);
            return (socket.LocalEndPoint as IPEndPoint)?.Port ?? 65534;
        }
    }
}
