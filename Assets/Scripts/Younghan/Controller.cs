using System;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    [Serializable]
    private struct Input
    {
        [SerializeField]
        public KeyCode[] keyCodes;

        public bool isPressed
        {
            set;
            get;
        }
    }

    [SerializeField]
    private Input upInput;
    [SerializeField]
    private Input downInput;
    [SerializeField]
    private Input leftInput;
    [SerializeField]
    private Input rightInput;
    [SerializeField]
    private Input jumpInput;
    [SerializeField]
    private Input healInput;
    [SerializeField]
    private Input attack1Input;
    [SerializeField]
    private Input attack2Input;
    [SerializeField]
    private Input attack3Input;

    private void Update()
    {
        Set(ref upInput);
        Set(ref downInput);
        Set(ref leftInput);
        Set(ref rightInput);
        Set(ref jumpInput);
        Set(ref healInput);
        Set(ref attack1Input);
        Set(ref attack2Input);
        Set(ref attack3Input);
        if (_target != null)
        {
           
        }
        upInput.isPressed = false;
        downInput.isPressed = false;
        leftInput.isPressed = false;
        rightInput.isPressed = false;
        jumpInput.isPressed = false;
        healInput.isPressed = false;
        attack1Input.isPressed = false;
        attack2Input.isPressed = false;
        attack3Input.isPressed = false;
    }

    private void Set(ref Input input)
    {
        int length = input.keyCodes != null ? input.keyCodes.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKey(input.keyCodes[i]) == true)
            {
                input.isPressed = true;
                break;
            }
        }
    }
}