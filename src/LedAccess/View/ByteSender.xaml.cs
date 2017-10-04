namespace LedAccess.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using LedAccess.ViewModel;

    /// <summary>
    /// Interaktionslogik für ByteSender.xaml
    /// </summary>
    public partial class ByteSender : Window
    {
        private ConnectionOverview viewModel;

        public ByteSender(ConnectionOverview viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            sendBytes();
        }

        private void sendBytes()
        {
            viewModel.SendRawBytes(txtRawBytes.Text.Split(' ').Select(x => Convert.ToByte(x)).ToArray());
        }

        private void BtnSend_OnKeyDown(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Enter)
            {
                sendBytes();
            }
        }
    }
}
