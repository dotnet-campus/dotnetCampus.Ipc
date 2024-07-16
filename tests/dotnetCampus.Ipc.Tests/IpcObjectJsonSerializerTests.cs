using dotnetCampus.Ipc.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class IpcObjectJsonSerializerTests
    {
        [TestMethod("序列化对象之后，能通过二进制反序列化回对象")]
        public void Serialize()
        {
            // Arrange
            IIpcObjectSerializer ipcObjectSerializer = new IpcObjectJsonSerializer();
            var foo = new Foo() { Name = "林德熙是逗比" };

            // Action
            var byteList = ipcObjectSerializer.Serialize(foo);
            var deserializeFoo = ipcObjectSerializer.Deserialize<Foo>(byteList);

            // Assert
            Assert.AreEqual(foo.Name, deserializeFoo.Name);
        }

        public class Foo
        {
            public string Name { set; get; } = "";
        }
    }
}
