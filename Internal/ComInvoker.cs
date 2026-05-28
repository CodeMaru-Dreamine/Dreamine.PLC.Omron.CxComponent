using System.Globalization;
using System.Reflection;

namespace Dreamine.PLC.Omron.CxComponent.Internal;

internal static class ComInvoker
{
    public static void SetProperty(object target, string name, object? value)
    {
        target.GetType().InvokeMember(
            name,
            BindingFlags.SetProperty,
            binder: null,
            target,
            [value],
            CultureInfo.InvariantCulture);
    }

    public static object? Invoke(object target, string name, params object?[] args)
    {
        return target.GetType().InvokeMember(
            name,
            BindingFlags.InvokeMethod,
            binder: null,
            target,
            args,
            CultureInfo.InvariantCulture);
    }
}
