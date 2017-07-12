using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ScaleRay : MonoBehaviour {

    Vector3 scale { set { } get { return transform.localScale; } }
    Material material;
    int _scale;
	// Use this for initialization
	void Start () {
        _scale = Shader.PropertyToID("_Scale");

        material = GetComponent<Renderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateScale();
	}
    void UpdateScale()
    {
        material.SetVector(_scale, scale);
    }
}
