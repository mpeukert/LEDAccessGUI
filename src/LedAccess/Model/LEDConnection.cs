namespace LedAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;

    public class LEDConnection
    {
        private Socket tcpConnection;

        private String name;
        private String ipAddress;
        private int port;

        private bool receiving;

        private ObservableCollection<Program> programList;
        private Task receiveTask;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        #region constructors
        public LEDConnection(String name, String ipAddress, int port)
        {
            this.name = name;
            this.ipAddress = ipAddress;
            this.port = port;
            tcpConnection = new Socket(AddressFamily.InterNetwork ,SocketType.Stream, ProtocolType.Tcp);
            receiving = false;

            programList = new ObservableCollection<Program>();
        }
        #endregion constructors

        #region properties
        public String Name
        {
            get { return this.name; }
        }

        public String IPAddress
        {
            get { return this.ipAddress; }
        }
        
        public int Port
        {
            get { return this.port; }
        }

        public String ConnectionInfo
        {
            get { return $"{ipAddress}:{port}"; }
        }
        #endregion properties

        #region functions
        public override string ToString()
        {
            return $"Name: {name} ({ipAddress}:{port})";
        }

        public void Connect()
        {
            tcpConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpConnection.Connect(new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port));

            StartReceive();

            requestPrograms();
        }

        public void Disconnect()
        {
            StopReceive();
            
            tcpConnection.Shutdown(SocketShutdown.Both);
            tcpConnection.Close();
        }

        public String State
        {
            get
            {
                if (tcpConnection.Connected)
                {
                    return "connected";
                }
                else
                {
                    return "disconnected";
                }
            }
        }

        public override bool Equals(object obj)
        {
            LEDConnection other = (LEDConnection) obj;
            return this.ipAddress == other.ipAddress;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public ObservableCollection<Program> GetPrograms()
        {
            //transmit commands
            //select programm:
            // 2 
            //  0 : twinkle
            //  1 : rainbow
            //  2 : std red
            //  3 : rgb

            return programList;
        }

        public void TransmitRGB(byte red, byte green, byte blue)
        {
            transmit( new byte[] {2, 3, red, green, blue });
        }

        public void Reboot()
        {
            transmit( new byte[] { 255 });
        }

        public void SaveToEprom()
        {
            transmit( new byte[] { 128 });
        }

        public void SetBrightness(byte brightness)
        {
            transmit(new byte[] {1, brightness });
        }

        public void StandbyMode(byte state)
        {
            transmit( new byte[] { 0, state });
        }

        public void ExecuteRawByteCmd(byte[] rawBytes)
        {
            transmit(rawBytes);
        }

        public void PrintMessage(String messageContent)
        {
            List<byte> txBytes = new List<byte>();
            txBytes.Add((byte) 88);
            txBytes.AddRange(Encoding.ASCII.GetBytes(messageContent));
            txBytes.Add((byte) 0);                                          //0 termination
            transmit(txBytes.ToArray());
        }

        public void ActivateProgram(int selectedProgramIndex)
        {
            transmit(GetPrograms()[selectedProgramIndex].GetByteEncoding());
        }

        private void StartReceive()
        {
            receiving = true;
            CancellationToken token = cancellationTokenSource.Token;
            receiveTask = new Task(() => receive(), token);

            receiveTask.Start();
        }

        private void StopReceive()
        {
            receiving = false;
            cancellationTokenSource.Cancel();
            //Thread.Sleep(250);
        }

        private void transmit(byte[] txBuffer)
        {
            try
            {
                tcpConnection.Send(txBuffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void receive()
        {
            while (receiving)
            {
                byte[] receiveBuffer = new byte[256];
                try
                {
                    if (tcpConnection.Receive(receiveBuffer) > 0)
                    {
                        String messageString = Encoding.ASCII.GetString(receiveBuffer);
                        processMessage(messageString);
                    }
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void processMessage(String message)
        {
            if (message.Contains("prog"))
            {
                List<String> splitMsg = new List<string>( message.Split(new string[] {"prog;"}, StringSplitOptions.RemoveEmptyEntries));

                foreach (string prog in splitMsg)
                {
                    List<String> splitProg = new List<string>(prog.Split(';'));

                    int progNumber = Convert.ToInt32(splitProg[0]);
                    String progName = splitProg[1];
                    String toolTip = splitProg[2];
                    
                    Parameter[] parameters = new Parameter[3] {new Parameter(), new Parameter(), new Parameter()};

                    try
                    {
                        for (int i = 3; i < splitProg.Count; i += 3)
                        {
                            String paramName = splitProg[i];
                            int paramMin = Convert.ToInt32(splitProg[i + 1]);
                            int paramMax = Convert.ToInt32(splitProg[i + 2]);

                            parameters[i/3 - 1] = new Parameter(paramName, paramMin, paramMax);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    programList.Add(new Program(progName, toolTip, parameters[0], parameters[1], parameters[2], progNumber));
                }
            }
        }

        private void requestPrograms()
        {
            transmit(new byte[] {89});
        }
        #endregion functions
    }
}
