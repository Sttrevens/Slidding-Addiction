using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

[System.Serializable]
public struct VideoHistoryEntry
{
    public VideoEntry videoEntry;
    public int textureIndex;
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

    private VideoEntry currentVideoEntry;

    public CategoryUI categoryUI;
    private VideoCategory favoriteCategory;
    private float happinessValue;
    public HappinessBar happinessBar;
    public float maxHappinessValue = 10f;

    private float anxietyValue = 0f;
    private float maxAnxietyValue = 10f;
    private bool isMissFavoriteVideo = false;
    public AnxietyBar anxietyBar;

    private float elapsedTime;
    private float valueUpdateInterval = 1.0f;
    private float nextValueUpdateTime = 0;

    private Stack<VideoHistoryEntry> videoHistory = new Stack<VideoHistoryEntry>();
    private int currentTextureIndex = 0;

    public bool isOnTable = false;

    public FavoriteCategory favoriteCategoryClass;

    void Start()
    {
        likeCounts = new Dictionary<VideoCategory, int>{
            {VideoCategory.A, 0},
            {VideoCategory.B, 0},
            {VideoCategory.C, 0}
        };

        nextImageStartPosition = nextImageRectTransform.anchoredPosition;

        // Set the first video
        //DisplayNextPlaceholder();
        VideoEntry nextVideoEntry = GetNextVideoEntry();
        currentTextureIndex = UnityEngine.Random.Range(0, nextVideoEntry.placeholders.Count);
        if (currentVideoEntry != null)
        {
            // Store both video entry and the index of the texture displayed
            videoHistory.Push(new VideoHistoryEntry
            {
                videoEntry = currentVideoEntry,
                textureIndex = currentTextureIndex
            });
        }
        nextImageRectTransform.GetComponent<RawImage>().texture = nextVideoEntry.placeholders[currentTextureIndex];
        currentVideoEntry = nextVideoEntry;
        anxietyValue = Mathf.Clamp(anxietyValue, 0, maxAnxietyValue);
        happinessValue = Mathf.Clamp(happinessValue, 0, maxHappinessValue);

        favoriteCategory = favoriteCategoryClass.favoriteCategory;
        Debug.Log("Actual favorite:" + favoriteCategory);
    }

    void Update()
    {
        if (!isOnTable)
        {
            elapsedTime += Time.deltaTime;

            // Check if it's time to update the value
            if (elapsedTime >= nextValueUpdateTime)
            {
                anxietyValue += 0.5f;
                UpdateAnxiety();
                nextValueUpdateTime += valueUpdateInterval; // Set the next update time
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;

            // Check if it's time to update the value
            if (elapsedTime >= nextValueUpdateTime)
            {
                anxietyValue -= 0.5f;
                UpdateAnxiety();
                nextValueUpdateTime += valueUpdateInterval; // Set the next update time
            }
        }

        if (anxietyValue > maxAnxietyValue)
        {
            anxietyValue = maxAnxietyValue;
        }
        else if (anxietyValue < 0)
        {
            anxietyValue = 0;
        }

        if (happinessValue > maxHappinessValue)
        {
            happinessValue = maxHappinessValue;
        }
        else if (happinessValue < 0)
        {
            happinessValue = 0;
        }
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

    public void LikeCurrentVideo()
    {
        likeCounts[GetCurrentVideoCategory()]++;
        // Additional logic to update UI or other elements here
        Debug.Log("Liked video in category: " + GetCurrentVideoCategory());

        if (GetCurrentVideoCategory() == favoriteCategory)
        {
            UpdateHappiness();
            isMissFavoriteVideo = false;
        }
        else
        {
            anxietyValue++;
            UpdateAnxiety();
        }
    }

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

        int randomIndex = UnityEngine.Random.Range(0, weightedList.Count);
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
        int randomIndex = UnityEngine.Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }

    public void DisplayNextPlaceholder()
    {
        VideoEntry nextVideoEntry = GetNextVideoEntry();
        currentTextureIndex = UnityEngine.Random.Range(0, nextVideoEntry.placeholders.Count);

        if (currentVideoEntry != null)
        {
            // Store both video entry and the index of the texture displayed
            videoHistory.Push(new VideoHistoryEntry
            {
                videoEntry = currentVideoEntry,
                textureIndex = currentTextureIndex
        });
        }

        nextImageRectTransform.GetComponent<RawImage>().texture = nextVideoEntry.placeholders[currentTextureIndex];
        StartCoroutine(SwipeTransition());

        currentVideoEntry = nextVideoEntry;
        // Store the texture index of the next video entry
        // ...
    }

    public void DisplayPreviousPlaceholder()
    {
        if (videoHistory.Count > 0)
        {
            VideoHistoryEntry previousEntry = videoHistory.Pop();
            currentImageRectTransform.GetComponent<RawImage>().texture = previousEntry.videoEntry.placeholders[previousEntry.textureIndex];

            StartCoroutine(SwipeTransitionDown());
            currentVideoEntry = previousEntry.videoEntry;
            // Store the texture index of the previous video entry
            // ...
        }
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

        // Swap references so next becomes current
        var tempRectTransform = currentImageRectTransform;
        currentImageRectTransform = nextImageRectTransform;
        nextImageRectTransform = tempRectTransform;

        // Enable interaction on the new current image
        currentImageRectTransform.GetComponent<RawImage>().raycastTarget = true;

        // Reset like button sprite if necessary
        likeButtonImage.sprite = likeSprite;

        if (isMissFavoriteVideo == true)
        {
            anxietyValue++;
            UpdateAnxiety();
        }

        if (currentVideoEntry.category != favoriteCategory)
        {
            isMissFavoriteVideo = false;
        }
        else
        {
            isMissFavoriteVideo = true;
        }
    }

    IEnumerator SwipeTransitionDown()
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
        Vector2 nextImageStartPos = new Vector2(currentImageStartPos.x, currentImageStartPos.y + moveDistance);

        // Ending positions
        Vector2 currentImageEndPos = currentImageStartPos - new Vector2(0, moveDistance);
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

        // Swap references so next becomes current
        var tempRectTransform = currentImageRectTransform;
        currentImageRectTransform = nextImageRectTransform;
        nextImageRectTransform = tempRectTransform;

        // Enable interaction on the new current image
        currentImageRectTransform.GetComponent<RawImage>().raycastTarget = true;

        likeButtonImage.sprite = likeSprite;
    }


    void UpdateHappiness()
    {
        happinessValue = (float)likeCounts[favoriteCategory];
        happinessBar.SetHealth(happinessValue, maxHappinessValue);
    }

    void UpdateAnxiety()
    {
        anxietyBar.SetHealth(anxietyValue, maxAnxietyValue);
    }
}