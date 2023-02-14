using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class BuildingObject : ScriptableObject
{
    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down:  return Dir.Left;
            case Dir.Left:  return Dir.Up;
            case Dir.Up:    return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right
    }

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public List<int> ToPlace;
    public List<int> ToRemoveX;
    public List<int> ToRemoveY;

    public int GetRotationAngle(Dir dir)
    {
        int maxValue = ToPlace.Max();

        if (maxValue == 1 && ToPlace.Count == 1)
            return 0;

        switch (dir)
        {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        int maxValue = ToPlace.Max();

        if (maxValue == 1 && ToPlace.Count == 1)
            return new Vector2Int(0, 0);

        switch (dir)
        {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Up:    return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
            case Dir.Left:  return new Vector2Int(0, 1);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
                for (int x = 0; x < ToPlace.Count; x++)
                {
                    for (int i = 0; i < ToPlace[x]; i++)
                    {
                        Debug.Log(offset + new Vector2Int(x, i));
                        gridPositionList.Add(offset + new Vector2Int(x, i));
                    }
                }
                for (int x = 0; x < ToRemoveX.Count; x++)
                {
                    Debug.Log("Removed: " + (offset + new Vector2Int(ToRemoveX[x], ToRemoveY[x])));
                    gridPositionList.Remove(offset + new Vector2Int(ToRemoveX[x], ToRemoveY[x]));
                }
                break;
            case Dir.Up:
                for (int x = 0; x < ToPlace.Count; x++)
                {
                    for (int i = 0; i < ToPlace[x]; i++)
                    {
                        Debug.Log(offset + new Vector2Int(-x, -i));
                        gridPositionList.Add(offset + new Vector2Int(-x, -i));
                    }
                }
                for (int x = 0; x < ToRemoveX.Count; x++)
                {
                    Debug.Log("Removed: " + (offset + new Vector2Int(-ToRemoveX[x], -ToRemoveY[x])));
                    gridPositionList.Remove(offset + new Vector2Int(-ToRemoveX[x], -ToRemoveY[x]));
                }
                break;
            case Dir.Left:
                for (int x = 0; x < ToPlace.Count; x++)
                {
                    for (int i = 0; i < ToPlace[x]; i++)
                    {
                        Debug.Log(offset + new Vector2Int(i, -x));
                        gridPositionList.Add(offset + new Vector2Int(i, -x));
                    }
                }
                for (int x = 0; x < ToRemoveX.Count; x++)
                {
                    Debug.Log("Removed: " + (offset + new Vector2Int(ToRemoveY[x], -ToRemoveX[x])));
                    gridPositionList.Remove(offset + new Vector2Int(ToRemoveY[x], -ToRemoveX[x]));
                }
                break;
            case Dir.Right:
                for (int x = 0; x < ToPlace.Count; x++)
                {
                    for (int i = 0; i < ToPlace[x]; i++)
                    {
                        Debug.Log(offset + new Vector2Int(-i, x));
                        gridPositionList.Add(offset + new Vector2Int(-i, x));
                    }
                }
                for (int x = 0; x < ToRemoveX.Count; x++)
                {
                    Debug.Log("Removed: " + (offset + new Vector2Int(-ToRemoveY[x], ToRemoveX[x])));
                    gridPositionList.Remove(offset + new Vector2Int(-ToRemoveY[x], ToRemoveX[x]));
                }
                break;
        }
        return gridPositionList;
    }
}
