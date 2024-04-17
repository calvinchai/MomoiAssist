using Microsoft.Extensions.Hosting;
using MomoiAssist.Helpers;
using MomoiAssist.Models;
using System.Windows.Threading;

namespace MomoiAssist.Services
{

    public class EmulatorWindowFinderService : IHostedService
    {
        private EmulatorWindow? emulatorWindow = null;
        public EmulatorWindow? CurrentWindow => emulatorWindow;
        public EmulatorWindow? EmulatorWindow
        {
            get
            {
                if (emulatorWindow == null)
                {
                    emulatorWindow = GetPossibleEmulatorWindows().FirstOrDefault();
                }

                return emulatorWindow;

            }
            set
            {
                if (value is EmulatorWindow window)
                {
                    emulatorWindow = window;
                }


            }
        }

        private OverlayService overlayService;

        public EmulatorWindowFinderService(OverlayService overlayService)
        {
            this.overlayService = overlayService;
        }

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
                overlayService.StartOverlay(emulatorWindow);
                //emulatorWindow.UpdateScreenshot();
            }
        }

        private readonly List<string> possibleEmulatorNames = new List<string> { "MuMu Player 12", "MuMu" };


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
                    //emulatorWindow.UpdateScreenshot();
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
                //emulatorWindow.UpdateScreenshot();
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartWindowUpdateTimer();
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }



    }
}
