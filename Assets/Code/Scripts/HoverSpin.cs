using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverSpin : MonoBehaviour
{

    [SerializeField]  private float bobHeight;
    [SerializeField][Range(-100, 100)] private float spinSpeed;
    [SerializeField][Range(1, 100)] private float bobSpeed;

    private Vector3 homePosition;
    private Vector3 targetPosition;

    private void Start()
    {
        homePosition = transform.position;
        targetPosition = homePosition + new Vector3(0f, bobHeight, 0f);
    }

    void Update()
    {

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y + spinSpeed / 100f, transform.eulerAngles.z);
        if (targetPosition.y > homePosition.y)
        {
            transform.position = transform.position + new Vector3(0f, bobSpeed / 100, 0f);
            if (transform.position.y > targetPosition.y)
            {
                targetPosition = homePosition - new Vector3(0f, bobHeight, 0f);
            }
        }
        if (targetPosition.y < homePosition.y)
        {
            transform.position = transform.position - new Vector3(0f, bobSpeed / 100, 0f);
            if (transform.position.y < targetPosition.y)
            {
                targetPosition = homePosition + new Vector3(0f, bobHeight, 0f);
            }
        }
    }
}
