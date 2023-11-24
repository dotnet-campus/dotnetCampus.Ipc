using System.Diagnostics;

namespace IpcUno.Utils
{
    static class UISpyHelper
    {
        public static void Spy(this DependencyObject element)
        {
            Uno.Extensions.IndentedStringBuilder builder = new ();
            SpyInner(element, builder);
            var spyText = builder.ToString();
            Debug.WriteLine(spyText);
        }

        private static void SpyInner(DependencyObject element, Uno.Extensions.IndentedStringBuilder builder)
        {
            var name = string.Empty;
            if (element is FrameworkElement frameworkElement)
            {
                name = frameworkElement.Name;
            }
            builder.AppendLine($"{name}({element.GetType().FullName})\r\n");

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                using var t = builder.Indent();
                var child = VisualTreeHelper.GetChild(element, i);
                SpyInner(child, builder);
            }
        }
    }
}
