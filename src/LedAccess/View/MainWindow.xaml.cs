using System;
using LedAccess.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LedAccess.View;

namespace LedAccess
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConnectionOverview viewModel;
        private ByteSender rawByteSender;

        public MainWindow()
        {
            viewModel = new ConnectionOverview();
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GetClients();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.GetNewSelectedConnection(listBox.SelectedIndex);
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ConnectAction();
        }

        private void SliderBLue_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SliderGreen_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SliderRed_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SliderRed_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SliderGreen_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SliderBLue_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.UpdateColorPanel();
        }

        private void SendRGB(object sender, MouseButtonEventArgs e)
        {
            if (chkBoxLiveTransmit.IsChecked.Value)
            {
                viewModel.TransmitRGBColor();
            }
        }

        private void SendBrightness(object sender, MouseButtonEventArgs e)
        {
            viewModel.TransmitBrightness();
        }

        private void btnRebbot_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RebootCurrent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveToEprom();
        }

        private void chkBoxStandby_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.StandbyMode(false);
        }

        private void ChkBoxStandby_OnUnchecked(object sender, RoutedEventArgs e)
        {
            viewModel.StandbyMode(true);
        }

        private void btnByteSender_Click(object sender, RoutedEventArgs e)
        {
            rawByteSender = new ByteSender(viewModel);
            rawByteSender.Show();
        }

        private void ComboBoxPrograms_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.GetNewSelectedProgram(comboBoxPrograms.SelectedIndex);
        }

        private void btnPrintMsg_Click(object sender, RoutedEventArgs e)
        {
            viewModel.PrintMessageOnConnection(txtMsgContent.Text);
        }

        private void BtnPrintMsg_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                viewModel.PrintMessageOnConnection(txtMsgContent.Text);
            }
        }

        private void btnActivateProgram_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ActivateSelectedProgram(comboBoxPrograms.SelectedIndex);
        }

        private void SliderTemp_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.UpdateColorPanel();
        }

        private void btnActivateRGB_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TransmitRGBColor();
        }

        private void ChkBoxLiveTransmit_OnChecked(object sender, RoutedEventArgs e)
        {
            viewModel.TransmitRGBColor();
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.RefreshPrograms();
        }
    }
}
