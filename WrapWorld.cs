using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapWorld : MonoBehaviour
{
    public static float rightBound = 64;
    public static float leftBound = -64;

    private static List<Transform> wrapTransforms = new List<Transform>();

   
    public static void AddToWrapList(Transform wrapTransform)
    {
        wrapTransforms.Add(wrapTransform);
    }

    private void Update()
    {
        foreach(var t in wrapTransforms)
        {
            if (t.parent != null) continue;
            float xpos = t.position.x;
            if(xpos > rightBound)
            {
                Vector3 pos = t.position;
                pos.x = leftBound;
                t.position = pos;
            }
            else if(xpos < leftBound)
            {
                Vector3 pos = t.position;
                pos.x = rightBound;
                t.position = pos;
            }
        }
    }

}
