using Microsoft.Extensions.Hosting;
using MomoiAssist.Helpers;
using MomoiAssist.Helpers.PInvoke;
using MomoiAssist.Models;
using MomoiAssist.Views.Overlays;
using System.Windows.Interop;
using static MomoiAssist.Helpers.PInvoke.User32;

namespace MomoiAssist.Services
{
    public class OverlayService : IHostedService
    {
        private EmulatorWindow? emulatorWindow;

        public OverlayService()
        {


        }

        WinEventDelegate? winEventProc = null;
        IntPtr winEventHook, winEventHook2;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != emulatorWindow.Handle)
            {
                return;
            }
            Console.WriteLine("Event: " + eventType + " hwnd: " + hwnd);
            if (eventType == (uint)WinEvent.EVENT_SYSTEM_MOVESIZESTART)
            {
                // 窗口开始移动
                // Window starts moving
                masterOverlay.Hide();
                UnhookWinEvent(winEventHook2);
            }
            else if (eventType == (uint)WinEvent.EVENT_SYSTEM_MOVESIZEEND)
            {
                GetWindowThreadProcessId(emulatorWindow.Handle, out int processId);
                winEventHook2 = SetWinEventHook(
                    (uint)WinEvent.EVENT_OBJECT_LOCATIONCHANGE, (uint)WinEvent.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, winEventProc, (uint)processId, 0, 0);
                emulatorWindow.UpdateRect(null, null);
                masterOverlay.Top = emulatorWindow.WindowRect.Top;
                masterOverlay.Left = emulatorWindow.WindowRect.Left;
                masterOverlay.Width = emulatorWindow.WindowRect.Right - emulatorWindow.WindowRect.Left;
                masterOverlay.Height = emulatorWindow.WindowRect.Bottom - emulatorWindow.WindowRect.Top;
                masterOverlay.Show();
            }
            else
            {
                emulatorWindow.UpdateRect(null, null);
                masterOverlay.Top = emulatorWindow.WindowRect.Top;
                masterOverlay.Left = emulatorWindow.WindowRect.Left;
                masterOverlay.Width = emulatorWindow.WindowRect.Right - emulatorWindow.WindowRect.Left;
                masterOverlay.Height = emulatorWindow.WindowRect.Bottom - emulatorWindow.WindowRect.Top;
                masterOverlay.Show();
            }

        }



        public void StartOverlay(EmulatorWindow emulatorWindow)
        {

            this.emulatorWindow = emulatorWindow;
            winEventProc = new WinEventDelegate(OnWinEvent);
            GetWindowThreadProcessId(emulatorWindow.Handle, out int processId);
            winEventHook = SetWinEventHook(
                (uint)WinEvent.EVENT_SYSTEM_MOVESIZESTART, (uint)WinEvent.EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, winEventProc, (uint)processId, 0, 0);
            winEventHook2 = SetWinEventHook(
                (uint)WinEvent.EVENT_OBJECT_LOCATIONCHANGE, (uint)WinEvent.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, winEventProc, (uint)processId, 0, 0);

            StartMasterOverlay();
        }

        public void StopOverlay()
        {
            UnhookWinEvent(winEventHook);
            UnhookWinEvent(winEventHook2);
            masterOverlay?.Close();
        }

        MasterOverlay? masterOverlay;
        SampleOverlay? slaveOverlay;
        private void StartMasterOverlay()
        {
            masterOverlay = new MasterOverlay();

            WindowHelper.SetToolWindow(masterOverlay);
            emulatorWindow.UpdateRect(null, null);

            masterOverlay.Top = emulatorWindow.WindowRect.Top;
            masterOverlay.Left = emulatorWindow.WindowRect.Left;
            masterOverlay.Width = emulatorWindow.WindowRect.Right - emulatorWindow.WindowRect.Left;
            masterOverlay.Height = emulatorWindow.WindowRect.Bottom - emulatorWindow.WindowRect.Top;

            var hwnd = new WindowInteropHelper(masterOverlay);
            hwnd.Owner = emulatorWindow.Handle;

            masterOverlay.Show();

            MakeWindowClickThrough(masterOverlay);

            slaveOverlay = new SampleOverlay();
            //slaveOverlay.Top = emulatorWindow.Rect.Top;
            slaveOverlay.Top = emulatorWindow.WindowRect.Bottom - 120 - 4;
            //slaveOverlay.Left = emulatorWindow.Rect.Left+4;
            slaveOverlay.Left = emulatorWindow.WindowRect.Right - 360 - 4;
            slaveOverlay.Owner = masterOverlay;
            slaveOverlay.Show();

        }

        private void MakeWindowClickThrough(Window overlay)
        {
            WindowInteropHelper helper = new WindowInteropHelper(overlay);
            int extendedStyle = User32.GetWindowLong(helper.Handle, (int)WindowLongParam.GWL_EXSTYLE);
            User32.SetWindowLong(helper.Handle, (int)WindowLongParam.GWL_EXSTYLE, extendedStyle | (int)ExtendedWindowStyles.WS_EX_LAYERED | (int)ExtendedWindowStyles.WS_EX_TRANSPARENT);
        }



    }
}
