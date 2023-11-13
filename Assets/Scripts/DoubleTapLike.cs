using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DoubleTapLike : MonoBehaviour, IPointerClickHandler
{
    public VideoManager videoManager;
    public Image likeButtonImage; // Assign in the inspector
    public Sprite likeSprite; // Assign in the inspector
    public Sprite likedSprite; // Assign in the inspector

    private float lastTapTime = 0f;
    private float tapTimeThreshold = 0.4f; // Time in seconds for double tap

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked!");
        // If the time since the last tap is less than the threshold, we consider it a double-tap
        if ((Time.time - lastTapTime) < tapTimeThreshold)
        {
            // Detected double tap
            HandleLike();
        }
        // Update the last tap time
        lastTapTime = Time.time;
    }

    public void LikeButtonClicked()
    {
        // Called from UI Button OnClick event
        HandleLike();
    }

    private void HandleLike()
    {
        Debug.Log("Liked!");
        //if ((Time.time - lastTapTime) < tapTimeThreshold)
        //{
            // Let the VideoManager know that the current video has been liked
            videoManager.LikeCurrentVideo();

            // Perform the like UI animation
            StartCoroutine(LikeAnimation());
        //}
        // Update the last tap time
        lastTapTime = Time.time;
    }

    private IEnumerator LikeAnimation()
    {
        // Enlarge the like button UI
        likeButtonImage.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        // Wait for a very short time
        yield return new WaitForSeconds(0.2f);

        // Return to the original scale and change sprite to indicate it's liked
        likeButtonImage.transform.localScale = Vector3.one;
        likeButtonImage.sprite = likedSprite;
    }
}
