using System;
using UnityEngine;

/// <summary>
/// 키보드 입력을 이용하여 플레이어를 조종할 수 있는 클래스
/// </summary>
public class Controller : MonoBehaviour
{
    //오른쪽 이동 방향
    private const bool RightDirection = true;
    //왼쪽 이동 방향
    private const bool LeftDirection = false;

    //조종할 대상 플레이어
    public Player player;

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
        if (player != null)
        {
            if(rightInput.isPressed != leftInput.isPressed)
            {
                switch (rightInput.isPressed)
                {
                    case RightDirection:
                        player.MoveRight();
                        break;
                    case LeftDirection:
                        player.MoveLeft();
                        break;
                }
            }
            if(jumpInput.isPressed == true)
            {
                player.Jump();
            }
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