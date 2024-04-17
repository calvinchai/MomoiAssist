using System.Runtime.InteropServices;

namespace MomoiAssist.Helpers.PInvoke;

public static class Gdi32
{
    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, nint hdcSrc, int nXSrc, int nYSrc, uint dwRop);

}