using System;
using UnityEngine;

public class SandcastleInputManager : MonoBehaviour
{
    public float mouseScrollSpeed = 100f;
    public float keyDownInvokeInterval = 1f;
    public KeyCode resetBuildZone, toggleUI, undo, redo, toggleRemove;

    public Action MouseUp;
    public Action<float> MouseScrolled;
    public Action<Vector3> MouseDown;
    public Action<Vector3, Vector3> MouseDragged;
    public Action ResetBuildZoneKeyPressed, UndoLastActionKeyPressed, RedoActionKeyPressed, ToggleUIKeyPressed, ToggleRemoveModeKeyPressed;

    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;
    private float keyPressedTime;

    public void CheckInput()
    {
        CheckMouseInputs();
        CheckKeyboardInputs();
    }

    private void CheckKeyboardInputs() {

        //Reset build zone
        if (Input.GetKeyDown(resetBuildZone)) {
            ResetBuildZoneKeyPressed?.Invoke();
        }
        //Toggle UI
        else if (Input.GetKeyDown(toggleUI)) {
            ToggleUIKeyPressed?.Invoke();
        }
        //Undo
        else if (Input.GetKey(undo)) {

            if (Input.GetKeyDown(undo) || Time.time - keyPressedTime >= keyDownInvokeInterval) {
                UndoLastActionKeyPressed?.Invoke();
                keyPressedTime = Time.time;
            }
        }
        //Redo
        else if (Input.GetKey(redo)) {

            if (Input.GetKeyDown(redo) || Time.time - keyPressedTime >= keyDownInvokeInterval) {
                RedoActionKeyPressed?.Invoke();
                keyPressedTime = Time.time;
            }
        }
        //Toggle remove
        else if (Input.GetKeyDown(toggleRemove)) {
            ToggleRemoveModeKeyPressed?.Invoke();
        }
    }

    private void CheckMouseInputs() {

        //Detect mouse click
        if (Input.GetMouseButtonDown(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                position = hit.point;
                MouseDown?.Invoke(position);
            }
        }

        //Mouse dragged
        if (Input.GetMouseButton(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                MouseDragged?.Invoke(position, hit.point);
        }

        //Detect mouse click
        if (Input.GetMouseButtonUp(0)) {
            MouseUp?.Invoke();
        }

        //Mouse scroll
        if (Input.mouseScrollDelta.y != 0f) {
            MouseScrolled?.Invoke(Input.GetAxis("Mouse ScrollWheel") * mouseScrollSpeed * 10);
        }
    }
}
