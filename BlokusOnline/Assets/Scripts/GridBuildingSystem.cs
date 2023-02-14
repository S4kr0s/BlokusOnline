using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private List<BuildingObject> buildingObjectList;
    private BuildingObject buildingObject;
    private Grid<GridObject> grid;
    private BuildingObject.Dir dir = BuildingObject.Dir.Down;
    private bool firstPlaced = false;
    [SerializeField] private PlayerTeam placingTeam = PlayerTeam.NONE;
    [SerializeField] private List<Material> materials;
    [SerializeField] int gridWidth = 20;
    [SerializeField] int gridHeight = 20;
    [SerializeField] bool isSingleplayer = true;

    private void Awake()
    {
        Instance = this;
        float cellSize = 10f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        buildingObject = buildingObjectList[0];
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return (placedObject == null);
        }

        public bool IsSameTeam(PlayerTeam placingTeam)
        {
            if (placedObject == null)
            {
                return true;
            }
            else
            {
                return (placedObject.PlacedBy == placingTeam);
            }
        }

        public bool PlacedObjectExists
        {
            get { return (placedObject != null); }
        }

        public void SetMaterial(Material material)
        {
            placedObject.SetMaterial(material);
        }

        public override string ToString()
        {
            return x + ", " + z + "\n" + placedObject;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);

            List<Vector2Int> gridPositionList = buildingObject.GetGridPositionList(new Vector2Int(x, z), dir);

            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (grid.GetGridObject(gridPosition.x, gridPosition.y) != null)
                {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                    else if (!HasNoNeighbors(gridPosition))
                    {
                        canBuild = false;
                        break;
                    }
                }
                else
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild == true)
            {
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (HasAngledNeighbors(gridPosition))
                    {
                        canBuild = true;
                        break;
                    }
                    canBuild = false;
                }
            }

            GridObject gridObject = grid.GetGridObject(x, z);

            if (canBuild)
            {
                Vector2Int rotationOffset = buildingObject.GetRotationOffset(dir);
                Vector3 builtObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                PlacedObject placedObject = PlacedObject.Create(builtObjectWorldPosition, new Vector2Int(x, z), dir, buildingObject, placingTeam);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetMaterial(materials[(int)placingTeam]);
                }

                if(firstPlaced == false) firstPlaced = true;
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                if (isSingleplayer)
                {
                    placingTeam = GetNextPlayerTeam();
                }
            }
            else
            {
                Debug.Log("Failed!");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            dir = BuildingObject.GetNextDir(dir);
        }

        /*
        if (Input.GetKeyDown(KeyCode.Q)) { buildingObject = buildingObjectList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.W)) { buildingObject = buildingObjectList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.E)) { buildingObject = buildingObjectList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.R)) { buildingObject = buildingObjectList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.T)) { buildingObject = buildingObjectList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Z)) { buildingObject = buildingObjectList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.U)) { buildingObject = buildingObjectList[6]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.I)) { buildingObject = buildingObjectList[7]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.O)) { buildingObject = buildingObjectList[8]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.P)) { buildingObject = buildingObjectList[9]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.A)) { buildingObject = buildingObjectList[10]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.S)) { buildingObject = buildingObjectList[11]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.D)) { buildingObject = buildingObjectList[12]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.F)) { buildingObject = buildingObjectList[13]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.G)) { buildingObject = buildingObjectList[14]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.H)) { buildingObject = buildingObjectList[15]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.J)) { buildingObject = buildingObjectList[16]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.K)) { buildingObject = buildingObjectList[17]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.L)) { buildingObject = buildingObjectList[18]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Y)) { buildingObject = buildingObjectList[19]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.X)) { buildingObject = buildingObjectList[20]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        */
    }

    private void DeselectObjectType()
    {
        buildingObject = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        if (mousePosition == Vector3.zero)
            return new Vector3(-1, -1, -1);

        grid.GetXZ(mousePosition, out int x, out int z);

        if (buildingObject != null)
        {
            Vector2Int rotationOffset = buildingObject.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (buildingObject != null)
        {
            return Quaternion.Euler(0, buildingObject.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;

        }
    }

    public BuildingObject GetBuildingObject()
    {
        return buildingObject;
    }

    public bool HasNoNeighbors(Vector2Int position)
    {
        bool hasNoNeighbors = true;

        List<GridObject> gridObjects = new List<GridObject>();
        gridObjects.Add(grid.GetGridObject(position.x, position.y + 1));
        gridObjects.Add(grid.GetGridObject(position.x + 1, position.y));
        gridObjects.Add(grid.GetGridObject(position.x, position.y - 1));
        gridObjects.Add(grid.GetGridObject(position.x - 1, position.y));

        foreach (GridObject gridObject in gridObjects)
        {
            if (gridObject != null)
            {
                if (gridObject.PlacedObjectExists)
                {
                    if (gridObject.IsSameTeam(placingTeam))
                    {
                        hasNoNeighbors = false;
                    }
                }
            }
        }

        return hasNoNeighbors;
    }

    public bool HasAngledNeighbors(Vector2Int position)
    {

        int checkX = 0, checkY = 0;
        switch (placingTeam)
        {
            case PlayerTeam.BLUE:
                checkX = 0;
                checkY = 0;
                break;
            case PlayerTeam.YELLOW:
                checkX = 0;
                checkY = gridHeight - 1;
                break;
            case PlayerTeam.RED:
                checkX = gridWidth - 1;
                checkY = gridHeight - 1;
                break;
            case PlayerTeam.GREEN:
                checkX = gridWidth - 1;
                checkY = 0;
                break;
            case PlayerTeam.NONE:
            default:
                break;
        }

        GridObject _gridObject = grid.GetGridObject(checkX, checkY);

        if (_gridObject != null && !_gridObject.PlacedObjectExists) 
        {
            firstPlaced = false;
        }

        if (!firstPlaced)
        {
            Debug.Log(placingTeam);
            if (position.x == 0 && position.y == 0 && placingTeam == PlayerTeam.BLUE)
                return true;
            if (position.x == gridWidth - 1 && position.y == 0 && placingTeam == PlayerTeam.GREEN)
                return true;
            if (position.x == 0 && position.y == gridHeight - 1 && placingTeam == PlayerTeam.YELLOW)
                return true;
            if (position.x == gridWidth - 1 && position.y == gridHeight - 1 && placingTeam == PlayerTeam.RED)
                return true;
            if (placingTeam == PlayerTeam.NONE)
                return true;
            return false;
        }
        else
        {
            bool hasAngledNeighbors = false;

            List<GridObject> gridObjects = new List<GridObject>();
            gridObjects.Add(grid.GetGridObject(position.x + 1, position.y + 1));
            gridObjects.Add(grid.GetGridObject(position.x + 1, position.y - 1));
            gridObjects.Add(grid.GetGridObject(position.x - 1, position.y - 1));
            gridObjects.Add(grid.GetGridObject(position.x - 1, position.y + 1));

            foreach (GridObject gridObject in gridObjects)
            {
                if (gridObject != null)
                {
                    if (gridObject.PlacedObjectExists && gridObject.IsSameTeam(placingTeam))
                    {
                        hasAngledNeighbors = true;
                        break;
                    }
                }
            }
            return hasAngledNeighbors;
        }
    }

    public void ChangeSelectedBuilding(int index)
    {
        buildingObject = buildingObjectList[index];
        dir = BuildingObject.Dir.Down;
        RefreshSelectedObjectType();
    }

    public PlayerTeam GetNextPlayerTeam()
    {
        switch (placingTeam)
        {
            case PlayerTeam.BLUE:
                return PlayerTeam.YELLOW;
            case PlayerTeam.YELLOW:
                return PlayerTeam.RED;
            case PlayerTeam.RED:
                return PlayerTeam.GREEN;
            case PlayerTeam.GREEN:
            default:
                return PlayerTeam.BLUE;
        }
    }
}