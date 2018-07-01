using System;
using System.Diagnostics;

namespace Messure.WiimoteLib {
    internal class WiiBB {
        private static WiiBB wiibb;
        private Wiimote wmm;
        public event Action<WiimoteState> WiimoteState;
        private WiiBB(Action<WiimoteState> wiimoteStateChanged)
        {
            var wmc = new WiimoteCollection();
            WiimoteState += wiimoteStateChanged;
            int index = 1;

            try {
                wmc.FindAllWiimotes();
            }
            catch ( Exception ex ) {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Found: {0} devices", wmc.Count);
            foreach ( Wiimote wm in wmc ) {
                wmm = wm;
                // connect it and set it up as always
                wm.WiimoteChanged += (s, e) => {
                    WiimoteState?.Invoke(e.WiimoteState);
                };
                wm.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;

                wm.Connect();
                if ( wm.WiimoteState.ExtensionType != ExtensionType.BalanceBoard )
                    wm.SetReportType(InputReport.IRAccel, true);

                wm.SetLEDs(index++);
            }
        }

        internal static void Close()
        {
            wiibb.wmm.Disconnect();
        }

        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
        {
            Console.WriteLine(e.Inserted);
        }

        internal static void Init(Action<WiimoteState> wiimoteStateChanged)
        {
            wiibb = new WiiBB(wiimoteStateChanged);
        }
    }
}
