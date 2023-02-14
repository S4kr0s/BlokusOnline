using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, BuildingObject.Dir dir, BuildingObject buildingObject, PlayerTeam placedBy)
    {
        Transform placedObjectTransform = Instantiate(buildingObject.prefab, worldPosition, Quaternion.Euler(0, buildingObject.GetRotationAngle(dir), 0));
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.buildingObject = buildingObject;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.placedBy = placedBy;
        return placedObject;
    }

    public void SetMaterial(Material material)
    {
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material;
        }
    }

    private BuildingObject buildingObject;
    private Vector2Int origin;
    private BuildingObject.Dir dir;
    private PlayerTeam placedBy;
    public PlayerTeam PlacedBy { get { return placedBy; } }

    public List<Vector2Int> GetGridPositionList()
    {
        return buildingObject.GetGridPositionList(origin, dir);
    }
}

public enum PlayerTeam
{
    BLUE = 1,
    YELLOW = 2,
    RED = 3,
    GREEN = 4,
    NONE = 0
}
