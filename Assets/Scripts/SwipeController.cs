using UnityEngine;

public class SwipeController : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 currentSwipe;
    private bool isSwiping = false;

    public float minSwipeLength = 30f;
    public VideoManager videoManager;

    void Update()
    {
        // Check for mouse input to start the swipe
        if (Input.GetMouseButtonDown(0))
        {
            // Record start position
            startTouchPosition = Input.mousePosition;
            isSwiping = true;
        }

        // Check for mouse input to end the swipe
        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            currentSwipe = (Vector2)Input.mousePosition - startTouchPosition;

            // Check if the swipe is long enough to be considered a swipe
            // Check if the swipe is long enough to be considered a swipe
            if (currentSwipe.magnitude > minSwipeLength)
            {
                // Check if the swipe is primarily vertical
                if (Mathf.Abs(currentSwipe.y) > Mathf.Abs(currentSwipe.x))
                {
                    // Check if the swipe is upwards
                    if (currentSwipe.y > 0)
                    {
                        // Swipe up detected
                        Debug.Log("Swipe Up");
                        videoManager.DisplayNextPlaceholder();
                    }
                }
            }
        }

        // Optional: Use arrow keys to simulate swipes (for testing in editor)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Simulate swipe up");
            videoManager.DisplayNextPlaceholder();
        }
    }
}
