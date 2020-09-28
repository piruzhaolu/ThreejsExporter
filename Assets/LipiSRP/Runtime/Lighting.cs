using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    static int _dirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
        _dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
    
    private const string _bufferName = "Lighting";
    
    CommandBuffer _buffer = new CommandBuffer
    {
        name = _bufferName
    };


    private CullingResults _cullingResults;
    

    //执行光源信息写入
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        _cullingResults = cullingResults;
        _buffer.BeginSample(_bufferName);
        SetupLight();
        _buffer.EndSample(_bufferName);
        context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();

    }
    
    void SetupLight()
    {
        NativeArray<VisibleLight> visibleLights = _cullingResults.visibleLights; // 从cullingResults中得到可见光
        if (visibleLights.Length > 0)
        {
            var light = visibleLights[0].light;
            _buffer.SetGlobalVector(_dirLightColorId, light.color.linear * light.intensity); // 设置CommandBuffer的全局颜色变量
            _buffer.SetGlobalVector(_dirLightDirectionId, -light.transform.forward);
        }
        
    }

    void SetupDirectionlLight()
    {
        Light light = RenderSettings.sun;
        _buffer.SetGlobalVector(_dirLightColorId, light.color.linear * light.intensity); // 设置CommandBuffer的全局颜色变量
        _buffer.SetGlobalVector(_dirLightDirectionId, -light.transform.forward);
    }

}
