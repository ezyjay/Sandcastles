using System;
using UnityEngine;

public class SandcastleInputManager : MonoBehaviour
{
    public float mouseScrollSpeed = 100f;
    public KeyCode resetBuildZone, toggleUI, undo, redo, toggleRemove;

    public Action MouseClicked;
    public Action<float> MouseScrolled;
    public Action ResetBuildZoneKeyPressed, UndoLastActionKeyPressed, RedoActionKeyPressed, ToggleUIKeyPressed, ToggleRemoveModeKeyPressed;

    public void CheckInput()
    {
        //Detect mouse click
        if (Input.GetMouseButtonDown(0))
            MouseClicked?.Invoke();

        //Mouse scroll
        else if (Input.mouseScrollDelta.y != 0f) {
            MouseScrolled?.Invoke(Input.GetAxis("Mouse ScrollWheel") * mouseScrollSpeed * 10);
        }

        //Reset build zone
        if (Input.GetKeyDown(resetBuildZone)) {
            ResetBuildZoneKeyPressed?.Invoke();
        }
        //Toggle UI
        else if (Input.GetKeyDown(toggleUI)) {
            ToggleUIKeyPressed?.Invoke();
        } 
        //Undo
        else if (Input.GetKeyDown(undo)) {
            UndoLastActionKeyPressed?.Invoke();
        } 
        //Redo
        else if (Input.GetKeyDown(redo)) {
            RedoActionKeyPressed?.Invoke();
        }
        //Toggle remove
        else if (Input.GetKeyDown(toggleRemove)) {
            ToggleRemoveModeKeyPressed?.Invoke();
        }
    }
}
