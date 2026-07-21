using UnityEngine;
using UnityEngine.InputSystem;

public class MobileTouchPaddleInput : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [SerializeField] private float stopDistance = 0.08f;

    private Paddle targetPaddle;
    private bool isInputEnabled;
    private float currentInput;

    public void Init(Paddle paddle)
    {
        targetPaddle = paddle;
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

        if (!TryGetPointerScreenPosition(out Vector2 screenPosition))
            return;

        float distanceFromCamera = Mathf.Abs(targetCamera.transform.position.z - targetPaddle.transform.position.z);

        Vector3 worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));

        float yDifference = worldPosition.y - targetPaddle.transform.position.y;

        if (Mathf.Abs(yDifference) <= stopDistance)
        {
            currentInput = 0f;
        }
        else if (yDifference > 0f)
        {
            currentInput = 1f;
        }
        else
        {
            currentInput = -1f;
        }
    }

    private void FixedUpdate()
    {
        if (!isInputEnabled)
            return;

        if (targetPaddle == null)
            return;

        targetPaddle.Move(currentInput);
    }

    private bool TryGetPointerScreenPosition(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
            {
                screenPosition = touch.position.ReadValue();
                return true;
            }
        }

#if UNITY_EDITOR
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }
        }
#endif

        return false;
    }
}
