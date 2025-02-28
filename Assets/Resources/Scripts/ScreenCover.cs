using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCover : MonoBehaviour
{
    public static ScreenCover instance;
    float timer;
    public float animationTime;
    float cameraDefault;
    public float cameraViewPercent;
    bool inAnimation;
    Image screenCover;
    bool active = false;
    void Awake()
    {
        cameraDefault = Camera.main.orthographicSize;
        screenCover = GetComponent<Image>();

        instance = this;
    }

    void Update()
    {
        // Debug remove when unneeded

        // Animation
        if (inAnimation){
            // Find if the animation is starting or ending
            if (!active) timer += Time.unscaledDeltaTime;
            else         timer -= Time.unscaledDeltaTime;
            
            // Augment visuals
            UpdateValues();

            // Stop animation if done
            if (timer > animationTime || timer < 0){
                TurnAnimationOff();
            }
        }
    }
    public void TurnAnimationOn(){
        // Start animation
        inAnimation = true;
    }

    public void InstantSetToBlack(){
        timer = animationTime;
        active = true;
        UpdateValues();
    }

    public void TurnAnimationOff(){
        // Stop animation
        inAnimation = false;
        active = !active;
        
        if (active) timer = animationTime;
        else timer = 0;

        // Update visuals to fix small issues related to fps
        UpdateValues();
    }

    public void UpdateValues(){
        screenCover.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer/animationTime));
        if (Camera.main.orthographic)
            Camera.main.orthographicSize = Mathf.Lerp(cameraDefault, cameraDefault * cameraViewPercent, Mathf.Pow(timer/animationTime, 2));
    }
}
