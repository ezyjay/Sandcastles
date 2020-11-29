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

    public class GridIndex
    {
        public int x;
        public int y;

        public GridIndex(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    private void Awake() {

        ClearGrid(); 

        float xOffset = transform.position.x % 1;

        if (xOffset > 0.5f)
            xOffset -= tilesize;

        float zOffset = transform.position.z % 1;
        offset = new Vector3(xOffset, 0, zOffset);
    }

    /// <summary>
    /// Tranform the position of an object into a index on the grid
    /// </summary>
    /// <param name="position"> The position of an object </param>
    /// <returns> A GridIndex containing the x and y index of the tile </returns>
    public GridIndex GetTileIndexFromPosition(Vector3 position) {
        return new GridIndex((int)position.x, (int)position.z);
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
    public void AddObjectToGrid(GridIndex gridIndex) {

        grid[gridIndex.x, gridIndex.y] += 1;
        Debug.Log("Object Add to grid at index " + gridIndex.x + ", " + gridIndex.y + ": Number of objects in tile = " + grid[gridIndex.x, gridIndex.y]);
    }

    /// <summary>
    /// Checks the number of objects in a tile and returns true if there are more than 0
    /// </summary>
    /// <param name="gridIndex"> The index of the tile </param>
    /// <returns> A bool indicating wether the tile contains an object of not </returns>
    public bool HasObjectInTile(GridIndex gridIndex) {

        return grid[gridIndex.x, gridIndex.y] > 0;
    }

    /// <summary>
    /// Gets the number of objects in a tile
    /// </summary>
    /// <param name="gridIndex"> The index of the tile </param>
    /// <returns> The numbers of objects in the tile </returns>
    public int GetNumberOfObjectsInTile(GridIndex gridIndex) {

        return grid[gridIndex.x, gridIndex.y];
    }

    /// <summary>
    /// Moves an object on the grid
    /// </summary>
    /// <param name="objectTransform"> The object to move</param>
    /// <param name="newPosition"> The new position of the object </param>
    public void MoveObjectOnGrid(Transform objectTransform, Vector3 newPosition) {
        float newXPos = tilesize * Mathf.Round(newPosition.x / tilesize);
        float newZPos = tilesize * Mathf.Round(newPosition.z / tilesize);
        Vector3 newPos = new Vector3(newXPos, objectTransform.position.y, newZPos) + new Vector3(1, 0, 1) * tilesize / 2 + offset;
        objectTransform.position = newPos;
    }

    /// <summary>
    /// Checks if the position is inside the grid
    /// </summary>
    /// <param name="position"> The position of the object </param>
    /// <returns> A bool indicating wether the position is inside the bounds of the grid </returns>
    public bool IsPositionInGrid(Vector3 position) {

        position += offset;
        if (position.x > maxGridSize || position.z > maxGridSize || position.x < 0 || position.z < 0)
            return false;

        return true;
    }
}
