using UnityEngine;
using UnityEngine.InputSystem;
public class KeyboardPaddleInput : MonoBehaviour
{
    private Paddle targetPaddle;

    private Key upKey;
    private Key downKey;

    private bool isInputEnabled;
    private float currentInput;

    public void Init(Paddle paddle, Key up, Key down)
    {
        targetPaddle = paddle;
        upKey = up;
        downKey = down;
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
    }

    private void Update()
    {
        currentInput = 0f;

        if (!isInputEnabled)
            return;

        if (targetPaddle == null)
            return;

        if (Keyboard.current[upKey].isPressed)
            currentInput += 1f;

        if (Keyboard.current[downKey].isPressed)
            currentInput -= 1f;

        currentInput = Mathf.Clamp(currentInput, -1f, 1f);
    }

    private void FixedUpdate()
    {
        if (!isInputEnabled)
            return;

        if (targetPaddle == null)
            return;

        targetPaddle.Move(currentInput);
    }
}
