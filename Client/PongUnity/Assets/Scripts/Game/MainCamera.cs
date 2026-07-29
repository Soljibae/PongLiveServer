using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{

    [SerializeField] Vector2 targetAspect = new Vector2(16f, 9f);

    private Camera targetCamera;

    private int previousWidth;
    private int previousHeight;

    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ApplyAspectRatio();
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.width != previousWidth ||
              Screen.height != previousHeight)
        {
            ApplyAspectRatio();
        }
    }

    private void ApplyAspectRatio()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        float targetRatio = targetAspect.x / targetAspect.y;
        float screenRatio = (float)Screen.width / Screen.height;

        float scaleHeight = screenRatio / targetRatio;

        if (scaleHeight < 1f)
        {
            // 화면이 16:9보다 좁음 → 위아래 여백
            targetCamera.rect = new Rect(
                0f,
                (1f - scaleHeight) * 0.5f,
                1f,
                scaleHeight
            );
        }
        else
        {
            // 화면이 16:9보다 넓음 → 좌우 여백
            float scaleWidth = 1f / scaleHeight;

            targetCamera.rect = new Rect(
                (1f - scaleWidth) * 0.5f,
                0f,
                scaleWidth,
                1f
            );
        }

        previousWidth = Screen.width;
        previousHeight = Screen.height;
    }
}
