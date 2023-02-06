using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField, Range(0, 200)] private int resolution;
    [SerializeField, Range(0, 200)] private int step;
 
    private ComputeBuffer positionsBuffer;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");


    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }
    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }


    private void Update()
    {
        UpdateFunctionOnGPU();

		material.SetBuffer(positionsId, positionsBuffer);
		material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }


    private void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        computeShader.SetBuffer(0, positionsId, positionsBuffer);


        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);
    }
}