using System;

namespace Net.Leksi.WpfMarkup;
public static class NotifyInstanceCreated
{
    public static EventHandler? InstanceCreated;
    internal static EventArgs s_instanceCreatedArgs = new();
}
