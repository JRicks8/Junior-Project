using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverSpin : MonoBehaviour
{

    [SerializeField] private float bobHeight;
    [SerializeField][Range(-100, 100)] private float spinSpeed;
    [SerializeField][Range(1, 100)] private float bobSpeed;
    [SerializeField]
    [Range(0, 1)] private float animStartOffset;

    private Vector3 homePosition;
    private Vector3 targetPosition;

    private void Start()
    {
        homePosition = transform.position;
        targetPosition = homePosition + new Vector3(0f, bobHeight, 0f);
    }

    void Update()
    {
        if (animStartOffset > 0)
        {
            animStartOffset -= Time.deltaTime;
            return;
        }
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y + spinSpeed / 100f, transform.eulerAngles.z);
        if (targetPosition.y > homePosition.y)
        {
            transform.position = transform.position + new Vector3(0f, bobSpeed / 1000, 0f);
            if (transform.position.y > targetPosition.y)
            {
                targetPosition = homePosition - new Vector3(0f, bobHeight, 0f);
            }
        }
        if (targetPosition.y < homePosition.y)
        {
            transform.position = transform.position - new Vector3(0f, bobSpeed / 1000, 0f);
            if (transform.position.y < targetPosition.y)
            {
                targetPosition = homePosition + new Vector3(0f, bobHeight, 0f);
            }
        }
    }
}
