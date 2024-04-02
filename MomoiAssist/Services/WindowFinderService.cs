using MomoiAssist.Helpers;
using MomoiAssist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MomoiAssist.Services
{

    public class WindowFinderService
    {
        private readonly IServiceProvider _serviceProvider;
        public WindowFinderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            StartWindowUpdateTimer();
        }

        private EmulatorWindow? emulatorWindow = null;

        public void ClearEmulatorWindow()
        {
            emulatorWindow = null;
        }

        private DispatcherTimer windowUpdateTimer = new DispatcherTimer();

        private void StartWindowUpdateTimer()
        {
            windowUpdateTimer.Interval = TimeSpan.FromSeconds(1); // update every 5 seconds
            windowUpdateTimer.Tick += UpdateWindow;
            windowUpdateTimer.Start();

        }
        public void UpdateWindow(object sender, EventArgs e)
        {
            if (emulatorWindow != null)
            {
                return;
            }
            emulatorWindow = GetPossibleEmulatorWindows().FirstOrDefault();
            if (emulatorWindow != null)
            {
                emulatorWindow.UpdateScreenshot();
            }
        }

        private readonly List<string> possibleEmulatorNames = new List<string> {"MuMu Player 12", "MuMu"};


        // list 3 possible emulator window at most
        // first one is the one we selected by default 
        public List<EmulatorWindow> GetPossibleEmulatorWindows()
        {
            List<EmulatorWindow> emulatorWindows = new List<EmulatorWindow>();
            foreach (IntPtr hWnd in EmulatorWindowHelper.GetAllWindows())
            {
                if (possibleEmulatorNames.Contains(EmulatorWindowHelper.GetWindowName(hWnd)))
                {
                    emulatorWindow = new EmulatorWindow(EmulatorWindowHelper.GetWindowName(hWnd), hWnd);
                    emulatorWindow.UpdateScreenshot();
                    emulatorWindows.Add(emulatorWindow);
                }
                if (emulatorWindows.Count >= 3)
                {
                    break;
                }
            }
            return emulatorWindows;
        }

        public List<string> AllWindowNames => EmulatorWindowHelper.GetAllWindowNames();

        public EmulatorWindow? GetEmulatorWindow()
        {
            if (emulatorWindow == null)
            {
                emulatorWindow = GetPossibleEmulatorWindows().FirstOrDefault();
                
            }
            if (emulatorWindow != null)
            {
                emulatorWindow.UpdateScreenshot();
            }
            return emulatorWindow;
        }

        public void SetEmulatorWindow(EmulatorWindow emulatorWindow)
        {
            this.emulatorWindow = emulatorWindow;
        }

        public void SetEmulatorWindow(IntPtr handle)
        {
            emulatorWindow = new EmulatorWindow(EmulatorWindowHelper.GetWindowName(handle), handle);
        }

        public EmulatorWindow? CurrentWindow => emulatorWindow;


    }
}
