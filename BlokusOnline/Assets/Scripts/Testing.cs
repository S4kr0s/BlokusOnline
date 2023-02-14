using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid<int> grid;

    private void Start()
    {
        //grid = new Grid<int>(20, 20, 10f, Vector3.zero, int);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(GetMouseWorldPosition());
            grid.SetValue(GetMouseWorldPosition(), 999);
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log(grid.GetValue(GetMouseWorldPosition()));
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
        {
            return raycastHit.point;
        } 
        else
        {
            return Vector3.zero;
        }

        // for 2d
        // Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // vector.z = 0f;
        // return vector;
    }
}
