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

    // Initialization
    void Start()
    {
        likeCounts = new Dictionary<VideoCategory, int>{
            {VideoCategory.A, 0},
            {VideoCategory.B, 0},
            {VideoCategory.C, 0}
        };
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

    // Call this to update the display with the next placeholder.
    public void DisplayNextPlaceholder()
    {
        Debug.Log("Swiped!");
        Texture nextPlaceholder = GetNextVideoPlaceholder();
        displayImage.texture = nextPlaceholder; // Set the RawImage texture to the new placeholder.
    }
}