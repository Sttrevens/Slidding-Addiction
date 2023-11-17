using UnityEngine;
using UnityEngine.UI;

public class CategoryUIImage : MonoBehaviour
{
    public VideoManager videoManager; // Assign in inspector
    public Sprite categoryASprite;    // Assign in inspector
    public Sprite categoryBSprite;    // Assign in inspector
    public Sprite categoryCSprite;    // Assign in inspector

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        UpdateImage();
    }

    public void UpdateImage()
    {
        switch (videoManager.favoriteCategory)
        {
            case VideoCategory.A:
                imageComponent.sprite = categoryASprite;
                break;
            case VideoCategory.B:
                imageComponent.sprite = categoryBSprite;
                break;
            case VideoCategory.C:
                imageComponent.sprite = categoryCSprite;
                break;
        }
    }
}