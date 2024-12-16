using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalAligner : MonoBehaviour
{
    int childCount = 0;
    List<int> conditionCounts = new();

    void Update()
    {
        if (transform.childCount != childCount) UpdateLayout();
        for(int i = 0; i < transform.childCount; i++){
            if(conditionCounts[i] != transform.GetChild(i).GetChild(transform.GetChild(i).childCount - 1).childCount){
                UpdateLayout();
            }
        }
    }

    void UpdateLayout(){
        conditionCounts.Clear();
        childCount = transform.childCount;
        float verticalOffset = -100;
        GameObject child, contentHolder, lastElement;
        float elementHeight, heightestPoint, lowestPoint;
        for (int i = 0; i < transform.childCount; i++){
            child = transform.GetChild(i).gameObject;
            RectTransform childRect = child.GetComponent<RectTransform>();
            child.transform.localPosition = new Vector3(childRect.rect.width/2, verticalOffset, 0);
            heightestPoint = child.transform.localPosition.y+childRect.rect.height/2;
            contentHolder = child.transform.GetChild(child.transform.childCount-1).gameObject;
            lowestPoint = heightestPoint - childRect.rect.height;
            if (contentHolder.transform.childCount != 0){
                lastElement = contentHolder.transform.GetChild(contentHolder.transform.childCount-1).gameObject;
                lowestPoint -= lastElement.GetComponent<RectTransform>().rect.height * contentHolder.transform.childCount;
            }
            elementHeight = lowestPoint - heightestPoint;
            verticalOffset += elementHeight;

            conditionCounts.Add(contentHolder.transform.childCount);
        }
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(verticalOffset) - 100);
    }
}
