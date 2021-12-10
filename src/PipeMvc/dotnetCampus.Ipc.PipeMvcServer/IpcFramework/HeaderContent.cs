using System.Collections.Generic;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    public class HeaderContent
    {
        public string Key { set; get; }
        public List<string> Value { set; get; }
    }
}