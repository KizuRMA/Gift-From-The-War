using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rantanMove : MonoBehaviour
{
    Transform rantanTransform;
    int time;
    int timeMax;
    int hugou;
    float move;

    // Start is called before the first frame update
    void Start()
    {
        rantanTransform = GetComponent<Transform>();
        time = 450;
        timeMax = 900;
        hugou = 1;
        move = 0.0001f;
    }

    // Update is called once per frame
    void Update()
    {
        time += hugou;

        if(time > timeMax || time < 0)
        {
            hugou *= -1;
        }
        if (hugou > 0)
        {
            rantanTransform.position += new Vector3(0, move, 0);
        }
        else
        {
            rantanTransform.position += new Vector3(0, -move, 0);
        }
    }
}
