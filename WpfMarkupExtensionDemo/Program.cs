using System;

namespace WpfMarkupExtensionDemo;

public class Program
{
    [STAThread]
    public static void Main()
    {
        App app = new();
        app.Run(MainWindow.Create());
    }
}