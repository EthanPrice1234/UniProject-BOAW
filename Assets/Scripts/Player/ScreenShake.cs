using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake instance;
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            // Using random unit sphere to position the camera like its shaking
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeIntensity;
            shakeDuration -= Time.deltaTime;
            shakeIntensity -= Time.deltaTime / 10;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public static void Shake(float duration, float intensity)
    {
        if (instance != null)
        {
            instance.shakeDuration = duration;
            instance.shakeIntensity = intensity;
        }
    }
} 