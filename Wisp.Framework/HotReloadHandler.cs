#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(Wisp.Framework.HotReloadHandler))]

namespace Wisp.Framework;

public static class HotReloadHandler
{
    public static event Action<Type[]?>? UpdateApplicationEvent;
    
    public static void UpdateApplication(Type[]? updatedTypes)
    {
        UpdateApplicationEvent?.Invoke(updatedTypes);
    }
}
#endif