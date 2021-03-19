using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandcastleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject uiPanel;
    public GameObject largerShapes, smallerShapes, topPieces, decorations;
    public GameObject disabledShapeSelection;

    [Header("Button")]
    public Button smallShapesButton;

    [Header("Texts")]
    public GameObject substractModeText;
    public GameObject buildModeText;
    public GameObject controlsPanel, showControlsText, hideControlsText;

    public Action<SandBlobType> ChangeSandBlobTypeButtonPressed;
    public Action<OperationType> ChangeBuildModeButtonPressed;
    public Action<SandDecorationType> ChangeDecorationTypeButtonPressed;
    public Action ResetBuildZoneButtonPressed, UndoLastActionButtonPressed, RedoActionButtonPressed;

    private bool largeShapes = false;
    private bool removeModeActive = false;
    private bool controlsPanelActive = false;
    private List<GameObject> allAddCategoriesUI = new List<GameObject>();

    private void Awake() {
        smallShapesButton.Select();
        allAddCategoriesUI.Add(largerShapes);
        allAddCategoriesUI.Add(smallerShapes);
        allAddCategoriesUI.Add(topPieces);
        allAddCategoriesUI.Add(decorations);
    }

    public void ShowTopPieces() {
        ToggleAllAddCategoryUIs(false);
        topPieces.SetActive(true);
    }
    public void ShowDecorationsPieces() {
        ToggleAllAddCategoryUIs(false);
        decorations.SetActive(true);
    }

    public void ChangeShapeSize(bool changetoBigBlobs) {
        largeShapes = changetoBigBlobs;
        if (removeModeActive) {
            removeModeActive = false;
            ChangeBuildModeButtonPressed?.Invoke(OperationType.ADD);
        }

        ToggleAllAddCategoryUIs(false);
        if (largeShapes) {
            largerShapes.SetActive(true);
        } 
        else {
            smallerShapes.SetActive(true);
        }
    }

    public void Undo() {
        UndoLastActionButtonPressed?.Invoke();
    }

    public void Redo() {
        RedoActionButtonPressed?.Invoke();
    }

    public void DeleteAll() {
        ResetBuildZoneButtonPressed?.Invoke();
    }

    public void ChangeSandBlobType(SandBlobTypeUI sandBlob) {
        ChangeSandBlobTypeButtonPressed?.Invoke(sandBlob.sandBlobType);
    }

    public void ChangeDecorationType(DecorationTypeUI decorationTypeUI) {
        ChangeDecorationTypeButtonPressed?.Invoke(decorationTypeUI.decoration);
    }

    public void ChangeMode() {
        removeModeActive = !removeModeActive; 

        if (removeModeActive) {
            disabledShapeSelection.SetActive(true);
            substractModeText.SetActive(false);
            buildModeText.SetActive(true);
            ChangeSandBlobTypeButtonPressed?.Invoke(SandBlobType.CUBE_1x1);
            ChangeBuildModeButtonPressed?.Invoke(OperationType.SUBTRACT);
        } 
        else {
            disabledShapeSelection.SetActive(false);
            substractModeText.SetActive(true);
            buildModeText.SetActive(false);
            ChangeBuildModeButtonPressed?.Invoke(OperationType.ADD);
        }
    }

    public void ToggleControlsPanel() {
        controlsPanelActive = !controlsPanelActive; 
        if (controlsPanelActive) {
            controlsPanel.SetActive(true);
            showControlsText.SetActive(false);
            hideControlsText.SetActive(true);
        } else {
            controlsPanel.SetActive(false);
            showControlsText.SetActive(true);
            hideControlsText.SetActive(false);
        }
    }

    public void ToggleUIPanel() {
        uiPanel.SetActive(!uiPanel.activeSelf);
    }

    private void ToggleAllAddCategoryUIs(bool on) {
        foreach (GameObject addCategoryUI in allAddCategoriesUI) {
            addCategoryUI.SetActive(on);
        }
    }
}
