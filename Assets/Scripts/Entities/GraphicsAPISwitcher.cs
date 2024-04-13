#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class GraphicsAPISwitcher
{
    static GraphicsAPISwitcher()
    {
        // Switching to Vulkan (example)
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
    }
}
#endif
