using UnityEngine;

public class AutoRotateScreen : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;   // voliteľné
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }
}