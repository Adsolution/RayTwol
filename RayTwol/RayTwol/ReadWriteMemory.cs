﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Timers;

namespace RayTwol
{
    static class Memory
    {
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        public static string runButtonText = "RUN";
        public static Process process;
        public static bool isSynced;
        public static Timer checkRunningTimer = new Timer();

        public static Vec3 rayPos;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);
        
        

        public static Vec3 GetRaymanPosition()
        {
            return new Vec3(
                BitConverter.ToSingle(ReadProcessMemoryPointer(Memory.process, 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -8), 0),
                BitConverter.ToSingle(ReadProcessMemoryPointer(Memory.process, 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -4), 0),
                BitConverter.ToSingle(ReadProcessMemoryPointer(Memory.process, 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -0), 0));
        }

        public static void SetRaymanPosition(Vec3 pos)
        {
            WriteProcessMemoryPointer(Memory.process, BitConverter.GetBytes(pos.x), 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -8);
            WriteProcessMemoryPointer(Memory.process, BitConverter.GetBytes(pos.y), 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -4);
            WriteProcessMemoryPointer(Memory.process, BitConverter.GetBytes(pos.z), 0x00500560, new int[] { 0x29C, 0x360, 0x4, 0x3C, 0x154 }, -0);
        }


        
        public static byte[] ReadProcessMemoryPointer(Process process, int baseAddress, int[] offsets, int finalOffset = 0)
        {
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            ReadProcessMemory((int)processHandle, baseAddress, buffer, 4, ref bytesRead);
            int temp = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                ReadProcessMemory((int)processHandle, temp + offsets[i], buffer, 4, ref bytesRead);
                temp = BitConverter.ToInt32(buffer, 0);
            }
            ReadProcessMemory((int)processHandle, temp + offsets[offsets.Length - 1] + finalOffset, buffer, 4, ref bytesRead);
            return buffer;
        }
        

        public static void WriteProcessMemoryPointer(Process process, byte[] value, int baseAddress, int[] offsets, int finalOffset = 0)
        {
            IntPtr processHandle = OpenProcess(0x1F0FFF, false, process.Id);
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            ReadProcessMemory((int)processHandle, baseAddress, buffer, 4, ref bytesRead);
            int temp = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                ReadProcessMemory((int)processHandle, temp + offsets[i], buffer, 4, ref bytesRead);
                temp = BitConverter.ToInt32(buffer, 0);
            }
            WriteProcessMemory((int)processHandle, temp + offsets[offsets.Length - 1] + finalOffset, value, 4, ref bytesRead);
        }
    }
}
