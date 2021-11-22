using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizacion : MonoBehaviour
{
    List<GameObject> particles;
    public int NUM_PARTICLES;
    Vector3 camPos;
    Camera cam;
    Vector3 xCam;
    Vector3 yCam;
    Vector3 zCam;
    Vector3 w;
    float near;
    float far;
    float width;
    float height;
    void Start()
    {
        particles = new List<GameObject>();
        for(int i=0;i< NUM_PARTICLES;i++){
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float x = Random.Range(-4.0f, 4.0f);
            float z = Random.Range(-4.0f, 4.0f);
            sphere.transform.position = new Vector3(x, 10, z);
            //sphere.transform.position = new Vector3(0, 10, 0);
            Rigidbody rb = sphere.AddComponent<Rigidbody>(); // Simulación de Física        
            float r = Random.Range(0f, 1.0f);
            float g = Random.Range(0f, 1.0f);
            float b = Random.Range(0f, 1.0f);
            Color c = new Color(r, g, b);
            sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
            float scale = Random.Range(0.2f, 1.5f);
            sphere.transform.localScale = new Vector3(scale, scale, scale);
            rb.mass = scale;
            particles.Add(sphere);
        }

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camPos = cam.transform.position.normalized;
        //Debug.Log("CAMERA: " + camPos.ToString("F5"));

        xCam = cam.transform.right.normalized;
        yCam = cam.transform.up.normalized;
        zCam = cam.transform.forward.normalized;
        /*Debug.Log("CAM x: " + xCam.ToString("F5"));
        Debug.Log("CAM y: " + yCam.ToString("F5"));
        Debug.Log("CAM z: " + zCam.ToString("F5"));*/

        near = cam.nearClipPlane;
        far = cam.farClipPlane;
        width = cam.rect.width / 10;
        height = cam.pixelHeight / 10;
        /*Debug.Log("Near: " + near);
        Debug.Log("Far: " + far);
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + near);*/
    }

    void calcularCulling(GameObject particle, Camera cam, float near, float far, float width, float height){
        //xCam = cam.transform.right;
        //yCam = cam.transform.up;
        //zCam = cam.transform.forward;
        camPos = cam.transform.position.normalized;
        //Debug.Log("CAMERA: " + camPos.ToString("F5"));

        xCam = cam.transform.right.normalized;
        yCam = cam.transform.up.normalized;
        zCam = cam.transform.forward.normalized;

        /*Debug.Log("CAM x: " + xCam.ToString("F5"));
        Debug.Log("CAM y: " + yCam.ToString("F5"));
        Debug.Log("CAM z: " + zCam.ToString("F5"));*/

        near = cam.nearClipPlane;
        far = cam.farClipPlane;
        width = cam.rect.width;
        height = cam.pixelHeight;

        particle.GetComponent<MeshRenderer>().enabled = true;
        //Vector3 camPos = cam.transform.position.normalized;
        w = particle.transform.position - camPos;
        float dotZ = Vector3.Dot(w,zCam);
        float dotY = Vector3.Dot(w,yCam);
        float dotX = Vector3.Dot(w,xCam);
        if (dotZ < near || dotZ > far){
            //desaparecer
            //particles.RemoveAt(position);
            particle.GetComponent<MeshRenderer>().enabled = false;
        }
        if (dotY < -height/2 || dotY > height/2){
            //desaparecer
            //particles.RemoveAt(position);
            particle.GetComponent<MeshRenderer>().enabled = false;
        }
        if (dotX < -width/2 || dotX > width/2){
            //desaparecer
            //particles.RemoveAt(position);
            particle.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("a")){
            cam.transform.Rotate(cam.transform.up, 1, Space.Self);
        }else if(Input.GetKey("d")){
            cam.transform.Rotate(cam.transform.up, -1, Space.Self);
        }
        for(int i=0;i<particles.Count;i++){
            calcularCulling(particles[i], cam, near, far, width, height);

        }
    }
}
