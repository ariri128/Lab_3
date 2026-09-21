using UnityEngine;

// Moves the player ship left/right along the X axis only, clamped so it never leaves the camera's view
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;

    private float minX;
    private float maxX;

    private void Start()
    {
        CalculateScreenBounds();
    }

    // Converts the camera's left/right viewport edges into world-space X limits, inset by the sprite's own half-width
    private void CalculateScreenBounds()
    {
        float halfWidth = GetComponentInChildren<SpriteRenderer>().bounds.extents.x;
        float depth = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, depth));

        minX = leftEdge.x + halfWidth;
        maxX = rightEdge.x - halfWidth;
    }

    private void Update()
    {
        MoveHorizontally();
    }

    // Reads horizontal input, moves along Vector3.right only, then clamps the result within screen bounds
    private void MoveHorizontally()
    {
        float input = Input.GetAxisRaw("Horizontal");
        Vector3 nextPosition = transform.position + Vector3.right * input * moveSpeed * Time.deltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        transform.position = nextPosition;
    }
}