using UnityEngine;
using UnityEngine.UI;

public class TimeControl : MonoBehaviour
{
    [Header("Time settings")]
    public float fastTime = 3;
    public float normalTime = 2;
    public float slowTime = 1;

    [Header("Buttons")]
    public Image pauseButton;
    public Image slowButton;
    public Image normalButton;
    public Image fastButton;

    [Header("Colors")]
    public Color unselectedColor;
    public Color selectedColor;

    float lastTime;
    
    private void Start() {
        NormalSpeed();
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.Tab)){
            NextTime();
        }

        if(Input.GetKeyUp(KeyCode.Space)){
            if(Time.timeScale == 0){
                if(lastTime == 1){
                    SlowDown();
                }else if(lastTime == 2){
                    NormalSpeed();
                }else{
                    SpeedUp();
                }
            }else{
                PauseTime();
            }
        }
    }

    public void SpeedUp(){
        Time.timeScale = fastTime;
        lastTime = fastTime;
        
        pauseButton.color = unselectedColor;
        slowButton.color = unselectedColor;
        normalButton.color = unselectedColor;
        fastButton.color = selectedColor;
    }

    public void NormalSpeed(){
        Time.timeScale = normalTime;
        lastTime = normalTime;

        pauseButton.color = unselectedColor;
        slowButton.color = unselectedColor;
        normalButton.color = selectedColor;
        fastButton.color = unselectedColor;

    }

    public void SlowDown(){
        Time.timeScale = slowTime;
        lastTime = slowTime;

        pauseButton.color = unselectedColor;
        slowButton.color = selectedColor;
        normalButton.color = unselectedColor;
        fastButton.color = unselectedColor;
    }

    public void PauseTime(){
        Time.timeScale = 0;

        pauseButton.color = selectedColor;
        slowButton.color = unselectedColor;
        normalButton.color = unselectedColor;
        fastButton.color = unselectedColor;
    }

    public void NextTime(){
        if(Time.timeScale == 1){
            NormalSpeed();
        }else if(Time.timeScale == 2){
            SpeedUp();
        }else{
            SlowDown();
        }
    }
}
