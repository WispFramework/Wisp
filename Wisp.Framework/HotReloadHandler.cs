#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(Wisp.Framework.HotReloadHandler))]

namespace Wisp.Framework;

public static class HotReloadHandler
{
    public static event Action<Type[]?>? UpdateApplicationEvent;
    public static event Action<Type[]?>? PostUpdateApplicationEvent;
    
    public static void UpdateApplication(Type[]? updatedTypes)
    {
        UpdateApplicationEvent?.Invoke(updatedTypes);
        PostUpdateApplicationEvent?.Invoke(updatedTypes);
    }
}
#endif