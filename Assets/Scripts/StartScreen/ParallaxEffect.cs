using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float offsetMultiplier = 25f;
    private float smoothTime = 0.3f;

    private Vector3 startPosition;
    private Vector3 velocity;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float centerX = mousePos.x - 0.5f;
        float centerY = mousePos.y - 0.5f;

        Vector3 targetPos = new Vector3(
            startPosition.x + (centerX * offsetMultiplier),
            startPosition.y + (centerY * offsetMultiplier),
            startPosition.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPos, 
            ref velocity, 
            smoothTime
        );
    }
}