using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Launcher
{

    public class Input
    {

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

		[DllImport("User32.dll")]
        private static extern short GetKeyState(int vKey);


		public static bool IsKeydown(int vKey)
		{
			return ((ushort)GetKeyState(vKey) >> 15) == 1;
        }
    }
    
}
