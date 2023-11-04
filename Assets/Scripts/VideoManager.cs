using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VideoCategory
{
    A,
    B,
    C
}

[System.Serializable]
public class VideoEntry
{
    public VideoCategory category;
    public List<Texture> placeholders;
}

public class VideoManager : MonoBehaviour
{
    public List<VideoEntry> videoPool;
    private Dictionary<VideoCategory, int> likeCounts;
    public RawImage displayImage;

    public RectTransform currentImageRectTransform; // Assign this in the inspector
    public RectTransform nextImageRectTransform; // Assign this in the inspector
    public float swipeSpeed = 1f;

    private Vector2 nextImageStartPosition;

    // Initialization
    void Start()
    {
        likeCounts = new Dictionary<VideoCategory, int>{
            {VideoCategory.A, 0},
            {VideoCategory.B, 0},
            {VideoCategory.C, 0}
        };

        nextImageStartPosition = nextImageRectTransform.anchoredPosition;
    }

    // Method to get the next video/placeholder.
    public Texture GetNextVideoPlaceholder()
    {
        // Example: Simple random selection weighted by inverse of like count
        List<Texture> weightedList = new List<Texture>();
        foreach (var entry in videoPool)
        {
            // The more likes, the less chance to appear
            int weight = Mathf.Max(1, 10 - likeCounts[entry.category]);
            for (int i = 0; i < weight; i++)
            {
                weightedList.AddRange(entry.placeholders);
            }
        }

        int randomIndex = Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }

    public void DisplayNextPlaceholder()
    {
        // Get the next texture
        Texture nextPlaceholder = GetNextVideoPlaceholder();

        // Set the texture to the next image (which is initially off-screen below)
        nextImageRectTransform.GetComponent<RawImage>().texture = nextPlaceholder;

        // Start the animation coroutine
        StartCoroutine(SwipeTransition());
    }

    IEnumerator SwipeTransition()
    {
        // Disable interaction during the transition
        currentImageRectTransform.GetComponent<RawImage>().raycastTarget = false;
        nextImageRectTransform.GetComponent<RawImage>().raycastTarget = false;

        float duration = 1f / swipeSpeed;
        float elapsed = 0f;

        // Calculate the exact move distance based on the current image's height
        float moveDistance = currentImageRectTransform.rect.height;

        // Starting positions
        Vector2 currentImageStartPos = currentImageRectTransform.anchoredPosition;
        Vector2 nextImageStartPos = new Vector2(currentImageStartPos.x, currentImageStartPos.y - moveDistance);

        // Ending positions
        Vector2 currentImageEndPos = currentImageStartPos + new Vector2(0, moveDistance);
        Vector2 nextImageEndPos = currentImageStartPos; // Next image moves to where the current image was

        while (elapsed < duration)
        {
            // Update elapsed time
            elapsed += Time.deltaTime;

            // Calculate the next position based on the elapsed time
            float normalizedTime = elapsed / duration;
            currentImageRectTransform.anchoredPosition = Vector2.Lerp(currentImageStartPos, currentImageEndPos, normalizedTime);
            nextImageRectTransform.anchoredPosition = Vector2.Lerp(nextImageStartPos, nextImageEndPos, normalizedTime);

            yield return null;
        }

        // After the transition, reposition and prepare for the next swipe
        currentImageRectTransform.anchoredPosition = currentImageEndPos;
        nextImageRectTransform.anchoredPosition = nextImageEndPos;

        // Move the old current image off-screen to be the next one
        currentImageRectTransform.GetComponent<RawImage>().texture = GetNextVideoPlaceholder();
        currentImageRectTransform.anchoredPosition = nextImageStartPos;

        // Swap references so next becomes current
        var tempRectTransform = currentImageRectTransform;
        currentImageRectTransform = nextImageRectTransform;
        nextImageRectTransform = tempRectTransform;

        // Enable interaction on the new current image
        currentImageRectTransform.GetComponent<RawImage>().raycastTarget = true;
    }
}