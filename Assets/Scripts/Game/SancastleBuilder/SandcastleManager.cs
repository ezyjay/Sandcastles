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
        inputManager.MouseUp += OnMouseUp;
        inputManager.MouseScrolled += OnMouseScrolled;
        inputManager.MouseDragged += OnMouseDragged;
        inputManager.MouseDown += OnMouseDown;
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
        inputManager.MouseUp += OnMouseUp;
        inputManager.MouseScrolled += OnMouseScrolled;
        inputManager.MouseDragged -= OnMouseDragged;
        inputManager.MouseDown -= OnMouseDown;
        inputManager.ToggleUIKeyPressed -= OnToggleUIKeyPressed;
        inputManager.ToggleRemoveModeKeyPressed -= OnToggleRemoveModeKeyPressed;
    }

    private void Update() {
        inputManager.CheckInput();
    }

    private void OnMouseScrolled(float scrollSpeed) {
        buildManager.RotateObjectWithScroll(scrollSpeed);
    }

    private void OnMouseUp() {
        buildManager.AddObjectOnMouseUp();
    }

    private void OnMouseDragged(Vector3 startPosition, Vector3 currentMousePosition) {
        buildManager.OnMouseDragged(startPosition, currentMousePosition);
    }

    private void OnMouseDown(Vector3 position) {
        buildManager.OnMouseDown(position);
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
