using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Launcher
{
    // https://stackoverflow.com/questions/61567576/how-to-make-a-global-keyboard-hook-in-c-sharp
    // I'm an evil human being :(
    // Credit to https://stackoverflow.com/users/11318498/hanabanashiku
    // For most o
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

        //private static readonly int VK_SNAPSHOT = 0x2C; //This is the print-screen key.
     

        //short keyState = GetAsyncKeyState(VK_SNAPSHOT);

        /*
            //Check if the MSB is set. If so, then the key is pressed.
            bool prntScrnIsPressed = ((keyState >> 15) & 0x0001) == 0x0001;

            //Check if the LSB is set. If so, then the key was pressed since
            //the last call to GetAsyncKeyState
            bool unprocessedPress = ((keyState >> 0) & 0x0001) == 0x0001;
        */
    }
    
}
