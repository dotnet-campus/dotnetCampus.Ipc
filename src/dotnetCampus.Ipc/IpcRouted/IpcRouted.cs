using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc
{

    class IpcRequestHandler : IIpcRequestHandler
    {
        public Task<IIpcResponseMessage> HandleRequest(IIpcRequestContext requestContext)
        {
            throw null;
        }
    }

    public class IpcRoutedAttribute : Attribute
    {
        public IpcRoutedAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    public interface IIpcRoutedHandler
    {

    }

    public class IpcRoutedRequestContext
    {


        public IPeerProxy Peer { get; }

        public IpcMessage IpcMessage { get; }

        
    }

    public class IpcRoutedFramework
    {
        public IpcRoutedFramework(IEnumerable<(IpcRoutedAttribute attribute, Func<IIpcRoutedHandler> creator)> collection)
        {

        }

        public IIpcRequestHandler BuildIpcRequestHandler()
        {
            throw null;
        }
    }
}
