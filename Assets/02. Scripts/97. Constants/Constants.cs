using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Constants
{
    // 애니메이터 파라미터명
    public static class AnimatorParams
    {
        public const string PlayerHP = "PlayerHP";
        public const string Speed = "Speed";
        public const string VerticalVelocity = "VerticalVelocity";
        public const string IsGrounded = "IsGrounded";
        public const string IsSliding = "IsSliding";
        public const string JumpTrigger = "Jump";
        public const string DoubleJumpTrigger = "DoubleJump";
    }

    // 입력 키
    public static class InputKeys
    {
        public const KeyCode Jump = KeyCode.Z;
        public const KeyCode Slide = KeyCode.X;
        public const KeyCode IncreaseSize = KeyCode.I;
        public const KeyCode ResetSize = KeyCode.O;
        public const KeyCode IncreaseSpeed = KeyCode.KeypadPlus;
        public const KeyCode DecreaseSpeed = KeyCode.KeypadMinus;
    }
}
