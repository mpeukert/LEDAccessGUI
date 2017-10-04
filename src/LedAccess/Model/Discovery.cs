namespace LedAccess
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ViewModel;

    public class Discovery : NotifyBaseClass
    {
        private const String DISCOVERY_ANSWER = "lam_id";
        private const String DISCOVERY_REQ = "lam_discovery";
        
        private UdpClient udpSender;
        private UdpClient udpReceiver;
        private ConnectionOverview viewModel;

        private Task receiveTask;
        private Task broadcastTask;
        private Stopwatch taskWatch;

        private int port;

        private bool gatherClients = false;

        #region constructors
        public Discovery(ConnectionOverview viewModel, int port)
        {
            this.viewModel = viewModel;
            this.port = port;
            broadcastTask = new Task(SendBroadcast);
            receiveTask = new Task(ReceiveTask);
            taskWatch = new Stopwatch();
            ConnectClients();
        }
        #endregion constructors

        #region functions
        public void GatherClients()
        {
            if (!gatherClients)
            {
                if (broadcastTask.Status == TaskStatus.Running)
                {
                    broadcastTask.Wait();
                }

                if (receiveTask.Status == TaskStatus.Running)
                {
                    receiveTask.Wait();
                }
                broadcastTask = new Task(SendBroadcast);
                receiveTask = new Task(ReceiveTask);

                gatherClients = true;

                taskWatch.Reset();
                taskWatch.Start();
                receiveTask.Start();
                broadcastTask.Start();
            }
        }

        private void ConnectClients()
        {
            udpReceiver = new UdpClient(port);
            udpReceiver.EnableBroadcast = true;

            udpSender = new UdpClient();
            udpSender.Connect(new IPEndPoint(IPAddress.Broadcast, port));
            udpSender.EnableBroadcast = true;
        }

        private void DecodeMessage(String message, IPEndPoint remoteEndPoint)
        {
            if (message.StartsWith(DISCOVERY_ANSWER))
            {
                int start = message.IndexOf(DISCOVERY_ANSWER);
                String name = message.Substring(start + DISCOVERY_ANSWER.Length);
                LEDConnection connection = new LEDConnection(name, remoteEndPoint.Address.ToString(), remoteEndPoint.Port);
                if (!viewModel.AllConnections.Contains(connection))
                {

                    viewModel.AllConnections.AddOnUI(connection);
                }
            }
        }

        //task functions
        private void ReceiveTask()
        {
            int failedReadAttempts = 0;

            while (gatherClients)
            {
                byte[] recBytes = new byte[64];
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                if (udpReceiver.Available > 0)
                {
                    recBytes = udpReceiver.Receive(ref remoteEndPoint);
                    String decodedMsg = Encoding.ASCII.GetString(recBytes);
                    if (decodedMsg.ToLower().Contains(DISCOVERY_ANSWER))
                    {
                        DecodeMessage(decodedMsg, remoteEndPoint);
                    }
                }
                else
                {
                    failedReadAttempts++;
                }

                if (failedReadAttempts >= 50 || taskWatch.ElapsedMilliseconds > 30000)
                {
                    taskWatch.Stop();
                    gatherClients = false;
                }

                Thread.Sleep(200);
            }
        }

        private void SendBroadcast()
        {
            while (gatherClients)
            {
                byte[] rawBroadcast = Encoding.ASCII.GetBytes(DISCOVERY_REQ);
                udpSender.Send(rawBroadcast, rawBroadcast.Length);
                Thread.Sleep(1000);
            }
        }
        #endregion functions
    }
}
