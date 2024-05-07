using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Launcher
{
    public class RemoteFunction
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

        public static void ExecuteFunction(int processHandle, long funcPointer)
        {
            IntPtr addr = IntPtr.Zero;
            IntPtr hThread = IntPtr.Zero;
            UIntPtr bytesWritten = UIntPtr.Zero;
            /*  48 B8 9D 13 D4 81 F7 7F 00 00  mov rax, 0x7FF781D4139D ; move the address 0x7FF781D4139D to rax
                FF D0                          call rax                ; call the value stored in rax registry
                C3                             retn                    ; returns, without this opcode the process will crash
            */
            byte[] opcode = new byte[13];
            opcode[0] = 0x48;
            opcode[1] = 0xB8;
            byte[] pointerToByte = BitConverter.GetBytes(funcPointer);
            for (int i = 0 ; i < pointerToByte.Length;i++) 
            {
                opcode[2 + i] = pointerToByte[i];
            }
            opcode[10] = 0xFF;
            opcode[11] = 0xD0;
            opcode[12] = 0xC3;
            /*{
                 0x48, 0xB8, 0x9D, 0x13, 0xD4, 0x81, 0xF7, 0x7F, 0x00, 0x00,
                 0xFF, 0xD0,
                 0xC3
            };*/

            // allocate memory
            if ((addr = VirtualAllocEx(processHandle, IntPtr.Zero, (uint)opcode.Length, 0x00001000, 0x0040)) == IntPtr.Zero)
            {
                Console.WriteLine("failed to allocate memory.");
                return;
            }
            // write memory
            if (!WriteProcessMemory(processHandle, addr, opcode, (uint)opcode.Length, out bytesWritten))
            {
                Console.WriteLine("could not write to process' memory.");
                return;
            }
            uint lpThreadId = 0;
            // create thread
            if ((hThread = CreateRemoteThread(processHandle, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, out lpThreadId)) == IntPtr.Zero)
            {
                Console.WriteLine("hThread value is 0x0.");
                return;
            }
        }

    }
}
