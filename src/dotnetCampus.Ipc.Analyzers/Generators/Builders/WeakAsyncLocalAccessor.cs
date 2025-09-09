namespace dotnetCampus.Ipc.Generators.Builders;

/// <summary>
/// 以弱引用的方式辅助保存 <see cref="AsyncLocal{T}"/> 的值，使其可以在异步上下文中安全地访问、修改和清除。
/// </summary>
/// <typeparam name="T">要保存的值的类型。</typeparam>
internal class WeakAsyncLocalAccessor<T> where T : class
{
    private readonly AsyncLocal<AsyncLocalHolder> _current = new();

    /// <summary>
    /// 获取或设置当前异步上下文中的值。设置为 <see langword="null"/> 时，会清除自设置值时起的所有异步上下文中的值。
    /// </summary>
    public T? Value
    {
        get => _current.Value?.Reference?.TryGetTarget(out var target) is true ? target : null;
        set
        {
            var holder = _current.Value;

            if (holder is not null)
            {
                // 确保 IServiceProvider 在所有 ExecutionContext 中被清除。
                holder.Reference = null;
            }

            if (value is not null)
            {
                // 直接修改 AsyncLocal 的值，以引发 ExecutionContext 的 COW（Copy-On-Write）。
                // 新的 IServiceProvider 不会被传递到原先的 ExecutionContext 中。
                _current.Value = new AsyncLocalHolder { Reference = new WeakReference<T>(value) };
            }
        }
    }

    /// <summary>
    /// 辅助保存 <see cref="AsyncLocal{T}"/> 的值，以允许我们在不被 <see cref="ExecutionContext"/> COW（Copy-On-Write）影响的情况下，修改或清除值。
    /// </summary>
    private sealed class AsyncLocalHolder
    {
        /// <summary>
        /// 保存对值的弱引用。
        /// </summary>
        public WeakReference<T>? Reference { get; set; }
    }
}
