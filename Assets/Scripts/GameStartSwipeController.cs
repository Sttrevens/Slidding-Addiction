using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartSwipeController : MonoBehaviour
{
    public RectTransform menuScreen;
    public RectTransform goalImageScreen;
    public RectTransform gameScreen;

    public GameObject gameStartScreen;
    public GameObject nextVideoPlaceholder;

    public float transitDuration = 0.5f;

    private int currentScreen = 0;

    public AnimationCurve swingCurve;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Use actual swipe detection here
        {
            SwipeDown();
        }

        if (currentScreen == 2)
        {
            // Start the game
            StartGame();
        }
    }

    void SwipeDown()
    {
        if (currentScreen == 0)
        {
            // First swipe - move the Menu screen down and reveal the Goal Image screen
            StartCoroutine(MovePanel(menuScreen, Screen.height));
            StartCoroutine(MovePanel(goalImageScreen, Screen.height));
            StartCoroutine(MovePanel(gameScreen, Screen.height));
            currentScreen++;
        }
        else if (currentScreen == 1)
        {
            // Second swipe - move the Goal Image screen down and reveal the Game screen
            StartCoroutine(MovePanel(menuScreen, Screen.height));
            StartCoroutine(MovePanel(goalImageScreen, Screen.height));
            StartCoroutine(MovePanel(gameScreen, Screen.height));
            currentScreen++;
        }
    }

    IEnumerator MovePanel(RectTransform panel, float newY)
    {
        float duration = transitDuration;
        float elapsed = 0;
        Vector2 startPosition = panel.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y + newY);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float curveValue = swingCurve.Evaluate(elapsed / duration);
            panel.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, curveValue);
            yield return null;
        }

        // Ensure the panel is exactly at the end position when done
        panel.anchoredPosition = endPosition;
    }

    void StartGame()
    {
        // Logic to start the game
        Debug.Log("Game Started");

        gameStartScreen.SetActive(true);
    }
}
