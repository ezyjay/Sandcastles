using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileValue
{
    public int height = 0;
    public List<int> verticalValues = new List<int>();

    public TileValue(int listSize) {
        height = 0;
        verticalValues = new List<int>(new int[listSize]);
    }

    public void AddObjectAtVerticalIndex(int index) {
        verticalValues[index] = 1;
    }

    public void RemoveObjectAtVerticalIndex(int index) {
        verticalValues[index] = 0;
    }

    public bool HasObjectAtVerticalIndex(int index) {
        return verticalValues[index] == 1;
    }
}

public class GridDrawer : MonoBehaviour
{
    public int maxGridSize = 10;
    public int tileSize = 1;
    public SpriteRenderer tile;

    private Vector3 offset;
    //Tracks the number of objects contained in each tile of the grid
    private TileValue[,] grid;

    private void Awake() {

        ClearGrid();

        float xOffset = transform.position.x % 1;

        if (xOffset > tileSize / 2)
            xOffset -= tileSize;

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

         grid = new TileValue[maxGridSize, maxGridSize];
        for (int i = 0; i < maxGridSize; i++) {
            for (int j = 0; j < maxGridSize; j++) {
                grid[i, j] = new TileValue(maxGridSize);
            }
        }
    }

    /// <summary>
    /// Increment the number of objects at index
    /// </summary>
    /// <param name="index"> The index of the tile </param>
    public void IncrementTileValueAtIndex(Vector2Int index, int maxTileValue = int.MaxValue) {

        int incrementedTileValue = GetHeightAtIndex(index) + tileSize;
        if (incrementedTileValue <= maxTileValue + tileSize) {
            SetHeightAtIndex(index, incrementedTileValue);
            AddValueAtVerticalIndex(index, incrementedTileValue - 1);
            //Debug.Log("Object Add to grid at index " + gridIndex.x + ", " + gridIndex.y + ": Number of objects in tile = " + grid[gridIndex.x, gridIndex.y]);
        }
    }

    /// <summary>
    /// Increment the number of objects at indexes
    /// </summary>
    /// <param name="indexes"> The list of indexes to add </param>
    public void IncrementTileValueIndexes(List<Vector2Int> indexes, int maxTileValue = int.MaxValue) {
        foreach (Vector2Int index in indexes)
            IncrementTileValueAtIndex(index, maxTileValue);
    }

    /// <summary>
    /// Checks the number of objects in a tile and returns true if there are more than 0
    /// </summary>
    /// <param name="index"> The index of the tile </param>
    /// <returns> A bool indicating wether the tile contains an object of not </returns>
    public bool HasObjectAtIndex(Vector2Int index) {
        return GetHeightAtIndex(index) > 0;
    }

    /// <summary>
    /// Gets the number of objects in a tile
    /// </summary>
    /// <param name="index"> The index of the tile </param>
    /// <returns> The numbers of objects in the tile </returns>
    public int GetHeightAtIndex(Vector2Int index) {
        return GetTileValue(index).height;
    }

    /// <summary>
    /// Sets the number of objects in a tile
    /// </summary>
    /// <param name="index">The index of the tile </param>
    /// <param name="newHeight">The new value </param>
    public void SetHeightAtIndex(Vector2Int index, int newHeight) {
        GetTileValue(index).height = newHeight;
    }

    /// <summary>
    /// Gets the TileValue object at an index
    /// </summary>
    /// <param name="index">The index of the tile </param>
    /// <returns> the TileValue </returns>
    public TileValue GetTileValue(Vector2Int index) {
        return grid[index.x, index.y];
    }

    /// <summary>
    /// Gets a list of ints corresponding to the vertical tiles at an index
    /// </summary>
    /// <param name="index"> The index on the tile</param>
    /// <returns></returns>
    public List<int> GetVerticalValuesAtIndex(Vector2Int index) {
        return GetTileValue(index).verticalValues;
    }

    /// <summary>
    /// Gets the vertival value at an index
    /// </summary>
    /// <param name="index">The tile index</param>
    /// <param name="verticalIndex">The vertical index</param>
    /// <returns> An int indicating if there is an object at the index</returns>
    public int GetVerticalValueAtIndex(Vector2Int index, int verticalIndex) {
        return GetTileValue(index).verticalValues[verticalIndex];
    }

    /// <summary>
    /// Adds an object to a vertical index
    /// </summary>
    /// <param name="index">The tile index</param>
    /// <param name="verticalIndex">The vertical index</param>
    public void AddValueAtVerticalIndex(Vector2Int index, int verticalIndex) {

        //Add the object
        GetTileValue(index).AddObjectAtVerticalIndex(verticalIndex);

        //Ensure that tileHeightValue is coherent with vertical values
        EnsureHeightAndVerticalValuesAreCoherent(index, verticalIndex);
    }

    /// <summary>
    /// Adds an object to a vertical index at multiples indexes
    /// </summary>
    /// <param name="index">The list of indexes</param>
    /// <param name="verticalIndex">The vertical index</param>
    public void AddValueAtVerticalIndexes(List<Vector2Int> indexes, int verticalIndex) {

        foreach (Vector2Int index in indexes) {
            AddValueAtVerticalIndex(index, verticalIndex);
        }
    }

    /// <summary>
    /// Removes an object from a vertical index
    /// </summary>
    /// <param name="index"> The tile index</param>
    /// <param name="verticalIndex"> The vertical index</param>
    public void RemoveValueAtVerticalIndex(Vector2Int index, int verticalIndex) {

        //Remove the object
        GetTileValue(index).RemoveObjectAtVerticalIndex(verticalIndex);

        //Ensure that tileHeightValue is coherent with vertical values
        EnsureHeightAndVerticalValuesAreCoherent(index, verticalIndex);
    }

    /// <summary>
    /// Removes an object to a vertical index at multiples indexes
    /// </summary>
    /// <param name="index">The list of indexes</param>
    /// <param name="verticalIndex">The vertical index</param>
    public void RemoveValueAtVerticalIndexes(List<Vector2Int> indexes, int verticalIndex) {

        foreach (Vector2Int index in indexes) {
            RemoveValueAtVerticalIndex(index, verticalIndex);
        }
    }


    private void EnsureHeightAndVerticalValuesAreCoherent(Vector2Int index, int verticalIndex) {
        int newTileHeightValue = 0;
        for (int i = GetHeightAtIndex(index); i >= 0; i--) {
            if (GetTileValue(index).HasObjectAtVerticalIndex(i)) {
                newTileHeightValue = i + 1;
                break;
            }
        }
        SetHeightAtIndex(index, newTileHeightValue);
    }

    /// <summary>
    /// Moves an object on the grid
    /// </summary>
    /// <param name="objectTransform"> The object to move</param>
    /// <param name="newPosition"> The new position of the object </param>
    public void MoveObjectOnGrid(Transform objectTransform, Vector3 newPosition, bool changeY = false) {
        float newXPos = tileSize * (int)(newPosition.x / tileSize);
        float newZPos = tileSize * (int)(newPosition.z / tileSize);
        Vector3 newPos = new Vector3(newXPos, objectTransform.position.y, newZPos) + new Vector3(1, 0, 1) * tileSize / 2 + offset;
        if (changeY) {
            float newYPos = tileSize * (int)(newPosition.y / tileSize);
            newPos = new Vector3(newXPos, newYPos, newZPos) + new Vector3(1, 0, 1) * tileSize / 2 + offset;
        }
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

        int firstIndexValue = GetHeightAtIndex(indexes[0]);
        foreach (Vector2Int index in indexes) {
            if (GetHeightAtIndex(index) != firstIndexValue)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if all values of a list of indexes are the smaller than a certain index
    /// </summary>
    /// <param name="indexes"> The list of indexes </param>
    /// <returns> A bool indicating if all the grid values of indexes are >= to main index </returns>
    public bool IndexesHaveSameOrBiggerValueThanIndex(Vector2Int mainIndex, List<Vector2Int> indexes) {

        foreach (Vector2Int index in indexes) {
            if (GetHeightAtIndex(index) < GetHeightAtIndex(mainIndex))
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
