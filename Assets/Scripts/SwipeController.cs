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

            if (currentSwipe.magnitude > minSwipeLength)
            {
                if (Mathf.Abs(currentSwipe.y) > Mathf.Abs(currentSwipe.x))
                {
                    if (currentSwipe.y > 0)
                    {
                        Debug.Log("Swipe Up");
                        videoManager.DisplayNextPlaceholder();
                    }
                    else if (currentSwipe.y < 0)
                    {
                        Debug.Log("Swipe Down");
                        videoManager.DisplayPreviousPlaceholder();
                    }
                }
            }
        }
    }
}
