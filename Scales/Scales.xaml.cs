using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Messure.WiimoteLib;
using Messure.Utils;
using System.Threading;
using System.Windows.Controls;

namespace Messure
{

    public partial class Scales : Window
    {
        public bool IsConnected => _balanceBoard != null && _balanceBoard.isConnected();
        public string Label {
            get => _mainWindow.statusLbl.Content.ToString();
            set => _mainWindow.statusLbl.Content = value;
        }

        public Scales(MainWindow mainWindow) {
            _mainWindow = mainWindow;
            InitializeComponent();
            KeyDown += (s, e) => {
                mainWindow.Show();
                Hide();
            };
            FindWiiboard();
        }

        public void DisconnectBoard() {
            if (IsConnected)
            {
                _balanceBoard.SetLEDs(0);
                _balanceBoard.Dispose();
                _balanceBoard = null;
            }
        }
        private async void FindWiiboard() {
            Label = "Searching...";
            var wm = await Wiimote.Find();
            // connect it and set it up as always

            Label = "Connecting...";
            wm.Connect();
            if (wm.WiimoteState.ExtensionType != ExtensionType.BalanceBoard) {

                Label = "Not a board";
                DisconnectBoard();
                return;
            }

            Label = "Got Balance Board";
            _balanceBoard = wm;
            _balanceBoard.SetLEDs(1);
            _balanceBoard.WiimoteChanged += (s, e) => { WiimoteStateChanged(e.WiimoteState); };


            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(5)};
            _timer.Tick += Timer_Tick;
            _timer.Start();

        }
        private void WiimoteStateChanged(WiimoteState state) => _weight = state.BalanceBoardState.WeightKg;
        private void Timer_Tick(object sender, object e) => transform.Angle = _weight.Clamp(0).Remap(0, 100, 0, 360).Average();



        double _weight;
        private readonly MainWindow _mainWindow;
        private Wiimote _balanceBoard = null;
        public DispatcherTimer _timer;

    }
}
