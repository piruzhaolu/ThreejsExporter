using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Rendering/Li Pipeline")]
public class LiPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new LiPipeline();
    }
}


public class LiPipeline : RenderPipeline
{
    
    static ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
        _litShaderTagId = new ShaderTagId("CustomLit"); 
    
    public LiPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            Render(context, camera);
        }
    }
    
    Lighting lighting = new Lighting();
    private void Render(ScriptableRenderContext context, Camera camera)
    {
        context.SetupCameraProperties(camera);

        
        
        var buffer = new CommandBuffer
        {
            name = camera.name
        };
        CameraClearFlags clearFlags = camera.clearFlags;
        
        buffer.BeginSample("Render Camera");
        
        //
        buffer.ClearRenderTarget(
            (clearFlags & CameraClearFlags.Depth) != 0,
            (clearFlags & CameraClearFlags.Color) != 0,
            camera.backgroundColor);
        //uffer.ClearRenderTarget(true,true,Color.clear);
        

        


        if (!camera.TryGetCullingParameters(out var cullingParameters))
        {
            return;
        }
        
        buffer.BeginSample("RenderLoop.Draw");
        var cullingResults = context.Cull(ref cullingParameters);
        lighting.Setup(context,cullingResults); //设置光源数据

        var drawingSettings = new DrawingSettings(_unlitShaderTagId, new SortingSettings(camera));
        
        drawingSettings.SetShaderPassName(1, _litShaderTagId);
       
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        buffer.EndSample("RenderLoop.Draw");

        context.DrawSkybox(camera);
        
        buffer.BeginSample("RenderLoop.Draw");
        filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        buffer.EndSample("RenderLoop.Draw");
        
        buffer.EndSample("Render Camera");
        context.ExecuteCommandBuffer(buffer);
        buffer.Release();
        
        context.Submit();
    }
}