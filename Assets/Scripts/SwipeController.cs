using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float dragThreshold = 30.0f; // Minimum distance for a successful swipe
    public VideoManager videoManager; // Reference to your VideoManager

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Record start position when touch begins
        startTouchPosition = eventData.position;
        Debug.Log("Start touch position: " + startTouchPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Record end position when touch ends
        endTouchPosition = eventData.position;
        Debug.Log("End touch position: " + endTouchPosition);
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(startTouchPosition, endTouchPosition) >= dragThreshold)
        {
            Vector3 direction = endTouchPosition - startTouchPosition;
            Vector2 swipeType = Vector2.zero;

            // Check if the swipe is vertical or horizontal
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // The swipe is horizontal
                swipeType = Vector2.right * Mathf.Sign(direction.x);
            }
            else
            {
                // The swipe is vertical (if needed)
                swipeType = Vector2.up * Mathf.Sign(direction.y);
            }

            // Swipe right
            if (swipeType.x > 0)
            {
                // Show next video/placeholder to the left
                videoManager.DisplayNextPlaceholder();
            }
            // Swipe left
            else if (swipeType.x < 0)
            {
                // Show next video/placeholder to the right
                videoManager.DisplayNextPlaceholder();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Simulate swipe right");
            videoManager.DisplayNextPlaceholder();
        }
    }
}