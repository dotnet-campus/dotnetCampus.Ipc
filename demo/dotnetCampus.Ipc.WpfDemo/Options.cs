using DotNetCampus.Cli.Compiler;

namespace dotnetCampus.Ipc.WpfDemo;

public class Options
{
    /// <summary>
    /// 本机的服务名
    /// </summary>
    [Option]
    public string? ServerName { get; set; }

    /// <summary>
    /// 对方的服务名
    /// </summary>
    [Option]
    public string? PeerName { get; set; }
}
