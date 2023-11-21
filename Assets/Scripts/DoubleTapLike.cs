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

    public GameObject likeGameObject; // Assign the like GameObject in the inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - lastTapTime) < tapTimeThreshold)
        {
            HandleLike(eventData.position);
        }
        lastTapTime = Time.time;
    }

    private void HandleLike(Vector2 tapPosition)
    {
        Debug.Log("Liked!");

        // Convert tap position to world position (for UI, use RectTransformUtility)
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(tapPosition);

        // Move the like GameObject to the tap position and activate it
        likeGameObject.transform.position = worldPosition;
        likeGameObject.SetActive(true);

        // Start the animation coroutine
        StartCoroutine(LikeAnimation());
    }

    private IEnumerator LikeAnimation()
    {
        StartCoroutine(ButtonLikeAnimation());

        // Enlarge the like GameObject
        likeGameObject.transform.localScale = Vector3.one * 1.2f;

        // Wait for a short time
        yield return new WaitForSeconds(0.2f);

        // Return to the original scale
        likeGameObject.transform.localScale = Vector3.one;

        // Wait for a short time
        yield return new WaitForSeconds(0.2f);

        // Deactivate the like GameObject
        likeGameObject.SetActive(false);
    }

    public void LikeButtonClicked()
    {
        // Called from UI Button OnClick event
        HandleLikeButton();
    }

    private void HandleLikeButton()
    {
        Debug.Log("Liked!");
        //if ((Time.time - lastTapTime) < tapTimeThreshold)
        //{
            // Let the VideoManager know that the current video has been liked
            videoManager.LikeCurrentVideo();

            // Perform the like UI animation
            StartCoroutine(ButtonLikeAnimation());
        //}
        // Update the last tap time
        lastTapTime = Time.time;
    }

    private IEnumerator ButtonLikeAnimation()
    {
        // Enlarge the like button UI
        likeButtonImage.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        // Wait for a very short time
        yield return new WaitForSeconds(0.1f);

        // Return to the original scale and change sprite to indicate it's liked
        likeButtonImage.transform.localScale = Vector3.one;
        likeButtonImage.sprite = likedSprite;
    }
}
