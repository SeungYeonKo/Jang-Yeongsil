using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : Patterns.Singleton<GameApp>
{
  public bool TileMovementEnabled { get; set; } = false;
  public double SecondsSinceStart { get; set; } = 0;
  public int TotalTilesInCorrectPosition { get; set; } = 0;

    [SerializeField]
    List<string> easyModeImages = new List<string>();
    [SerializeField]
    List<string> normalModeImages = new List<string>(); 
    [SerializeField]
    List<string> hardModeImages = new List<string>();


    private List<string> currentModeImages = new List<string>();
    private string currentMode = "Normal";
    int imageIndex = 0;

    private void Start()
    {
        SetMode("Normal");
    }
    public void SetMode(string mode)
    {
        currentMode = mode;
        Debug.Log("현재 모드: " + mode);
        switch (mode)
        {
            case "Easy":
                currentModeImages = new List<string>(easyModeImages);
                break;
            case "Normal":
                currentModeImages = new List<string>(normalModeImages);
                break;
            case "Hard":
                currentModeImages = new List<string>(hardModeImages);
                break;
        }

        imageIndex = 0;
    }
    public string GetCurrentMode()
    {
        return currentMode;
    }

    public string GetJigsawImageName()
    {
        if (currentModeImages == null || currentModeImages.Count == 0)
        {
            Debug.LogError("No images available for the selected mode.");
            return null;
        }
        
        string imageName = currentModeImages[imageIndex++];
        if(imageIndex == currentModeImages.Count)
        {
            imageIndex = 0;
        }
        return imageName;
    }
}
