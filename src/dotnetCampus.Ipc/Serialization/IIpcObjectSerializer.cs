namespace dotnetCampus.Ipc.Serialization
{
    /// <summary>
    /// 对象序列化器
    /// </summary>
    public interface IIpcObjectSerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Serialize(object value);

        void Serialize(Stream stream, object? value);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] data);

        T? Deserialize<T>(Stream stream);
    }
}
