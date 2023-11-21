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
        StartSwipe();
        EndSwipe();

        // Optional: Use arrow keys to simulate swipes (for testing in editor)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Simulate swipe up");
            videoManager.DisplayNextPlaceholder();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Simulate swipe down");
            videoManager.DisplayPreviousPlaceholder();
        }
    }

    void StartSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Record start position
            startTouchPosition = Input.mousePosition;
            isSwiping = true;
            Debug.Log("Start Position: " + startTouchPosition);
        }
    }

    void EndSwipe()
    {
        // Check for mouse input to end the swipe
        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 endTouchPosition = Input.mousePosition;
            Debug.Log("End Position: " + endTouchPosition);

            isSwiping = false;
            currentSwipe = endTouchPosition - startTouchPosition;
            Debug.Log("Current Swipe:" + currentSwipe);

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
                    // Check if the swipe is downwards
                    else if (currentSwipe.y < 0)
                    {
                        // Swipe down detected
                        Debug.Log("Swipe Down");
                        videoManager.DisplayPreviousPlaceholder(); // Implement this method in VideoManager
                    }
                }
            }
        }
    }
}
