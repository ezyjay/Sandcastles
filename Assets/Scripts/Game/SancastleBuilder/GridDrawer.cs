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
    /// <param name="index"> The index of the tile </param>
    public void AddObjectToGridAtIndex(Vector2Int index) {

        grid[index.x, index.y] += 1;
        //Debug.Log("Object Add to grid at index " + gridIndex.x + ", " + gridIndex.y + ": Number of objects in tile = " + grid[gridIndex.x, gridIndex.y]);
    }

    /// <summary>
    /// Increment the number of objects at indexes
    /// </summary>
    /// <param name="indexes"> The list of indexes to add </param>
    public void AddObjectToGridAtIndexes(List<Vector2Int> indexes) {
        foreach (Vector2Int index in indexes)
            AddObjectToGridAtIndex(index); 
    }

    /// <summary>
    /// Checks the number of objects in a tile and returns true if there are more than 0
    /// </summary>
    /// <param name="index"> The index of the tile </param>
    /// <returns> A bool indicating wether the tile contains an object of not </returns>
    public bool HasObjectAtIndex(Vector2Int index) {

        return grid[index.x, index.y] > 0;
    }

    /// <summary>
    /// Gets the number of objects in a tile
    /// </summary>
    /// <param name="index"> The index of the tile </param>
    /// <returns> The numbers of objects in the tile </returns>
    public int GetValueAtIndex(Vector2Int index) {

        return grid[index.x, index.y];
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
    public bool AreIndexesInsideGridBounds(List<Vector2Int> indexes) {

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

        int firstIndexValue = GetValueAtIndex(indexes[0]);
        foreach (Vector2Int index in indexes) {
            if (GetValueAtIndex(index) != firstIndexValue)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Find the indexes in a square that surrounds a center index
    /// </summary>
    /// <param name="centerIndex"> The center index </param>
    /// <param name="distanceToCheck"> The distance to find neighbours </param>
    /// <param name="getDiagonalNeighbours"> A bool indicating if we should get the diagonaal neighbours or not</param>
    /// <returns> Returns a list of neighbour indexes</returns>
    public List<Vector2Int> GetNeighbourIndexes(Vector2Int centerIndex, Vector2Int distanceToCheck, bool getDiagonalNeighbours = true) {

        List<Vector2Int> indexes = new List<Vector2Int>();

        if (distanceToCheck == Vector2Int.zero) {
            indexes.Add(centerIndex);
        }
        else {
            for (int i = centerIndex.x - distanceToCheck.x; i <= centerIndex.x + distanceToCheck.x; i++) {
                for (int j = centerIndex.y - distanceToCheck.y; j <= centerIndex.y + distanceToCheck.y; j++) {

                    Vector2Int currentIndex = new Vector2Int(i, j);
                    //If we don't want to fill diagonal neighbours check that the distance is the same as the amount to add
                    if (getDiagonalNeighbours || Vector2Int.Distance(currentIndex, centerIndex) == distanceToCheck.x || currentIndex == centerIndex) {
                        
                        indexes.Add(currentIndex);
                    }
                }
            }
        }

        return indexes;
    }
}
