using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheels : MonoBehaviour
{
    public GameObject parent;
    private Transform transf;
    private Vector3 lastPos;

    private Vector3 rotVec = new Vector3(0,-1,0);
    // Start is called before the first frame update
    void Start()
    {
        transf = parent.transform;
        lastPos = transf.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPos != transf.position)
            transform.Rotate(rotVec,Space.Self);
        lastPos = transf.position;
    }
}
