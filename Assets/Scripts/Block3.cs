using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Block3 : MonoBehaviour
{

    public GameObject tailPrefab;
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        stopwatch.Start();
        for (int i = 0; i < 300; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                Vector2 v = new Vector2(i, j);
                GameObject trace = Instantiate(tailPrefab, v, Quaternion.identity);
            }
        }
        stopwatch.Stop();
        TimeSpan timeSpan = stopwatch.Elapsed;
        Debug.Log("findSeed:" + timeSpan.TotalMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
