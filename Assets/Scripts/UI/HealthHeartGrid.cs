using UnityEngine.UI;
using UnityEngine;

public class HealthHeartGrid : MonoBehaviour
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

    // Update cell size and spacing based on current screen resolution
    void UpdateCellSizeAndSpacing()
    {
        cellSize = new Vector2(currentResolution.x * 0.0442f, currentResolution.x * 0.0442f);


        if (currentResolution == resolutionHD)
        {
            spacing = new Vector2(5f, 5f);
            gridLayoutGroup.padding.left = 0;
            gridLayoutGroup.padding.top = 0;
        }
        else if (currentResolution == resolutionWXGA)
        {
            spacing = new Vector2(25f, 25f);
            gridLayoutGroup.padding.left = 0;
            gridLayoutGroup.padding.top = 0;
        }
        else if (currentResolution == resolutionQHD)
        {
            spacing = new Vector2(-20f, -20f);
            gridLayoutGroup.padding.left = -20;
            gridLayoutGroup.padding.top = -20;
        }
        else if (currentResolution == resolution4KUHD)
        {
            spacing = new Vector2(-75f, -75f);
            gridLayoutGroup.padding.left = -50;
            gridLayoutGroup.padding.top = -50;
        }
        // Set cell size and spacing
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spacing;
    }
}
