#nullable disable // 序列化的代码，不需要可空
using System.Collections.Generic;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HeaderContent
    {
        public string Key { set; get; }
        public List<string> Value { set; get; }
    }
}
