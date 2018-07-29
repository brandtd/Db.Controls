using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ParrotControl
{
    public class DroneDiscovery
    {
        public static async Task<IConnectResult> Connect(IPAddress address, ushort port)
        {
            ConnectResult result = new ConnectResult();

            byte[] responseBuffer = new byte[1024];

            TcpClient tcpClient = new TcpClient();
            UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            try
            {
                await tcpClient.ConnectAsync(address, port);
                NetworkStream stream = tcpClient.GetStream();

                byte[] request = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ConnectionRequest
                {
                    ControllerName = "ParrotControl",
                    ControllerType = "PC",
                    D2cPort = (udpClient.Client.LocalEndPoint as IPEndPoint).Port,
                }));
                await stream.WriteAsync(request, 0, request.Length);
                int read = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

                if (read > 0)
                {
                    ConnectionResponse response = JsonConvert.DeserializeObject<ConnectionResponse>(Encoding.UTF8.GetString(responseBuffer));

                    if (response.Status == 0)
                    {
                        result.Error = string.Empty;
                        result.Drone = new Bebop(udpClient);
                    }
                    else
                    {
                        result.Error = $"Status error: {response.Status}";
                    }
                }
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentOutOfRangeException ||
                ex is SocketException)
            {
                result.Error = $"Unable to connect with drone: {ex.Message}";
            }
            catch (JsonException ex)
            {
                result.Error = $"JSON error: {ex.Message}";
            }

            if (result.Drone == null)
            {
                udpClient.Dispose();
            }
            tcpClient.Dispose();

            return result;
        }
    }
}