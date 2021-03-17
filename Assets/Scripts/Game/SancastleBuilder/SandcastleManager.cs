using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandcastleManager : MonoBehaviour
{
    public SandcastleBuildManager buildManager;
    public SandcastleUI sandcastleUI;
    public SandcastleInputManager inputManager;

    private void OnEnable() {
        sandcastleUI.ChangeSandBlobTypeButtonPressed += OnChangeSandBlobTypeButtonPressed;
        sandcastleUI.ChangeBuildModeButtonPressed += OnChangeBuildModeButtonPressed;
        sandcastleUI.ChangeDecorationTypeButtonPressed += OnChangeDecorationTypeButtonPressed;
        sandcastleUI.ResetBuildZoneButtonPressed += OnResetBuildZonePressed;
        sandcastleUI.RedoActionButtonPressed += OnRedoActionPressed;
        sandcastleUI.UndoLastActionButtonPressed += OnUndoLastActionPressed;

        inputManager.ResetBuildZoneKeyPressed += OnResetBuildZonePressed;
        inputManager.UndoLastActionKeyPressed += OnUndoLastActionPressed;
        inputManager.RedoActionKeyPressed += OnRedoActionPressed;
        inputManager.MouseClicked += OnMouseClicked;
        inputManager.MouseScrolled += OnMouseScrolled;
        inputManager.ToggleUIKeyPressed += OnToggleUIKeyPressed;
        inputManager.ToggleRemoveModeKeyPressed += OnToggleRemoveModeKeyPressed;
    }

    private void OnDisable() {
        sandcastleUI.ChangeSandBlobTypeButtonPressed -= OnChangeSandBlobTypeButtonPressed;
        sandcastleUI.ChangeBuildModeButtonPressed -= OnChangeBuildModeButtonPressed;
        sandcastleUI.ChangeDecorationTypeButtonPressed -= OnChangeDecorationTypeButtonPressed;
        sandcastleUI.ResetBuildZoneButtonPressed -= OnResetBuildZonePressed;
        sandcastleUI.RedoActionButtonPressed -= OnRedoActionPressed;
        sandcastleUI.UndoLastActionButtonPressed -= OnUndoLastActionPressed;

        inputManager.ResetBuildZoneKeyPressed -= OnResetBuildZonePressed;
        inputManager.UndoLastActionKeyPressed -= OnUndoLastActionPressed;
        inputManager.RedoActionKeyPressed -= OnRedoActionPressed;
        inputManager.MouseClicked += OnMouseClicked;
        inputManager.MouseScrolled += OnMouseScrolled;
        inputManager.ToggleUIKeyPressed -= OnToggleUIKeyPressed;
        inputManager.ToggleRemoveModeKeyPressed -= OnToggleRemoveModeKeyPressed;
    }

    private void Update() {
        inputManager.CheckInput();
    }

    private void OnMouseScrolled(float scrollSpeed) {
        buildManager.RotateObjectWithScroll(scrollSpeed);
    }

    private void OnMouseClicked() {
        buildManager.AddObjectOnMouseClick();
    }

    private void OnRedoActionPressed() {
        buildManager.RedoAction();
    }

    private void OnUndoLastActionPressed() {
        buildManager.UndoLastAction();
    }

    private void OnResetBuildZonePressed() {
        buildManager.ResetBuildZone();
    }

    private void OnChangeDecorationTypeButtonPressed(SandDecorationType decorationType) {
        buildManager.ChangeDecorationType(decorationType);
    }

    private void OnChangeBuildModeButtonPressed(OperationType operationType) {
        buildManager.ChangeBuildMode(operationType);
    }

    private void OnChangeSandBlobTypeButtonPressed(SandBlobType sandBlobType) {
        buildManager.ChangeSandBlobType(sandBlobType);
    }

    private void OnToggleRemoveModeKeyPressed() {
        sandcastleUI.ChangeMode();
    }

    private void OnToggleUIKeyPressed() {
        sandcastleUI.ToggleUIPanel();
    }
}
