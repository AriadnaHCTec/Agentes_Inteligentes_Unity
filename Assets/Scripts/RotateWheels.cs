using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheels : MonoBehaviour
{
    public bool rotateWheel;
    private Vector3 rotVec = new Vector3(0,-1,0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateWheel)
            transform.Rotate(rotVec,Space.Self);
    }
}
