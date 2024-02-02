using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] List<Vector3> locationsToCycleThrough = new();

    private List<Color> locationColor = new();

    private bool colorsCreated = false;
    
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {


        for (int i = 0; i < locationsToCycleThrough.Count; i++)
        {
            while (locationColor.Count < locationsToCycleThrough.Count) locationColor.Add(new Color(
                                                                                            Random.Range(0,1f),
                                                                                            0f,
                                                                                            Random.Range(0, 1f)
                                                                                        ));
            while (locationColor.Count > locationsToCycleThrough.Count) locationColor.RemoveAt(locationColor.Count - 1);
            if (locationsToCycleThrough.Count == 0) locationColor.RemoveAt(0);
            Debug.Log(locationColor[i]);
            Gizmos.color = locationColor[i];
            Gizmos.DrawSphere(locationsToCycleThrough[i], .2f);
        }
    }
}
