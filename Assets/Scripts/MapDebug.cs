using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebug : MonoBehaviour
{

    private Transform checkpoint;

    private void Start()
    {
        checkpoint = transform.Find("Checkpoint");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(checkpoint.position, checkpoint.right, Color.blue);
    }
}
