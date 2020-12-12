using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int maxGridSize = 10;
    public int tilesize = 1;
    public SpriteRenderer tile;

    private Vector3 offset;
    //Tracks the number of objects contained in each tile of the grid
    private int[,] grid;

    private void Awake() {

        ClearGrid();

        float xOffset = transform.position.x % 1;

        if (xOffset > tilesize / 2)
            xOffset -= tilesize;

        float zOffset = transform.position.z % 1;
        offset = new Vector3(xOffset, 0, zOffset);
    }

    /// <summary>
    /// Tranform the position of an object into a index on the grid
    /// </summary>
    /// <param name="position"> The position of an object </param>
    /// <returns> A GridIndex containing the x and y index of the tile </returns>
    public Vector2Int GetTileIndexFromPosition(Vector3 position) {
        return new Vector2Int((int)position.x, (int)position.z);
    }

    /// <summary>
    /// Clears the grid
    /// </summary>
    public void ClearGrid() {
        grid = new int[maxGridSize, maxGridSize];
    }

    /// <summary>
    /// Increment the number of objects at index
    /// </summary>
    /// <param name="gridIndex"> The index of the tile </param>
    public void AddObjectToGrid(Vector2Int gridIndex) {

        grid[gridIndex.x, gridIndex.y] += 1;
        //Debug.Log("Object Add to grid at index " + gridIndex.x + ", " + gridIndex.y + ": Number of objects in tile = " + grid[gridIndex.x, gridIndex.y]);
    }

    /// <summary>
    /// Checks the number of objects in a tile and returns true if there are more than 0
    /// </summary>
    /// <param name="gridIndex"> The index of the tile </param>
    /// <returns> A bool indicating wether the tile contains an object of not </returns>
    public bool HasObjectInTile(Vector2Int gridIndex) {

        return grid[gridIndex.x, gridIndex.y] > 0;
    }

    /// <summary>
    /// Gets the number of objects in a tile
    /// </summary>
    /// <param name="gridIndex"> The index of the tile </param>
    /// <returns> The numbers of objects in the tile </returns>
    public int GetNumberOfObjectsInTile(Vector2Int gridIndex) {

        return grid[gridIndex.x, gridIndex.y];
    }

    /// <summary>
    /// Moves an object on the grid
    /// </summary>
    /// <param name="objectTransform"> The object to move</param>
    /// <param name="newPosition"> The new position of the object </param>
    public void MoveObjectOnGrid(Transform objectTransform, Vector3 newPosition) {
        float newXPos = tilesize * (int)(newPosition.x / tilesize);
        float newZPos = tilesize * (int)(newPosition.z / tilesize);
        Vector3 newPos = new Vector3(newXPos, objectTransform.position.y, newZPos) + new Vector3(1, 0, 1) * tilesize / 2 + offset;
        objectTransform.position = newPos;
    }

    /// <summary>
    /// Checks if the position is inside the grid
    /// </summary>
    /// <param name="position"> The position of the object </param>
    /// <returns> A bool indicating wether the position is inside the bounds of the grid </returns>
    public bool IsInsideGridBounds(Vector3 position) {

        if (position.x > maxGridSize || position.z > maxGridSize || position.x < 0 || position.z < 0)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the position is inside the grid
    /// </summary>
    /// <param name="position"> The position of the object </param>
    /// <returns> A bool indicating wether the position is inside the bounds of the grid </returns>
    public bool IsInsideGridBounds(Vector2Int index) {

        if (index.x > maxGridSize-1 || index.y > maxGridSize-1 || index.x < 0 || index.y < 0)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the position is inside the grid
    /// </summary>
    /// <param name="position"> The position of the object </param>
    /// <returns> A bool indicating wether the position is inside the bounds of the grid </returns>
    public bool IndexesInsideGridBounds(List<Vector2Int> indexes) {

        foreach (Vector2Int index in indexes) {
            if (!IsInsideGridBounds(index))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if all values of a list of indexes are the same
    /// </summary>
    /// <param name="indexes"> The list of indexes </param>
    /// <returns> A bool indicating if all the grid values of indexes are the same or not </returns>
    public bool IndexesHaveSameValue(List<Vector2Int> indexes) {

        int firstIndexValue = GetNumberOfObjectsInTile(indexes[0]);
        foreach (Vector2Int index in indexes) {
            if (GetNumberOfObjectsInTile(index) != firstIndexValue)
                return false;
        }

        return true;
    }
}
