using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private Transform visual;
    private BuildingObject buildingObject;

    private void Start()
    {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        if(targetPosition == new Vector3(-1, -1, -1))
        {
            transform.position = new Vector3(0, 1000, 0);
        }
        else if (transform.position == new Vector3(0, 1000, 0))
        {
            targetPosition.y = 1f;
            transform.position = targetPosition;

            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }
        else
        {
            targetPosition.y = 1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }

    }

    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        BuildingObject buildingObject = GridBuildingSystem.Instance.GetBuildingObject();

        if (buildingObject != null)
        {
            visual = Instantiate(buildingObject.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}
