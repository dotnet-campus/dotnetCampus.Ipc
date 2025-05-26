using System.ComponentModel;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 从服务器返回的值
    /// </summary>
    /// Copy From: https://github.com/jacqueskang/IpcServiceFramework.git
    [Obsolete("此类型不再使用，等待下次大版本更新一起删掉")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    // todo 等待下次大版本更新一起删掉
    public enum IpcStatus : int
    {
        Unknown = 0,
        Ok = 200,
        BadRequest = 400,
        InternalServerError = 500,
    }
}
