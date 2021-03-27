using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum OperationType
{
    ADD = 0,
    SUBTRACT = 1,
    DECORATE = 2,
}

public class SandcastleBuildManager : MonoBehaviour
{
    [Header("Parameters")]
    public bool showDebugInfo = false;
    public bool isInBuildMode = false;
    public bool enforceRulesForPositioning = false;
    public OperationType currentOperationType = OperationType.ADD;
    public SandBlobType currentSandBlobType = SandBlobType.CUBE_1x1;
    [ShowIf("currentOperationType", OperationType.DECORATE)]
    public SandDecorationType currentDecorationType = SandDecorationType.FLAG;
    public int maxHoleSize = 2;

    [Header("Components")]
    public GridDrawer gridDrawer;
    public SandBlobManager sandBlobManager;
    public DecorationManager decorationManager;
    public SandClayObjects sandClayObjects;
    public Transform extraPreviewsParent;

    private RaycastHit hit;
    private Ray ray;
    private bool canBuild = false;
    private bool isFillingHole = false;
    private Vector3 spawnPosition = Vector3.zero;
    private Vector2Int gridIndexSpawn = Vector2Int.zero;
    private List<Vector2Int> shapeIndexes = new List<Vector2Int>();
    private List<OperationType> allDoneOperations = new List<OperationType>();
    private List<OperationType> undoneOperations = new List<OperationType>();
    private List<GameObject> extraPreviewObjects = new List<GameObject>();
    private List<Vector2Int> extraPreviewObjectGridIndexes = new List<Vector2Int>();
    private Vector2Int previousDraggedMouseIndex;
    private Vector2 previousDragDirection;

    [EditorButton]
    public void ResetBuildZone() {
        sandClayObjects.DestroyAllSolids();
        gridDrawer.ClearGrid();
        decorationManager.ClearAllDecorations();
        allDoneOperations.Clear();
        undoneOperations.Clear();
    }

    private void Awake() {
        if (sandBlobManager == null)
            sandBlobManager = GetComponent<SandBlobManager>();

        if (sandClayObjects == null)
            sandClayObjects = GetComponent<SandClayObjects>();

#if !UNITY_EDITOR
        showDebugInfo = false;
#endif
    }

    private void OnValidate() {
#if UNITY_EDITOR
        sandBlobManager.ChangeBlobType(currentSandBlobType);
#endif
    }

    private void ShowDebugInfo(string header, Vector2Int index) {
        if (showDebugInfo) {
            //DEBUG
            Debug.Log(header + ": " + index + " ==> Value = " + gridDrawer.GetHeightAtIndex(index) + " // Vertical objects list = ");
            for (int i = gridDrawer.GetHeightAtIndex(index) - 1; i >= 0; i--) {
                Debug.Log(i + ": " + gridDrawer.GetVerticalValuesAtIndex(index)[i]);
            }
            Debug.Log("------------------------------------------------------------------------------------------------------------------------");
        }
    }

    private void OnEnable() {
        gridDrawer.gameObject.SetActive(true);

        sandClayObjects.Initialize();
        sandBlobManager.Initialize(currentSandBlobType);
    }

    private void Update() {
        if (isInBuildMode) {

            //Reset blob preview and force show solids
            canBuild = false;
            isFillingHole = false;
            sandClayObjects.ToggleSolids(true);
            sandBlobManager.BlobAtCorrectPosition(false);
            sandBlobManager.ToggleBlobPreview(false);

            //Ray cast down and if it's a buildable area, spawn sand blob on mouse click
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.cyan);
            if (Physics.Raycast(ray, out hit)) {

                //ADD
                if (currentOperationType == OperationType.ADD) {

                    //Move sandBlobPreview on the grid
                    gridDrawer.MoveObjectOnGrid(sandBlobManager.CurrentSandBlobPreview.transform, hit.point);

                    //Check if it's inside grid bounds
                    if (gridDrawer.IsInsideGridBounds(sandBlobManager.CurrentSandBlobPreview.transform.localPosition)) {

                        //Show blob preview
                        sandBlobManager.ToggleBlobPreview(true);

                        //Change position on grid on blob preview and determine spawn position and the surface it covers
                        gridIndexSpawn = gridDrawer.GetTileIndexFromPosition(sandBlobManager.CurrentSandBlobPreview.transform.position);
                        spawnPosition = sandBlobManager.GetSpawnPosition();
                        shapeIndexes = GetShapeGridIndexes(gridIndexSpawn);

                        //Check if current grid position already has something in it to set the Y value of blob preview
                        int hitPointHeight = (int)(hit.point.y + .2f);
                        if (gridDrawer.HasObjectAtIndex(gridIndexSpawn)) {

                            //If nothing at this height and it's lower than total height we are filling a hole
                            if (gridDrawer.GetVerticalValueAtIndex(gridIndexSpawn, hitPointHeight) == 0 && hitPointHeight < gridDrawer.GetHeightAtIndex(gridIndexSpawn)) {

                                isFillingHole = true;
                            }
                            //Place on top of column of objects
                            else {
                                hitPointHeight = gridDrawer.GetHeightAtIndex(gridIndexSpawn);
                            }
                            float newYPosition = spawnPosition.y + hitPointHeight * gridDrawer.tileSize;
                            spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                            sandBlobManager.SetBlobPreviewYPosition(newYPosition - .2f * gridDrawer.tileSize);

                        } else {
                            sandBlobManager.SetBlobPreviewYPosition(transform.position.y);
                        }

                        //Check if shape of blob is inside the grid and on a flat surface
                        if (gridDrawer.AreIndexesInsideGridBounds(shapeIndexes) && (!enforceRulesForPositioning || gridDrawer.IndexesHaveSameOrBiggerValueThanIndex(gridIndexSpawn, shapeIndexes))) {

                            //Can build
                            sandBlobManager.BlobAtCorrectPosition(true);
                            canBuild = true;
                        }
                    }
                }

                //SUBSTRACT
                else if (currentOperationType == OperationType.SUBTRACT) {

                    //Move sandBlobPreview on the grid
                    gridDrawer.MoveObjectOnGrid(sandBlobManager.CurrentSandBlobPreview.transform, hit.point, true);

                    gridIndexSpawn = gridDrawer.GetTileIndexFromPosition(sandBlobManager.CurrentSandBlobPreview.transform.position);

                    if (sandBlobManager.CurrentTempRemoveObject != null)
                        sandBlobManager.CurrentTempRemoveObject.transform.position = new Vector3(sandBlobManager.CurrentSandBlobPreview.transform.position.x, sandBlobManager.CurrentSandBlobPreview.transform.position.y + .5f, sandBlobManager.CurrentSandBlobPreview.transform.position.z);

                    //Check if it's inside grid bounds and there is an object at index
                    if (gridDrawer.IsInsideGridBounds(sandBlobManager.CurrentSandBlobPreview.transform.localPosition) && gridDrawer.HasObjectAtIndex(gridIndexSpawn)) {

                        //Show blob preview
                        sandBlobManager.ToggleBlobPreview(true);

                        //Check not leaving too many holes
                        if (!enforceRulesForPositioning && gridDrawer.GetTileValue(gridIndexSpawn).HasObjectAtVerticalIndex((int)sandBlobManager.CurrentTempRemoveObject.transform.position.y)
                            || gridDrawer.CanRemoveObjectAtVerticalIndex(gridIndexSpawn, gridDrawer.tileSize * maxHoleSize, (int)sandBlobManager.CurrentTempRemoveObject.transform.position.y)) {

                            //Can build
                            sandBlobManager.BlobAtCorrectPosition(true);
                            canBuild = true;
                        }
                    }
                }

                //DECORATE
                else if (currentOperationType == OperationType.DECORATE) {

                    if (decorationManager.CurrentDecoration != null && decorationManager.CurrentDecoration.decorationObject != null) {

                        decorationManager.CurrentDecoration.decorationObject.transform.position = hit.point;
                        Quaternion newRotation = Quaternion.FromToRotation(decorationManager.CurrentDecoration.decorationObject.transform.up, hit.normal);
                        decorationManager.CurrentDecoration.decorationObject.transform.rotation = newRotation * decorationManager.CurrentDecoration.decorationObject.transform.rotation;
                        canBuild = true;
                    }
                }
            }
        }
    }

    public void UndoLastAction() {

        if (allDoneOperations.Count > 0) {
            OperationType lastOperation = allDoneOperations[allDoneOperations.Count - 1];
            undoneOperations.Add(lastOperation);
            if (allDoneOperations.Count > 0)
                allDoneOperations.RemoveAt(allDoneOperations.Count - 1);
            else
                allDoneOperations.Clear();

            if (lastOperation == OperationType.ADD || lastOperation == OperationType.SUBTRACT) {
                Vector3 removedObjectPosition = sandClayObjects.Undo(out bool lastObjectWasSubstract);
                if (removedObjectPosition != Vector3.zero) {
                    Vector2Int removedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(removedObjectPosition);
                    List<Vector2Int> removedObjectShapeIndexes = GetShapeGridIndexes(removedObjectGridIndex);
                    if (lastObjectWasSubstract)
                        gridDrawer.AddValueAtVerticalIndexes(removedObjectShapeIndexes, (int)removedObjectPosition.y);
                    else
                        gridDrawer.RemoveValueAtVerticalIndexes(removedObjectShapeIndexes, (int)removedObjectPosition.y);

                    //DEBUG
                    ShowDebugInfo("UNDO", removedObjectGridIndex);
                }
            } else if (lastOperation == OperationType.DECORATE) {
                decorationManager.UndoLastAddedDecoration();
            }
        }
    }

    public void RedoAction() {

        if (undoneOperations.Count > 0) {
            OperationType lastOperation = undoneOperations[undoneOperations.Count - 1];
            if (undoneOperations.Count > 0)
                undoneOperations.RemoveAt(undoneOperations.Count - 1);
            else
                undoneOperations.Clear();
            allDoneOperations.Add(lastOperation);

            if (lastOperation == OperationType.ADD || lastOperation == OperationType.SUBTRACT) {
                Vector3 addedObjectPosition = sandClayObjects.Redo(out bool lastRemovedObjectWasSubstract);
                if (addedObjectPosition != Vector3.zero) {
                    Vector2Int addedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(addedObjectPosition);
                    List<Vector2Int> addedObjectShapeIndexes = GetShapeGridIndexes(addedObjectGridIndex);
                    if (lastRemovedObjectWasSubstract)
                        gridDrawer.RemoveValueAtVerticalIndexes(addedObjectShapeIndexes, (int)addedObjectPosition.y);
                    else
                        gridDrawer.AddValueAtVerticalIndexes(addedObjectShapeIndexes, (int)addedObjectPosition.y);

                    //DEBUG
                    ShowDebugInfo("REDO", addedObjectGridIndex);
                }
            } else if (lastOperation == OperationType.DECORATE) {
                decorationManager.RedoLastAction();
            }
        }
    }

    public void AddObjectOnMouseUp() {

        previousDragDirection = Vector2.zero;
        if (canBuild) {
            AddSandBlob();
            canBuild = false;
        } else {
            ClearExtraPreviews();
        }
    }

    public void RotateObjectWithScroll(float scrollSpeed) {

        if (currentOperationType == OperationType.ADD) {
            sandBlobManager.CurrentSandBlobPreview.transform.Rotate(Vector3.up * 90);
            if (extraPreviewObjects.Count > 0) {
                foreach (GameObject extraPreview in extraPreviewObjects) {
                    extraPreview.transform.Rotate(Vector3.up * 90);
                }
            }
        } 
        else if (currentOperationType == OperationType.DECORATE) {
            decorationManager.CurrentDecoration.decorationObject.transform.Rotate(Vector3.up * scrollSpeed * Time.deltaTime);
        }
    }

    public void ChangeSandBlobType(SandBlobType sandBlobType) {
        currentSandBlobType = sandBlobManager.ChangeBlobType(sandBlobType);
        if (currentOperationType == OperationType.DECORATE) {
            ChangeBuildMode(OperationType.ADD);
            decorationManager.DestroyCurrentDecoration();
        }
    }

    public void ChangeDecorationType(SandDecorationType decorationType) {
        ChangeBuildMode(OperationType.DECORATE);
        currentDecorationType = decorationType;
        decorationManager.CreateDecorationObject(decorationType, destroyPreviousObject: true);
    }

    public void ChangeBuildMode(OperationType newOperationType) {
        currentOperationType = newOperationType;
        if (currentOperationType == OperationType.SUBTRACT) {
            sandBlobManager.CurrentTempRemoveObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition, Quaternion.identity, currentOperationType, true).gameObject;
        } else if (currentOperationType == OperationType.ADD) {
            if (sandBlobManager.CurrentTempRemoveObject != null)
                Destroy(sandBlobManager.CurrentTempRemoveObject);
        } else if (currentOperationType == OperationType.DECORATE) {
            if (sandBlobManager.CurrentTempRemoveObject != null)
                Destroy(sandBlobManager.CurrentTempRemoveObject);
        }
    }

    public void OnMouseDragged(Vector3 startPosition, Vector3 currentMousePosition) {

        if (currentOperationType == OperationType.ADD) {

            Vector2Int startGridIndex = gridDrawer.GetTileIndexFromPosition(startPosition);
            Vector2Int currentGridIndex = gridDrawer.GetTileIndexFromPosition(currentMousePosition);
            Vector2 currentDragDirection = (new Vector2(currentGridIndex.x, currentGridIndex.y) - new Vector2(previousDraggedMouseIndex.x, previousDraggedMouseIndex.y)).normalized;

            //If current mouse position changed index 
            if ((int)currentSandBlobType < 100 && currentGridIndex != previousDraggedMouseIndex 
                || (int)currentSandBlobType >= 100 && ((currentGridIndex.x - previousDraggedMouseIndex.x) == 3 || (currentGridIndex.y - previousDraggedMouseIndex.y) == 3)) {

                //If preview not already created for this index, add preview
                if (!extraPreviewObjectGridIndexes.Contains(currentGridIndex)) {

                    if (!extraPreviewObjectGridIndexes.Contains(previousDraggedMouseIndex)) {

                        float yPosition = gridDrawer.GetHeightAtIndex(previousDraggedMouseIndex) * gridDrawer.tileSize;
                        Vector3 spawnPosition = new Vector3(previousDraggedMouseIndex.x, yPosition, previousDraggedMouseIndex.y);

                        GameObject extraPreview = GameObject.Instantiate(sandBlobManager.CurrentSandBlobPreview, spawnPosition, sandBlobManager.CurrentSandBlobPreview.transform.rotation, extraPreviewsParent);
                        gridDrawer.MoveObjectOnGrid(extraPreview.transform, spawnPosition);

                        extraPreview.SetActive(true);
                        extraPreviewObjects.Add(extraPreview);
                        extraPreviewObjectGridIndexes.Add(previousDraggedMouseIndex);
                    }

                //Else remove preview
                } else {
                    int indexToRemove = extraPreviewObjectGridIndexes.IndexOf(previousDraggedMouseIndex);

                    if (indexToRemove >= 0) {
                        Destroy(extraPreviewObjects[indexToRemove]);
                        extraPreviewObjects.RemoveAt(indexToRemove);
                        extraPreviewObjectGridIndexes.RemoveAt(indexToRemove);
                    }
                }
            }

            if ((int)currentSandBlobType >= 100 && ((currentGridIndex.x - previousDraggedMouseIndex.x) == 3 || (currentGridIndex.y - previousDraggedMouseIndex.y) == 3) || (int)currentSandBlobType < 100)
                previousDraggedMouseIndex = currentGridIndex;

            previousDragDirection = currentDragDirection;
        }
    }

    public void OnMouseDown(Vector3 position) {
        previousDraggedMouseIndex = gridDrawer.GetTileIndexFromPosition(position);
    }

    private void ClearExtraPreviews() {
        extraPreviewObjects.Clear();
        extraPreviewObjectGridIndexes.Clear();
        foreach (Transform extraPreviews in extraPreviewsParent) {
            Destroy(extraPreviews.gameObject);
        }
    }

    private void AddSandBlob() {

        //ADD
        if (currentOperationType == OperationType.ADD) {

            GameObject clayObject = CreateBlob(sandBlobManager.CurrentSandBlobPreview.transform, true);

            if (extraPreviewObjects.Count > 0) {

                foreach (GameObject extraPreview in extraPreviewObjects) {
                    if (Vector3.Distance(extraPreview.transform.position, sandBlobManager.CurrentSandBlobPreview.transform.position) > 0.5f) {
                        CreateBlob(extraPreview.transform);
                        allDoneOperations.Add(currentOperationType);
                    }
                }
                ClearExtraPreviews();
            }
            allDoneOperations.Add(currentOperationType);
            sandClayObjects.CommitClayObject(clayObject);
            sandBlobManager.CurrentSandBlobPreview.transform.rotation = Quaternion.identity;
        }
        //SUBSTRACT
        else if (currentOperationType == OperationType.SUBTRACT) {

            if (sandBlobManager.CurrentTempRemoveObject != null) {
                sandClayObjects.CommitClayObject(sandBlobManager.CurrentTempRemoveObject);
                gridDrawer.RemoveValueAtVerticalIndex(gridIndexSpawn, (int)sandBlobManager.CurrentTempRemoveObject.transform.position.y);
            }

            //New remove object preview
            sandBlobManager.CurrentTempRemoveObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition, Quaternion.identity, currentOperationType, true).gameObject;

            allDoneOperations.Add(currentOperationType);

            //DEBUG
            ShowDebugInfo("SUBSTRACT OBJECT", gridIndexSpawn);
        }

        //DECORATE
        else if (currentOperationType == OperationType.DECORATE) {

            decorationManager.CreateDecorationObject(currentDecorationType, commitCurrentObject: true);
            allDoneOperations.Add(currentOperationType);
        }

        undoneOperations.Clear();

        //Update solids
        sandClayObjects.UpdateSolids();
    }

    private GameObject CreateBlob(Transform preview, bool temporaryAdd = false) {

        //Create clay object
        Transform extraClayObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, preview.position, preview.rotation, temporaryAdd: temporaryAdd);

        //Spawn collider on clay object to be able to detect mouse over the blob
        sandBlobManager.EnableSandBlobCollider(extraClayObject);

        gridIndexSpawn = gridDrawer.GetTileIndexFromPosition(preview.position);
        spawnPosition = sandBlobManager.GetSpawnPosition();
        shapeIndexes = GetShapeGridIndexes(gridIndexSpawn);

        //Updade grid and other clay objects
        if (isFillingHole)
            gridDrawer.AddValueAtVerticalIndexes(shapeIndexes, (int)preview.position.y);
        else
            gridDrawer.IncrementTileValueIndexes(shapeIndexes, gridDrawer.GetHeightAtIndex(gridIndexSpawn));

        if (currentSandBlobType == SandBlobType.SPHERE_3x3)
            gridDrawer.IncrementTileValueAtIndex(gridIndexSpawn);

        //DEBUG
        ShowDebugInfo("ADD OBJECT", gridIndexSpawn);

        return extraClayObject.gameObject;
    }

    private List<Vector2Int> GetShapeGridIndexes(Vector2Int middleGridIndex, int addToAmountToAdd = 0) {

        Vector2Int amountToAdd = new Vector2Int((int)sandBlobManager.CurrentSandBlob.data.size.x / 2, (int)sandBlobManager.CurrentSandBlob.data.size.z / 2);

        if (addToAmountToAdd != 0)
            amountToAdd += addToAmountToAdd * Vector2Int.one;

        bool fillDiagonalNeighbours = true;
        if (sandBlobManager.CurrentSandBlob.data.type == SandBlobType.CYLINDER_3x1)
            fillDiagonalNeighbours = false;
        
        return gridDrawer.GetNeighbourIndexes(middleGridIndex, amountToAdd, fillDiagonalNeighbours); ;
    }

    
}
