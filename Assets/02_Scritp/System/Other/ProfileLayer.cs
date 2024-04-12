using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileLayer : MonoBehaviour
{
    public float semiMajorAxis; // 타원의 장축 반지름
    public float semiMinorAxis; // 타원의 단축 반지름

    int childCount;

    private void Update()
    {
        if (childCount != transform.childCount)
        {
            childCount = transform.childCount;
            ArrangeChildren();
        }
    }

    private void ArrangeChildren() //유저 추가 시 호출
    {
        float angleStep = 360f / childCount;

        for (int i = 0; i < childCount; i++)
        {
            float angle = i * angleStep + 90f;
            float x = transform.position.x + semiMajorAxis * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = transform.position.y + semiMinorAxis * Mathf.Sin(angle * Mathf.Deg2Rad);

            Transform child = transform.GetChild(i);
            child.position = new Vector3(x, y, 0);
        }
    }
}
