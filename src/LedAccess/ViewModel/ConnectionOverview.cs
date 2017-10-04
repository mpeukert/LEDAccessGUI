namespace LedAccess.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using Model;

    public class ConnectionOverview : NotifyBaseClass
    {
        private Discovery udpDiscovery;

        private ObservableCollection<LEDConnection> allConnections;
        private int selectedConnectionIndex;
        private String selectedConnectionName;
        private String selectedConnectionState = String.Empty;
        private String selectedConnectionConnectionInfo;
        private String currentButtonText;
        private int redValue;
        private int greenValue;
        private int blueValue;
        private int brightness;
        private int temperature;

        private bool tempModeEnabled;

        private Brush currentStateBrush;
        private Brush selectedRGBBrush;
        
        //attributes for program selection
        private int selectedProgramIndex;
        private ObservableCollection<Program> programList;
        
        private String programTooltip;

        private String firstParamName;
        private int firstParamVal;
        private int firstMin;
        private int firstMax;

        private String secondParamName;
        private int secondParamVal;
        private int secondMin;
        private int secondMax;

        private String thirdParamName;
        private int thirdParamVal;
        private int thirdMin;
        private int thirdMax;

        public ConnectionOverview()
        {
            allConnections = new ObservableCollection<LEDConnection>();
            udpDiscovery = new Discovery(this, 1337);
        }


        public void GetClients()
        {
            udpDiscovery.GatherClients();
        }

        public void GetNewSelectedConnection(int newIndex)
        {
            if (newIndex >= 0)
            {
                selectedConnectionIndex = newIndex;
                SelectedConnectionConntectionInfo = allConnections[selectedConnectionIndex].ConnectionInfo;
                SelectedConnectionName = allConnections[selectedConnectionIndex].Name;
                SelectedConnectionState = allConnections[selectedConnectionIndex].State;
            }
            ControllerSelected = true; //dummy call
        }

        public void RefreshPrograms()
        {
            ProgramList = allConnections[selectedConnectionIndex].GetPrograms();
        }

        public void GetNewSelectedProgram(int newIndex)
        {
            if (newIndex >= 0)
            {
                selectedProgramIndex = newIndex;
                
                SelectedProgramTooltip = programList[selectedProgramIndex].ProgramTooltip;

                FirstParameterName = programList[selectedProgramIndex].FirstParameter.ParameterName;
                FirstMin = programList[selectedProgramIndex].FirstParameter.MinValue;
                FirstMax = programList[selectedProgramIndex].FirstParameter.MaxValue;

                SecondParameterName = programList[selectedProgramIndex].SecondParameter.ParameterName;
                SecondMin = programList[selectedProgramIndex].SecondParameter.MinValue;
                SecondMax = programList[selectedProgramIndex].SecondParameter.MaxValue;

                ThirdParameterName = programList[selectedProgramIndex].ThirdParameter.ParameterName;
                ThirdMin = programList[selectedProgramIndex].ThirdParameter.MinValue;
                ThirdMax = programList[selectedProgramIndex].ThirdParameter.MaxValue;
            }
        }

        public void ActivateSelectedProgram(int selectedProgramIndex)
        {
            allConnections[selectedConnectionIndex].ActivateProgram(selectedProgramIndex);
        }

        public void TransmitRGBColor()
        {
            allConnections[selectedConnectionIndex].TransmitRGB((byte) RedValue, (byte) GreenValue, (byte) BlueValue);
        }
        public void TransmitBrightness()
        {
            allConnections[selectedConnectionIndex].SetBrightness((byte) Brightness);
        }

        public void RebootCurrent()
        {
            allConnections[selectedConnectionIndex].Reboot();
            SelectedConnectionState = "disconnected";
            allConnections.Remove(allConnections[selectedConnectionIndex]);
        }

        public void SaveToEprom()
        {
            allConnections[selectedConnectionIndex].SaveToEprom();
        }

        public void StandbyMode(bool state)
        {
            allConnections[selectedConnectionIndex].StandbyMode(Convert.ToByte(state));
        }

        public void PrintMessageOnConnection(String messageContent)
        {
            allConnections[selectedConnectionIndex].PrintMessage(messageContent);
        }

        public void ConnectAction()
        {
            if (selectedConnectionState.Equals("connected"))
            {
                allConnections[selectedConnectionIndex].Disconnect();
            }
            else if(selectedConnectionState.Equals("disconnected"))
            {
                allConnections[selectedConnectionIndex].Connect();
            }
            SelectedConnectionState = allConnections[selectedConnectionIndex].State;
        }

        public void SendRawBytes(byte[] rawBytes)
        {
            allConnections[selectedConnectionIndex].ExecuteRawByteCmd(rawBytes);
        }
        
        public void UpdateColorPanel()
        {
            if (TemperatureModeEnabled)
            {
                Color tempColor = TemperatureRGBConverter.KelvinToRGB(ColorTemperature);
                SelectedRGBBrush = new SolidColorBrush(tempColor);
                RedValue = tempColor.R;
                GreenValue = tempColor.G;
                BlueValue = tempColor.B;
            }
            else
            {
                SelectedRGBBrush = new SolidColorBrush(Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue));
            }
        }

        //GUI Binding Properties
        public ObservableCollection<LEDConnection> AllConnections
        {
            get { return allConnections; }
            set
            {
                allConnections = value;
                OnPropertyChanged();
            }
        }

        public String SelectedConnectionConntectionInfo
        {
            get { return selectedConnectionConnectionInfo; }
            set
            {
                selectedConnectionConnectionInfo = value;
                OnPropertyChanged();
            }
        }

        public String SelectedConnectionState
        {
            get { return selectedConnectionState; }
            set
            {
                selectedConnectionState = value;
                if (selectedConnectionState.Equals("connected"))
                {
                    CurrentStateBrush = Brushes.Green;
                    CurrentButtonText = "Disconnect";
                }
                else if(selectedConnectionState.Equals("disconnected"))
                {
                    CurrentStateBrush = Brushes.Red;
                    CurrentButtonText = "Connect";
                }
                ControllerConnected = true; //dummy expression
                OnPropertyChanged();
            }
        }

        public bool ControllerSelected
        {
            get
            {
                try
                {
                    if (selectedConnectionIndex > -1)
                    {
                        if (allConnections[selectedConnectionIndex] != null)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public bool ControllerConnected
        {
            get { return SelectedConnectionState.Equals("connected"); }
            set
            {
                OnPropertyChanged();
            }
        }

        public String CurrentButtonText
        {
            get { return currentButtonText; }
            set
            {
                currentButtonText = value;
                OnPropertyChanged();
            }
        }

        public String SelectedConnectionName
        {
            get { return selectedConnectionName; }
            set
            {
                selectedConnectionName = value;
                OnPropertyChanged();
            }
        }

        public bool TemperatureModeEnabled
        {
            get { return this.tempModeEnabled; }
            set
            {
                this.tempModeEnabled = value;
                TemperatureModeDisabled = value;
                OnPropertyChanged();
            }
        }

        public bool TemperatureModeDisabled
        {
            get { return !tempModeEnabled; }
            set
            {
                OnPropertyChanged();
            }
        }

        public int ColorTemperature
        {
            get { return this.temperature; }
            set
            {
                this.temperature = value;
                OnPropertyChanged();
            }
        }

        public Brush CurrentStateBrush
        {
            get { return currentStateBrush; }
            set
            {
                currentStateBrush = value;
                OnPropertyChanged();
            }
        }

        public int RedValue
        {
            get { return redValue; }
            set
            {
                redValue = value;
                OnPropertyChanged();
            }
        }

        public int GreenValue
        {
            get { return greenValue; }
            set
            {
                greenValue = value;
                OnPropertyChanged();
            }
        }

        public int BlueValue
        {
            get { return blueValue; }
            set
            {
                blueValue = value;
                OnPropertyChanged();
            }
        }

        public int Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnPropertyChanged();
            }
        }

        public Brush SelectedRGBBrush
        {
            get { return selectedRGBBrush; }
            set
            {
                selectedRGBBrush = value;
                OnPropertyChanged();
            }
        }

        //Binding properties for program selection tab
        public ObservableCollection<Program> ProgramList
        {
            get { return this.programList; }
            set
            {
                this.programList = value;
                OnPropertyChanged();
            }
        }

        public String SelectedProgramTooltip
        {
            get { return this.programTooltip; }
            set
            {
                this.programTooltip = value;
                OnPropertyChanged();
            }
        }

        public String FirstParameterName
        {
            get { return this.firstParamName; }
            set
            {
                this.firstParamName = value;
                OnPropertyChanged();
            }
        }

        public String SecondParameterName
        {
            get { return this.secondParamName; }
            set
            {
                this.secondParamName = value;
                OnPropertyChanged();
            }
        }
        public String ThirdParameterName
        {
            get { return this.thirdParamName; }
            set
            {
                this.thirdParamName = value;
                OnPropertyChanged();
            }
        }

        public int FirstParamVal
        {
            get { return this.firstParamVal; }
            set
            {
                this.firstParamVal = value;
                programList[selectedProgramIndex].FirstParameter.Value = value;
                OnPropertyChanged();
            }
        }

        public int SecondParamVal
        {
            get { return this.secondParamVal; }
            set
            {
                this.secondParamVal = value;
                programList[selectedProgramIndex].SecondParameter.Value = value;
                OnPropertyChanged();
            }
        }

        public int ThirdParamVal
        {
            get { return this.thirdParamVal; }
            set
            {
                this.thirdParamVal = value;
                programList[selectedProgramIndex].ThirdParameter.Value = value;
                OnPropertyChanged();
            }
        }

        public int FirstMin
        {
            get { return this.firstMin; }
            set
            {
                this.firstMin = value;
                OnPropertyChanged();
            }
        }

        public int FirstMax
        {
            get { return this.firstMax; }
            set
            {
                this.firstMax = value;
                OnPropertyChanged();
            }
        }

        public int SecondMin
        {
            get { return this.secondMin; }
            set
            {
                this.secondMin = value;
                OnPropertyChanged();
            }
        }

        public int SecondMax
        {
            get { return this.secondMax; }
            set
            {
                this.secondMax = value;
                OnPropertyChanged();
            }
        }

        public int ThirdMin
        {
            get { return this.thirdMin; }
            set
            {
                this.thirdMin = value;
                OnPropertyChanged();
            }
        }

        public int ThirdMax
        {
            get { return this.thirdMax; }
            set
            {
                this.thirdMax = value;
                OnPropertyChanged();
            }
        }
    }
}
