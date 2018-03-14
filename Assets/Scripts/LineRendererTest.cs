using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererTest : MonoBehaviour
{
    public Vector3 centerPosition;

	private void Start()
    {
        //var renderer = gameObject.AddComponent<LineRenderer>();
        //var startPosition = centerPosition + Vector3.forward;
        //renderer.material = Resources.Load("Materials/HexFieldMaterial") as Material;
        //renderer.widthMultiplier = 0.1f;

        //renderer.positionCount = 5;

        //renderer.SetPosition(0, new Vector3(-0.5f, 0, 0.5f) + centerPosition);
        //renderer.SetPosition(1, new Vector3(-0.5f, 0, -0.5f) + centerPosition);
        //renderer.SetPosition(2, new Vector3(0.5f, 0, -0.5f) + centerPosition);
        //renderer.SetPosition(3, new Vector3(0.5f, 0, 0.5f) + centerPosition);
        //renderer.SetPosition(4, new Vector3(-0.5f, 0, 0.5f) + centerPosition);

    }

    private void OnMouseEnter()
    {
        Debug.Log("Great success!");
    }

    private void OnMouseExit()
    {
        Debug.Log("Fine!");
    }
}
