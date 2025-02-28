using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationGrowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float growTime = 0.1f;
    public float time;
    public float growScale = 1.1f;

    bool pointerOver;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
    }

    public void ScaleDown(){
        pointerOver = false;
        time = 0;
        transform.localScale = Vector3.one;
    }

    void Update(){
        if (pointerOver){
            if (time >= growTime){
                time = growTime;
            }else{
                time += Time.unscaledDeltaTime;
            }
        }else{
            if (time <= 0){
                time = 0;
            }else{
                time -= Time.unscaledDeltaTime;
            }
        }

        float scale = Mathf.Lerp(1, growScale, time/growTime);
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
