namespace IpcUno.Utils
{
    static class ScrollViewerExtensions
    {
        public static void ScrollToBottom(this TextBox textBox)
        {
            ScrollToBottomInner(textBox);
        }

        public static void ScrollToBottom(this ListView listView)
        {
            ScrollToBottomInner(listView);
        }

        private static void ScrollToBottomInner(UIElement element)
        {
            if (element.VisualDescendant<ScrollViewer>() is { } scrollViewer)
            {
                scrollViewer.ChangeView(0.0f, scrollViewer.ExtentHeight, 1.0f, true);
            }
        }
    }
}
