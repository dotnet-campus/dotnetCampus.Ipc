namespace IpcUno.Utils
{
    static class TreeExtensions
    {
        public static T? VisualDescendant<T>(this UIElement element) where T : DependencyObject
            => VisualDescendant<T>((DependencyObject) element);

        public static T? VisualDescendant<T>(DependencyObject element) where T : DependencyObject
        {
            if (element is T)
            {
                return (T) element;
            }

            T? foundElement = default;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                foundElement = VisualDescendant<T>(child);
                if (foundElement != null)
                {
                    break;
                }
            }

            return foundElement;
        }
    }
}
