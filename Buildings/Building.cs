using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using System.IO;

public class Building : MonoBehaviour {

    BuildingTypes toBuild;
    public House[] house;
    public Commercial[] commercial;
    public Road[] road;
    public TownHall townHall;
    public Hospital hospital;
    public PoliceStation police;
    public FireStation fireStation;
    public Transform buildingObjects;
    public Transform hoverSprite;

    string file = "Assets\\Resources\\StartScene.txt";

    Driving driving;
    ResourceManager resources;

    public Transform objectToBuild;
    public GameObject ui;

    public int obj = 0;

    Vector3 placePos;
    Direction rotation;
    public float height = 0f;

    public Cells size;
    public GameObject[,] grid;

    float timer;

    void Awake ()
    {
        grid = new GameObject[size.x, size.z];
        driving = transform.GetComponent<Driving>();                // the driving scipt attached to the game controller
        resources = transform.GetComponent<ResourceManager>();      // the resource scipt attached to the game controller
        Physics.IgnoreLayerCollision(9, 9);

        if (!File.Exists(file))
            File.Create(file);
        else
            File.WriteAllText(file, String.Empty);
    }

	void LateUpdate ()
    {
        if (!driving.driving)   // if not driving...
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                switch (rotation)
                {
                    case (Direction.North):
                        rotation = Direction.East;
                        break;
                    case (Direction.East):
                        rotation = Direction.South;
                        break;
                    case (Direction.South):
                        rotation = Direction.West;
                        break;
                    case (Direction.West):
                        rotation = Direction.North;
                        break;
                }
            }

            HoverSprite();

            #region Clicking stuffs
            ui = EventSystem.current.currentSelectedGameObject;
            if (ui == null)
            {
                if (toBuild)
                {
                    if (Input.GetMouseButtonUp(0) && !toBuild.CompareTag("Road"))
                    {
                        Cells cell;
                        cell.x = (placePos.x == 0) ? 0 : Mathf.Abs((int)placePos.x / 10);
                        cell.z = (placePos.z == 0) ? 0 : Mathf.Abs((int)placePos.z / 10);
                        StartCoroutine(PlaceBuilding(cell, rotation));
                    }
                    if (toBuild.CompareTag("Road") && Input.GetMouseButton(0))
                    {
                        bool update = false;
                        Cells cell;
                        cell.x = (placePos.x == 0) ? 0 : Mathf.Abs((int)placePos.x / 10);
                        cell.z = (placePos.z == 0) ? 0 : Mathf.Abs((int)placePos.z / 10);
                        PlaceRoad(update, cell);
                    }
                }

                if (Input.GetMouseButton(1))    // right click means deletion
                {
                    if (objectToBuild != null)
                        ResetObjectToBuild(null);

                    //Raycast from mouse position on screen
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // Ray going from camera to mouse position
                    int layerMask = 1 << LayerMask.NameToLayer("Buildings");           // Ray will only intersect the buildings
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        if (!hit.transform.CompareTag("Untagged"))
                            Deletion(hit.transform);
                }
            }
            #endregion
        }
        else
        {
            if (objectToBuild != null)
            {
                ResetObjectToBuild(null);
                hoverSprite.gameObject.SetActive(false);
            }
        }
        
    }

    public void ResetObjectToBuild(BuildingTypes _toBuild)
    {
        rotation = Direction.North;
        if (objectToBuild != null)
            Destroy(objectToBuild.gameObject);

        if (_toBuild == null)
        {
            hoverSprite.gameObject.SetActive(true);
            toBuild = null;
        }
        else
        {
            toBuild = _toBuild;
            objectToBuild = Instantiate(_toBuild.transform);
        }
        if (objectToBuild)
        {
            hoverSprite.gameObject.SetActive(false);
        
            File.AppendAllText(file, "building.ResetObjectToBuild(" + _toBuild + ");" + Environment.NewLine);
        }
        else
            hoverSprite.gameObject.SetActive(true);
    }

    public IEnumerator PlaceBuilding(Cells cell, Direction r)
    {
        BuildingTypes buildPrefab = toBuild;

        if (buildPrefab.cost <= resources.money || resources.creative)
        {
            int xMax = buildPrefab.xMax, zMax = buildPrefab.zMax;

            #region check map based on direction
            switch (r)
            {
                case (Direction.North):
                    for (int x = cell.x; x < cell.x + xMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + zMax; z++)
                        {
                            if (!CheckForEdge(x, z) || grid[x, z] != null)
                                yield break;
                        }
                    }
                    break;
                case (Direction.South):
                    for (int x = cell.x; x < cell.x + xMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + zMax; z++)
                        {
                            if (!CheckForEdge(x, z) || grid[x, z] != null)
                                yield break;
                        }
                    }
                    break;
                case (Direction.East):
                    for (int x = cell.x; x < cell.x + zMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + xMax; z++)
                        {
                            if (!CheckForEdge(x, z) || grid[x, z] != null)
                                yield break;
                        }
                    }
                    break;
                case (Direction.West):
                    for (int x = cell.x; x < cell.x + zMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + xMax; z++)
                        {
                            if (!CheckForEdge(x, z) || grid[x, z] != null)
                                yield break;
                        }
                    }
                    break;
            }
            #endregion

            Renderer renderer;
            renderer = buildPrefab.transform.GetChild(0).GetComponent<Renderer>();
            Bounds size = renderer.bounds;
            placePos = new Vector3(-cell.x * 10 - ((r == Direction.East || r == Direction.West ? size.extents.z : size.extents.x) - 5), 0, -cell.z * 10 - ((r == Direction.East || r == Direction.West ? size.extents.x : size.extents.z) - 5));
            
            BuildingTypes build = Instantiate(buildPrefab) as BuildingTypes;
            build.gameObject.SetActive(false);
            build.transform.position = placePos;
            build.transform.rotation = r.ToRotation();
            build.transform.parent = buildingObjects;
            build.cell = cell;
            build.dir = r;
            build.GetPlaced = true;
            if (!resources.creative)
                resources.money -= buildPrefab.cost;


            File.AppendAllText(file, "StartCoroutine(building.PlaceBuilding(new Cells(" + cell.x + ", " + cell.z +"), Direction." + r.ToString() + "));" + Environment.NewLine);

            #region fill grid with object size
            switch (r)
            {
                case (Direction.North):
                case (Direction.South):
                    for (int x = cell.x; x < cell.x + xMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + zMax; z++)
                        {
                            grid[x, z] = build.gameObject;
                        }
                    }
                    break;
                case (Direction.East):
                case (Direction.West):
                    for (int x = cell.x; x < cell.x + zMax; x++)
                    {
                        for (int z = cell.z; z < cell.z + xMax; z++)
                        {
                            grid[x, z] = build.gameObject;
                        }
                    }
                    break;
            }
            #endregion

            if (resources.creative)
                yield return null;
            else
                yield return new WaitForSeconds(build.buildTime);
            build.gameObject.SetActive(true);
        }
    }

    IEnumerator BuildTime (float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    
    //Places the correct road in the cell where you click and updates any cells around that have roads in them
    public void PlaceRoad(bool update, Cells cell, Direction calledDir = Direction.None)
    {

        if (!(road[0].cost <= resources.money || resources.creative))
            return;
        placePos.y = height;
        int otherCount = 0; //number of adjacent roads
        bool[] otherDir = new bool[4]; //other directions
        Direction dir = Direction.North; //Default direction

        //If this is an updated position, destroy the cell
        if (update)
        {
            //Destroy the cell in the grid
            Destroy(grid[cell.x, cell.z]);

            //Add called direction to the other position as it hasn't been added to the grid yet
            switch (calledDir)
            {
                case (Direction.West):
                    otherDir[0] = true;
                    dir = Direction.East;
                    otherCount++;
                    break;
                case (Direction.North):
                    otherDir[1] = true;
                    dir = Direction.South;
                    otherCount++;
                    break;
                case (Direction.East):
                    otherDir[2] = true;
                    dir = Direction.West;
                    otherCount++;
                    break;
                case (Direction.South):
                    otherDir[3] = true;
                    dir = Direction.North;
                    otherCount++;
                    break;
                default:
                    break;
            }
        }
        
        if (grid[cell.x, cell.z] == null || update == true)
        {
            if (!update)
                if (!resources.creative)
                    resources.money -= road[0].cost;

            #region Check each adjacent cell to see if it contains a road
            //East
            if (cell.x > 0)
            {
                if (grid[cell.x - 1, cell.z] != null)   // first check if there is anything to the left
                {
                    if (grid[cell.x - 1, cell.z].tag == "Road") // then check whether that gameobject is a road
                    {
                        if (update == false)    
                        {
                            //update road to the east
                            update = true;

                            //Cell to update
                            Cells updateCell = cell;
                            updateCell.x = updateCell.x - 1;

                            PlaceRoad(update, updateCell, Direction.East);
                            update = false;
                        }
                        otherCount++;
                        otherDir[0] = true;
                        dir = Direction.East;
                    }
                }
            }
            //South
            if (cell.z > 0)
            {
                if (grid[cell.x, cell.z - 1] != null)
                {
                    if (grid[cell.x, cell.z - 1].tag == "Road")
                    {
                        if (update == false)
                        {
                            //update road to the south
                            update = true;

                            //Cell to update
                            Cells updateCell = cell;
                            updateCell.z = updateCell.z - 1;

                            PlaceRoad(update, updateCell, Direction.South);
                            update = false;
                        }
                        otherCount++;
                        otherDir[1] = true;
                        dir = Direction.South;
                    }
                }
            }
            //West
            if (cell.x < size.x - 1)
            {
                if (grid[cell.x + 1, cell.z] != null)
                {
                    if (grid[cell.x + 1, cell.z].tag == "Road")
                    {
                        if (update == false)
                        {
                            //update road to the west
                            update = true;

                            //Cell to update
                            Cells updateCell = cell;
                            updateCell.x = updateCell.x + 1;

                            PlaceRoad(update, updateCell, Direction.West);
                            update = false;
                        }
                        otherCount++;
                        otherDir[2] = true;
                        dir = Direction.West;
                    }
                }
            }
            //North
            if (cell.z < size.z - 1)
            {
                if (grid[cell.x, cell.z + 1] != null)
                {
                    if (grid[cell.x, cell.z + 1].tag == "Road")
                    {
                        if (update == false)
                        {
                            //update road to the north
                            update = true;

                            //Cell to update
                            Cells updateCell = cell;
                            updateCell.z = updateCell.z + 1;

                            PlaceRoad(update, updateCell, Direction.North);
                            update = false;
                        }
                        otherCount++;
                        otherDir[3] = true;
                        dir = Direction.North;
                    }
                }
            }
            #endregion

            //Choose the right road prefab, put it in the right place and add it to the grid
            Road roadToBuild = (otherCount == 0) ? road[0]: road[otherCount - 1];

            if (otherCount == 2)
            {
                dir = CornerDir(otherDir, dir);

                //Straight roads
                //North and South
                if (otherDir[3] && otherDir[1])
                    roadToBuild = road[0];
                //East and West
                if (otherDir[0] && otherDir[2])
                    roadToBuild = road[0];
            }

            if (otherCount == 3)
                dir = TJunctionDir(otherDir, dir);

            Road roadPrefab = Instantiate(roadToBuild) as Road;

            placePos.x = cell.x == 0 ? 0 : -(cell.x * 10);
            placePos.z = cell.z == 0 ? 0 : -(cell.z * 10);
            roadPrefab.transform.position = placePos;

            roadPrefab.transform.rotation = Quaternion.Euler(-90f, dir.ToRotation().eulerAngles.y, 0);
            roadPrefab.cell = cell;
            roadPrefab.dir = dir;
            grid[cell.x, cell.z] = roadPrefab.gameObject;
            roadPrefab.transform.parent = buildingObjects;
            
            File.AppendAllText(file, "building.PlaceRoad(" + update.ToString().ToLower() + ", new Cells(" + cell.x + ", " + cell.z + "), Direction." + dir.ToString() + ");" + Environment.NewLine);
        }
    }

    //Places a small square sprite on the terrain to show the grid
    void HoverSprite ()
    {
        if (!driving.driving)   // if not driving...
        {
            //Raycast from mouse position on screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // Ray going from camera to mouse position
            int layerMask = 1 << LayerMask.NameToLayer("Ground");           // Ray will only intersect the ground layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //Position for the hover Sprite thing, the Math.Round makes it so that it has a grid, the sprite will only move if over 1 space
                placePos = new Vector3(0, height, 0);
                //Mouses position over 10, rounded to the nearest integer, then x 10 to make the cells the right size 
                placePos.x = 10 * Mathf.Round(hit.point.x / 10);
                placePos.z = 10 * Mathf.Round(hit.point.z / 10);
                //Set the sprites position
                if (objectToBuild)
                {
                    Renderer renderer;
                    if (objectToBuild.tag == "Road")
                        renderer = objectToBuild.GetComponent<Renderer>();
                    else
                    {
                        renderer = objectToBuild.GetChild(0).GetComponent<Renderer>();
                        objectToBuild.rotation = rotation.ToRotation();
                    }
                    Bounds size = renderer.bounds;
                    objectToBuild.position = new Vector3(placePos.x - (size.extents.x - 5), placePos.y, placePos.z - (size.extents.z - 5));
                }
                else
                {
                    hoverSprite.gameObject.SetActive(true);
                    hoverSprite.position = placePos;
                }
            }
        }
    }

    //Setting up the corner rotation
    Direction CornerDir(bool[] otherDir, Direction dir)
    {
        //East and South
        if (otherDir[0] && otherDir[1])
            dir = Direction.East;
        //South and West
        if (otherDir[1] && otherDir[2])
            dir = Direction.North;
        //West and North
        if (otherDir[2] && otherDir[3])
            dir = Direction.West;
        //North and East
        if (otherDir[3] && otherDir[0])
            dir = Direction.South;
        //North and South
        if (otherDir[3] && otherDir[1])
            dir = Direction.South;
        //East and West
        if (otherDir[0] && otherDir[2])
            dir = Direction.West;
        return dir;
    }

    //Setting up the corner rotation
    Direction TJunctionDir(bool[] otherDir, Direction dir)
    {
        //East, South and West
        if (otherDir[0] && otherDir[1] && otherDir[2])
            dir = Direction.North;
        //South, West and North
        if (otherDir[1] && otherDir[2] && otherDir[3])
            dir = Direction.West;
        //West, North and East
        if (otherDir[2] && otherDir[3] && otherDir[0])
            dir = Direction.South;
        //North, East and South
        if (otherDir[3] && otherDir[0] && otherDir[1])
            dir = Direction.East;
        return dir;
    }

    //Sink the fractured object into the ground
    IEnumerator Sink (GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(5f);

        foreach (Transform o in objectToDestroy.transform.GetChild(1))
        {
            o.GetComponent<MeshCollider>().enabled = false;
            o.GetComponent<Rigidbody>().AddForce(Vector3.up * .1f, ForceMode.Impulse);
        }
        Destroy(objectToDestroy, 3f);
    }

    //Destroy the cell that the user has right clicked
    public void Deletion (Transform hit)
    {
        if (hit != null )
        {
            GameObject objectDeleted = hit.gameObject;
            BuildingTypes build = null;
            Cells cell;

            build = objectDeleted.GetComponent<BuildingTypes>();

            if (!build) return;  // for some reason deletion is called multiple times on one object

            cell = build.cell;
            resources.money += build.cost / 2;
            build.placed = false;

            #region
            //If the object has a fractured model
            if (objectDeleted.transform.childCount >= 2)
            {
                int xMax = 0, zMax = 0;
                xMax = build.xMax;
                zMax = build.zMax;

                if (objectDeleted.GetComponent<TownHall>())
                {
                    Destroy(objectDeleted);

                    for (int x = cell.x; x < cell.x + xMax; x++)
                        for (int z = cell.z; z < cell.z + zMax; z++)
                            grid[x, z] = null;

                    return;
                }

                objectDeleted.transform.GetChild(0).gameObject.SetActive(false);
                for (int x = cell.x; x < cell.x + xMax; x++)
                    for (int z = cell.z; z < cell.z + zMax; z++)
                        grid[x, z] = null;
                objectDeleted.transform.GetChild(1).gameObject.SetActive(true);
                StartCoroutine("Sink", objectDeleted);
            }
            else //If it doesn't have a model it just deletes
            {
                Destroy(objectDeleted);
                //Roads have to update any adjacent roads
                if (objectDeleted.tag == "Road")
                {
                    //Check each adjacent cell to see if it contains a road
                    grid[cell.x, cell.z] = null;
                    //East
                    if (cell.x > 0)
                    {
                        if (grid[cell.x - 1, cell.z] != null)
                        {
                            if (grid[cell.x - 1, cell.z].tag == "Road")
                            {
                                //Cell to update
                                Cells updateCell = cell;
                                updateCell.x = updateCell.x - 1;

                                PlaceRoad(true, updateCell);
                            }
                        }
                    }
                    //South
                    if (cell.z > 0)
                    {
                        if (grid[cell.x, cell.z - 1] != null)
                        {
                            if (grid[cell.x, cell.z - 1].tag == "Road")
                            {
                                //Cell to update
                                Cells updateCell = cell;
                                updateCell.z = updateCell.z - 1;

                                PlaceRoad(true, updateCell);
                            }
                        }
                    }
                    //West
                    if (cell.x < size.x - 1)
                    {
                        if (grid[cell.x + 1, cell.z] != null)
                        {
                            if (grid[cell.x + 1, cell.z].tag == "Road")
                            {
                                //Cell to update
                                Cells updateCell = cell;
                                updateCell.x = updateCell.x + 1;

                                PlaceRoad(true, updateCell);
                            }
                        }
                    }
                    //North
                    if (cell.z < size.z - 1)
                    {
                        if (grid[cell.x, cell.z + 1] != null)
                        {
                            if (grid[cell.x, cell.z + 1].tag == "Road")
                            {
                                //Cell to update
                                Cells updateCell = cell;
                                updateCell.z = updateCell.z + 1;

                                PlaceRoad(true, updateCell);
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }

    bool CheckForEdge (int x, int z)
    {
        return x >= 0 && x < size.x && z >= 0 && z < size.z;
    }
}
