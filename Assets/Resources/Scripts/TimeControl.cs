using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeControl : MonoBehaviour, ISavable
{
    public static TimeControl instance;
    
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
    public Gradient lightColor;
    public Gradient skyColor;

    [Header("Date")]
    public int day;
    public float time;
    [Header("Time Bar")]
    public RectTransform timeBar;
    public TextMeshProUGUI dateText;
    [Header("Sun")]
    public GameObject sun;
    public Light sunLight;
    float lastTime;

    void Awake(){
        instance = this;
    }
    private void Update() {
        if (MapLoader.instance != null && MapLoader.instance.loadingEditor) return;

        time += Time.deltaTime;
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

        UpdateVisuals();
    }

    public void UpdateVisuals(){
        float barWidth = Mathf.Lerp(0, 1, time/DataBase.instance.dayLenghtInSeconds);
        timeBar.localScale = new Vector3(barWidth, 1, 1);

        if (time > DataBase.instance.dayLenghtInSeconds){
            day++;
            time = 0;
            dateText.text = "Day: " + day.ToString();
        }

        sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(0, 360, time/DataBase.instance.dayLenghtInSeconds) - 180, 0, 0);

        sunLight.color = lightColor.Evaluate(time/DataBase.instance.dayLenghtInSeconds);
        Camera.main.backgroundColor = skyColor.Evaluate(time/DataBase.instance.dayLenghtInSeconds);
    }

    public void SpeedUp(){
        if (DecisionMenu.instance.currentEvent != null) return;

        Time.timeScale = fastTime;
        lastTime = fastTime;
        
        pauseButton.color = unselectedColor;
        slowButton.color = unselectedColor;
        normalButton.color = unselectedColor;
        fastButton.color = selectedColor;
    }

    public void NormalSpeed(){
        if (DecisionMenu.instance.currentEvent != null) return;

        Time.timeScale = normalTime;
        lastTime = normalTime;

        pauseButton.color = unselectedColor;
        slowButton.color = unselectedColor;
        normalButton.color = selectedColor;
        fastButton.color = unselectedColor;

    }

    public void SlowDown(){
        if (DecisionMenu.instance.currentEvent != null) return;
        
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
        if (DecisionMenu.instance.currentEvent != null) return;
        
        if(Time.timeScale == 1){
            NormalSpeed();
        }else if(Time.timeScale == 2){
            SpeedUp();
        }else{
            SlowDown();
        }
    }

    public int GetPriority()
    {
        return 0;
    }

    public void LoadData(GameData data)
    {
        lastTime = data.time.speed;
        day = data.time.day;
        time = data.time.time;

        if (MapLoader.instance != null && MapLoader.instance.loadingEditor) time = DataBase.instance.dayLenghtInSeconds/2;

        dateText.text = "Day: " + day.ToString();

        UpdateVisuals();
        Time.timeScale = 1;
        PauseTime();
    }

    public void SaveData(GameData data)
    {
        data.time = new(this);
    }
}
