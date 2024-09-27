using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorRenderer : MonoBehaviour
{

    [SerializeField] private Mesh floorMesh;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private int numInstances = 10;
    private RenderParams renderParams;

    void Start()
    {
        renderParams = new RenderParams(floorMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4[] instData = new Matrix4x4[numInstances];
        for(int i=0; i<numInstances; ++i)
            instData[i] = Matrix4x4.Translate(new Vector3(-4.5f+i, 0.0f, 5.0f));
        Graphics.RenderMeshInstanced(renderParams, floorMesh, 0, instData);
    }
}
