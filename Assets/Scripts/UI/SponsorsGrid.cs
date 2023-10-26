using UnityEngine.UI;
using UnityEngine;
public class SponsorsGrid : MonoBehaviour
{
    Vector2 currentResolution, resolutionHD, resolutionWXGA, resolutionQHD, resolution4KUHD, cellSize, spacing;
    GridLayoutGroup gridLayoutGroup;

    void Awake()
    {
        resolutionHD = new Vector2(1920, 1080);
        resolutionWXGA = new Vector2(1366, 768);
        resolutionQHD = new Vector2(2560, 1440);
        resolution4KUHD = new Vector2(3840, 2160);
        
        // Get references to components
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        currentResolution = new Vector2(Screen.width, Screen.height);
        // Update cell size and spacing based on current screen resolution
        UpdateCellSizeAndSpacing();
    }

    void UpdateCellSizeAndSpacing()
    {
        cellSize = new Vector2(currentResolution.x * 0.055f, currentResolution.x * 0.055f);

        if (currentResolution == resolutionHD)
        {
            spacing = new Vector2(5f, 5f);
            gridLayoutGroup.padding.right = 0;
            gridLayoutGroup.padding.bottom = 0;
        }
        else if (currentResolution == resolutionWXGA)
        {
            spacing = new Vector2(25f, 25f);
            gridLayoutGroup.padding.right = 10;
            gridLayoutGroup.padding.bottom = 10;
        }
        else if (currentResolution == resolutionQHD)
        {
            spacing = new Vector2(-40f, -40f);
            gridLayoutGroup.padding.right = -25;
            gridLayoutGroup.padding.bottom = -25;
        }
        else if (currentResolution == resolution4KUHD)
        {
            spacing = new Vector2(-115f, -115f);
            gridLayoutGroup.padding.right = -55;
            gridLayoutGroup.padding.bottom = -55;
        }
        // Set cell size and spacing
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spacing;
    }
}
