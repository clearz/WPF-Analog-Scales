using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Messure {

    using Screen = System.Windows.Forms.Screen;
    public partial class MainWindow : Window
    {

        private readonly Scales _scales;

        public MainWindow()
        {
            InitializeComponent();
            screenCmb.DataContext = Screen.AllScreens.Select((s, i) => new { Name = $"Screen {i + 1}", Id = i });

            Closing += (s, e) => _scales.DisconnectBoard();
            Closed += (s, e) => Application.Current.Shutdown();
            screen = Screen.PrimaryScreen;
            _scales = new Scales(this);
            _scales.Topmost = false;

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (hideMainChk.IsChecked ?? false)
                Hide();
            DisplayScales();
            this.Focus();
        }

        private void DisplayScales()
        {
            _scales.Show();
            _scales.WindowState = WindowState.Normal;
            _scales.Left = screen.Bounds.Left;
            _scales.Top = screen.Bounds.Top;
            _scales.Width = screen.Bounds.Width;
            _scales.Height = screen.Bounds.Height;
            _scales.Loaded += (s, ev) => ((Window)s).WindowState = WindowState.Maximized; ;
        }

        private void screenCmb_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Screen[] screens = Screen.AllScreens;
            screen = screens[screenCmb.SelectedIndex];

            DisplayScales();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // TODO handle change of screen
        }

        private Screen screen;
    }


}
