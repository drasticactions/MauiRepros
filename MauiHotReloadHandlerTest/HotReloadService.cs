#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(MauiHotReloadHandlerTest.HotReloadService))]
namespace MauiHotReloadHandlerTest { 
    public static class HotReloadService
    {
        internal static void ClearCache(Type[]? types) { }
        internal static void UpdateApplication(Type[]? types) {
            Microsoft.Maui.HotReload.MauiHotReloadHelper.UpdateApplication(types ?? new Type[0]);
        }
    }
}
#endif