using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCube : MonoBehaviour
{
    
    private void Update()
    {
        transform.position = Vector3.up * Mathf.Sin(Time.time) * 2f;
        transform.Rotate(Vector3.one * 90 * Time.deltaTime);
    }

}