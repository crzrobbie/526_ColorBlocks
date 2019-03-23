using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Block2 : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 0.3f;
    public GameObject tailPrefab;

    Vector2 dir = Vector2.right;

    Hashtable domain = new Hashtable();
    Hashtable tail = new Hashtable();

    bool inDomain = false;

    float minX;
    float minY;
    float maxX;
    float maxY;

    float startX;
    float startY;

    Vector2 startPoint;
    Vector2 endPoint;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    void Start()
    {
        //initialize value
        startX = transform.position.x;
        minX = transform.position.x;
        maxX = transform.position.x;

        startY = transform.position.y;
        minY = transform.position.y;
        maxY = transform.position.y;

        startPoint = transform.position;
        createDomain(); //create start block
        InvokeRepeating("Move", speed, speed); //control
    }

    void Move()
    {
        //Debug.Log("before transform:"+vector);
        Vector2 vector = transform.position;
        transform.Translate(dir);
        string key = vector.ToString();

        if (!domain.ContainsKey(key) && !tail.ContainsKey(key))
        {
            if(inDomain)
            {
                startPoint = vector;
            }
            inDomain = false;
            GameObject trace = Instantiate(tailPrefab, vector, Quaternion.identity);
            tail.Add(key, trace);
            updateField(vector);
        }
        //Debug.Log("MinX:" + minX + " MaxX:" + maxX + " MinY:" + minY + " MaxY:" + maxY);
    }


    void Update()
    {
        Control();
    }

    private void Control()
    {
        if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
            dir = Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
            dir = Vector2.left;
        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
            dir = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
            dir = Vector2.down;
    }

    void createDomain()
    {
        Vector2 vector = transform.position;
        int width = 3;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 v = new Vector2(vector.x - i, vector.y + 1 - j);
                GameObject d = Instantiate(tailPrefab, v, Quaternion.identity);
                domain.Add(v.ToString(), d);
                updateField(v);
                //Debug.Log(v);
            }
        }
    }

    private void updateField(Vector2 v)
    {
        minX = Math.Min(minX, v.x);
        maxX = Math.Max(maxX, v.x);
        minY = Math.Min(minY, v.y);
        maxY = Math.Max(maxY, v.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float x = collision.transform.position.x;
        float y = collision.transform.position.y;
        if (x == startX && y == startY)
        {
            Debug.Log("Start Point");
        }
        else
        {
            string position = ((Vector2)collision.transform.position).ToString();
            if (domain.ContainsKey(position))
            {
                //collide domain
                //Debug.Log("collide domain");
                if (!inDomain)
                {
                    //first collide domain
                    endPoint = (Vector2)collision.transform.position;
                    inDomain = true;
                    Debug.Log("first collide domain");
                    
                    //stopwatch.Start();
                    fill();
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //Debug.Log("total time:"+timeSpan.TotalMilliseconds);
                }
                else
                {
                    //head is in domain
                }
            }
            else if (tail.ContainsKey(position))
            {
                //collide tail
                Debug.Log("collide tail");
                //TODO clear all objects and game over
                ICollection key = domain.Keys;
                foreach (string k in key)
                {
                    Destroy((GameObject)domain[k]);
                }
                domain.Clear();
                key = tail.Keys;
                foreach (string k in key)
                {
                    Destroy((GameObject)tail[k]);
                }
                tail.Clear();
                Destroy(this);
            }
            else
            {
                //collide border
            }
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Collision stay");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Collision finish");
    }

    private void fill()
    {
        stopwatch.Start();
        Vector2 seed = findSeed();
        stopwatch.Stop();
        TimeSpan timeSpan = stopwatch.Elapsed;
        Debug.Log("findSeed:" + timeSpan.TotalMilliseconds);
        ICollection key = tail.Keys;
        foreach (string k in key)
        {
            if (!domain.ContainsKey(k))
            {
                domain.Add(k, tail[k]);
            }
        }
        tail.Clear();
        stopwatch.Start();
        if(seed!=Vector2.zero)
        {
            boundarySeedFill(seed);
        }
        stopwatch.Stop();
        timeSpan = stopwatch.Elapsed;
        Debug.Log("fill:" + timeSpan.TotalMilliseconds);
    }

    float[,] direction_8 = {
        {-1, 1},
        {-1, 0},
        {-1,-1},
        {0,  1},
        {0, -1},
        {1,  1},
        {1,  0},
        {1, -1}
    };
    private void boundarySeedFill(Vector2 v)
    {
        Stack<Vector2> stack = new Stack<Vector2>();
        stack.Push(v);
        while(stack.Count>0)
        {
            Vector2 curr = stack.Pop();
            if (!domain.ContainsKey(curr.ToString()))
            {
                GameObject body = Instantiate(tailPrefab, curr, Quaternion.identity);
                domain.Add(curr.ToString(), body);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 adj = new Vector2(curr.x + direction_8[i, 0], curr.y + direction_8[i, 1]);
                    if (!domain.ContainsKey(adj.ToString()))
                    {
                        stack.Push(adj);
                    }
                }
            }
        }

        /*
        if(!domain.ContainsKey(v.ToString()))
        {
            GameObject body = Instantiate(tailPrefab, v, Quaternion.identity);
            domain.Add(v.ToString(), body);
            for(int i = 0; i < direction_8.Length; i++)
            {
                boundarySeedFill(new Vector2(v.x + direction_8[i, 0], v.y + direction_8[i,1]));
            }
        }
        */
    }

    float[,] direction_4 = {       
        {-1, 0},      
        {0,  1},
        {0, -1},       
        {1,  0}
    };

    private Vector2 findSeed()
    {
        Vector2 v1 = new Vector2(startPoint.x, endPoint.y);
        Vector2 v2 = new Vector2(endPoint.x, startPoint.y);
        Vector2 target;
        if (domain.ContainsKey(v1.ToString()))
        {
            target = v1;
        }
        else
        {
            target = v2;
        }
        if(target.x == startPoint.x)
        {
            float min = Math.Min(startPoint.y, target.y);
            float max = Math.Max(startPoint.y, target.y);
            for(float i = min; i <= max; i++)
            {
                Vector2 tmp = new Vector2(target.x, i);
                if(!tail.ContainsKey(tmp.ToString()))
                {
                    tail.Add(tmp.ToString(), null);
                }
            }
            min = Math.Min(endPoint.x, target.x);
            max = Math.Max(endPoint.x, target.x);
            for(float i = min; i <= max; i++)
            {
                Vector2 tmp = new Vector2(i, target.y);
                if (!tail.ContainsKey(tmp.ToString()))
                {
                    tail.Add(tmp.ToString(), null);
                }
            }
        }
        else
        {
            float min = Math.Min(endPoint.y, target.y);
            float max = Math.Max(endPoint.y, target.y);
            for (float i = min; i <= max; i++)
            {
                Vector2 tmp = new Vector2(target.x, i);
                if (!tail.ContainsKey(tmp.ToString()))
                {
                    tail.Add(tmp.ToString(), null);
                }
            }
            min = Math.Min(startPoint.x, target.x);
            max = Math.Max(startPoint.x, target.x);
            for (float i = min; i <= max; i++)
            {
                Vector2 tmp = new Vector2(i, target.y);
                if (!tail.ContainsKey(tmp.ToString()))
                {
                    tail.Add(tmp.ToString(), null);
                }
            }
        }

        ICollection key = domain.Keys;
        foreach(string k in key)
        {
            Vector2 v = getVectorFromString(k);
            for(int i = 0; i < 4; i++)
            {
                Vector2 tmp = new Vector2(v.x + direction_4[i, 0], v.y + direction_4[i, 1]);
                if(!domain.ContainsKey(tmp.ToString()) && !tail.ContainsKey(tmp.ToString()))
                {
                    if (IsPointInPolygon(new Vector2(tmp.x,tmp.y)))
                    {
                        return tmp;
                    }
                }
            }
        }

        return Vector2.zero;
    }

    private Vector2 getVectorFromString(string k)
    {
        string tmp = k.Substring(1, k.Length - 2);
        string[] arrays = tmp.Split(',');
        return new Vector2(Convert.ToSingle(arrays[0].Trim()), Convert.ToSingle(arrays[1].Trim()));
    }

    private bool IsPointInPolygon(Vector2 v)
    {
        int num = 0;
        while(v.x<=maxX && v.y<=maxY)
        {
            if(tail.ContainsKey(v.ToString()))
            {
                num++;
            }
            v.x = v.x+1;
            v.y = v.y+1;
        }
        return num % 2 == 1;
    }
}
