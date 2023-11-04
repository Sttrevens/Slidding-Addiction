using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public RectTransform currentImageRectTransform;
    public RectTransform nextImageRectTransform;
    public float swipeSpeed = 1f;
    public Image likeButtonImage; // Assign in the inspector
    public Sprite likeSprite; // Assign in the inspector
    public Sprite likedSprite; // Assign in the inspector

    private Vector2 nextImageStartPosition;
    private float lastTapTime = 0f;
    private float tapTimeThreshold = 0.2f; // Time in seconds for double tap
    private VideoCategory currentCategory;

    private VideoEntry currentVideoEntry; 

    void Start()
    {
        likeCounts = new Dictionary<VideoCategory, int>{
            {VideoCategory.A, 0},
            {VideoCategory.B, 0},
            {VideoCategory.C, 0}
        };

        nextImageStartPosition = nextImageRectTransform.anchoredPosition;

        // Set the first video
        DisplayNextPlaceholder();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 && Time.time - lastTapTime < tapTimeThreshold)
        {
            LikeCurrentVideo();
            //StartCoroutine(LikeAnimation());
        }
        lastTapTime = Time.time;
    }

    //public void LikeButtonClicked()
    //{
    //    LikeCurrentVideo();
    //    StartCoroutine(LikeAnimation());
    //}

    public void LikeCurrentVideo()
    {
        likeCounts[currentCategory]++;
        // Additional logic to update UI or other elements here
        Debug.Log("Liked video in category: " + currentCategory);
    }

    // Animation for the like button to scale up and then return to normal size
    //private IEnumerator LikeAnimation()
    //{
    //    likeButtonImage.sprite = likedSprite; // Change to the liked sprite immediately
    //    Vector3 originalScale = likeButtonImage.transform.localScale;
    //    Vector3 targetScale = originalScale * 1.2f;

    //    // Scale up
    //    float t = 0f;
    //    while (t < 0.1f)
    //    {
    //        likeButtonImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / 0.1f);
    //        t += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Scale down
    //    t = 0f;
    //    while (t < 0.1f)
    //    {
    //        likeButtonImage.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / 0.1f);
    //        t += Time.deltaTime;
    //        yield return null;
    //    }

    //    likeButtonImage.transform.localScale = originalScale; // Reset to original scale
    //}

    // Method to get the next video/placeholder.
    public Texture GetNextVideoPlaceholder()
    {
        List<Texture> weightedList = new List<Texture>();
        foreach (var entry in videoPool)
        {
            int likeValue = likeCounts[entry.category];
            int weight = Mathf.Max(1, 10 - likeValue);
            for (int i = 0; i < weight; i++)
            {
                weightedList.AddRange(entry.placeholders);
            }
        }

        int randomIndex = Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }
    
    private VideoEntry GetNextVideoEntry()
    {
        List<VideoEntry> weightedList = new List<VideoEntry>();
        foreach (var entry in videoPool)
        {
            int likeValue = likeCounts[entry.category];
            // The lower the likes, the higher the chance to appear
            int weight = Mathf.Max(1, 10 - likeValue);
            for (int i = 0; i < weight; i++)
            {
                weightedList.Add(entry);
            }
        }

        // Randomly select from the weighted list
        int randomIndex = Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }

    public void DisplayNextPlaceholder()
    {
        // Start the coroutine to change the texture after a delay
        StartCoroutine(ChangeTextureWithDelay());
    }

    IEnumerator ChangeTextureWithDelay()
    {
        // Get the next video entry
        currentVideoEntry = GetNextVideoEntry();

        StartCoroutine(SwipeTransition());

        // Wait for 0.5 seconds before changing the texture
        yield return new WaitForSeconds(0.8f / swipeSpeed);

        // Now set the texture to the current image (which is displayed on screen)
        currentImageRectTransform.GetComponent<RawImage>().texture = currentVideoEntry.placeholders[Random.Range(0, currentVideoEntry.placeholders.Count)];

        // Start the swipe transition animation
    }

    private VideoCategory GetCurrentVideoCategory()
    {
        // Simply return the category of the current video entry
        return currentVideoEntry.category;
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

        likeButtonImage.sprite = likeSprite;
    }
}