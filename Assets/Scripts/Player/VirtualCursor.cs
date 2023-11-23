using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using System;
using System.Collections;


using UnityEngine.InputSystem.UI;

public class VirtualCursor : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] public RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Camera myCamera;
    [SerializeField] private float padding = 80f;

    private bool previousMouseState;
    public Mouse virtualMouse;
    public int playerNumber = -1;
    Vector2 newPosition;
    //Vector2 cursorInput;
    public int thisDeviceId;
    public bool stopRecordingInput = false;
    Gamepad myGamepad;

    private void OnEnable() {
        if (playerInput.user.controlScheme.ToString() != "Keyboard(<Keyboard>,<Mouse>)") {
            if (virtualMouse == null) { 
                virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
            }
            else if (!virtualMouse.added){
                InputSystem.AddDevice(virtualMouse);
            }

            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

            if (cursorTransform != null) {
                Vector2 position = cursorTransform.anchoredPosition;
                InputState.Change(virtualMouse.position, position);
            }

            InputSystem.onAfterUpdate += UpdateMotion;
            AnchorCursor(transform.position);
        } else {
            cursorTransform.gameObject.SetActive(false);
        }

        

    }

    public void SetMyGamepad(){
        foreach (Gamepad gpd in Gamepad.all){
            if (gpd.deviceId == thisDeviceId) {
                myGamepad = gpd;
            }
        }
    }

    private void OnDisable() {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void UpdateMotion() {
        if (virtualMouse == null || myGamepad == null || stopRecordingInput){
            return;
        }

        Vector2 deltaValue = myGamepad.leftStick.ReadValue();
        //Vector2 deltaValue = cursorInput;
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        newPosition = currentPosition + deltaValue;

        //newPosition.x = Mathf.Clamp(newPosition.x, padding, (myCamera.pixelWidth) - padding);
        //newPosition.y = Mathf.Clamp(newPosition.y, padding, (myCamera.pixelHeight) - padding);

        GetPositions();

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        /*bool aButtonIsPressed = myGamepad.aButton.IsPressed();
        if (previousMouseState != aButtonIsPressed){
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, myGamepad.aButton.IsPressed());
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIsPressed;
        }*/

        AnchorCursor(newPosition);
    }

    private void GetPositions () {
        switch(playerNumber) 
        {
        case 0:
            newPosition.x = Mathf.Clamp(newPosition.x, padding, (myCamera.pixelWidth) - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, (myCamera.pixelHeight) + padding, (myCamera.pixelHeight * 2f) - padding);
            break;
        case 1:
            newPosition.x = Mathf.Clamp(newPosition.x, (myCamera.pixelWidth) + padding, (myCamera.pixelWidth * 2f) - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, (myCamera.pixelHeight) + padding, (myCamera.pixelHeight * 2f) - padding);
            break;
        case 2:
            newPosition.x = Mathf.Clamp(newPosition.x, padding, (myCamera.pixelWidth) - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, padding, (myCamera.pixelHeight) - padding);
            break;
        case 3:
            newPosition.x = Mathf.Clamp(newPosition.x, (myCamera.pixelWidth) + padding, (myCamera.pixelWidth * 2f) - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, padding, (myCamera.pixelHeight) - padding);
            break;
        default:
            // code block
            break;
        }
    }

    private void AnchorCursor(Vector2 position){
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : myCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;

    }
}
