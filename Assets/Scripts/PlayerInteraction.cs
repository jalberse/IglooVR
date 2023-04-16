using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInteraction : MonoBehaviour
{
    //Creates an enum that will determine if we're using the right or left controller
    public enum ControllerType
    {
        RightHand,
        LeftHand
    }

    //Stores the target controller from the editor
    [SerializeField] ControllerType targetController;

    //References our Input Actions that we are using
    [SerializeField] InputActionAsset inputAction;

    // Used for transformation data.
    [SerializeField] GameObject rightHandGameObject;

    [SerializeField] float maxSpeed;

    // Typically Trigger; scalar.
    InputAction _activateActionValue;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("XRI " + targetController.ToString());
        _activateActionValue = inputAction.FindActionMap("XRI " + targetController.ToString() + " Interaction").FindAction("Activate Value");
        _activateActionValue.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = rightHandGameObject.transform.forward;
        var triggerPullValue = _activateActionValue.ReadValue<float>();
        if (triggerPullValue > 0.0f)
        {
            transform.position += direction * Time.deltaTime * triggerPullValue;
        }
    }
}
