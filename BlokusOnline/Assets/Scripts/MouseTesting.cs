using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTesting : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
        {
            transform.position = raycastHit.point;
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
}
