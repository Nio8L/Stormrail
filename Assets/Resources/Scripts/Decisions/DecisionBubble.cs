using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionBubble : MonoBehaviour
{
    GameObject objectToLookAt;
    public City linkedCity;
    public SpriteRenderer eventIcon;
    public Decision decision;
    public Image circleTimer;
    public float timeLeft = 30f;

    void Start(){
        objectToLookAt = Camera.main.gameObject;
    }
    void Update(){
        transform.LookAt(objectToLookAt.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z+180f);

        timeLeft -= Time.deltaTime;

        float timerAngle = Mathf.Lerp(0, 1, timeLeft/30f);
        circleTimer.fillAmount = timerAngle;

        if (timeLeft <= 0){
            Destroy(gameObject);
            decision.options[0].TakeOption(linkedCity);
        }
    }
}
