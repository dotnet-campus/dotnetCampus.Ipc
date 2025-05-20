using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace dotnetCampus.Ipc.Utils.IO;

internal static class PipeHelper
{
    public static bool IsPipeExists(string pipeName)
    {
        bool isWindows;
#if NETFRAMEWORK
        // 对于 .NET Framework 来说，那就一定是 Windows 系统了
        isWindows = true;
#elif NETCOREAPP3_1_OR_GREATER
            // 对于 .NET Core 3.1 和 .NET 5 来说，Windows 和 Linux 都可以
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        if (isWindows)
        {
            var isPipeExists = IsPipeExistsForWindows(pipeName);
            if (!isPipeExists)
            {
                // 如果连接的时候不存在，则返回失败
                return false;
            }
        }

        return true;
    }

#if NET6_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("Windows")]
#endif
    private static bool IsPipeExistsForWindows(string pipeName)
    {
        try
        {
            // 不要用 File.Exists 判断，内部会调用 GetFileAttributes 导致管道无法被连接

            unsafe
            {
                // 这里是一个结构体，但是不关心内容，直接栈上分配点空间给它
                var findFileData = stackalloc byte[604];

                var file = FindFirstFile(@"\\.\pipe\" + pipeName, (IntPtr) findFileData);

                const nint INVALID_HANDLE_VALUE = -1;

                if (file.DangerousGetHandle() != INVALID_HANDLE_VALUE)
                {
                    FindClose(file.DangerousGetHandle());
                    return true;
                }
            }
        }
        catch
        {

        }
        return false;
    }


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileW", ExactSpelling = true)]
    private static extern SafeFileHandle FindFirstFile([In] string lpFileName, [In] IntPtr lpFindFileData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindClose", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FindClose([In] IntPtr hFindFile);
}
