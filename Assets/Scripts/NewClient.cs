using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
public class NewClient : MonoBehaviour
{
    List<List<Vector3>> positions;

    public GameObject[] spheres;
    public GameObject[] lights;
    public float timeToUpdate = 1.0f;
    private float timer;
    public float dt;
    private bool dataRequested = false;
    private bool dataReceived = false;
    private float timer2 = 0;
    public int bufferSize = 3;
    private int posCount = 0;
    private float timer3 = 0;
    private int id = 0;
    public bool smoothen = false;

    void Smoothen()
    {   
        bool insertList = false;
        List<Vector3> list1 = positions[positions.Count - 2];
        List<Vector3> list2 = positions[positions.Count - 1];
        List<Vector3> list3 = new List<Vector3>();

        for(int i = 0; i < spheres.Length; i++)
        {
            Vector3 p0 = list1[i];
            Vector3 p1 = list2[i];
            Vector3 r = p1 - p0;
            Vector3 p01 = new Vector3(0,0,0);
            for(int j = 0; j < 3; j++)
            {
                if(r[j] == 2)
                {
                    p01[j] = p0[j]+1;
                    insertList = true;
                }
                else if(r[j] == -2)
                {
                    p01[j] = p0[j]-1;
                    insertList = true;
                }
                else
                    p01[j] = p1[j];
            }
            list3.Add(p01);
        }

        if(insertList)
        {
            print("Active");
            positions.RemoveAt(positions.Count - 1);
            positions.Add(list3);
            positions.Add(list2);
        }
    }

    IEnumerator SendData()
    {   
        int localId = id;
        id++;
        UnityWebRequest www = UnityWebRequest.Get("http://agentesinteligentes-e4.us-south.cf.appdomain.cloud/updatePositions");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            List<Vector3> newPositions = new List<Vector3>();
            string txt = www.downloadHandler.text.Replace('\'', '\"');
            txt = txt.TrimStart('"', '{', 'd', 'a', 't', 'a', ':', '[');
            txt = "{\"" + txt;
            txt = txt.TrimEnd(']', '}');
            if(spheres.Length > 1)
                txt = txt + '}';
            string[] strs = txt.Split(new string[] { "}, {" }, StringSplitOptions.None);
            //Debug.Log("strs.Length:" + strs.Length);
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].Trim();
                if (i == 0) strs[i] = strs[i] + '}';
                else if (i == strs.Length - 1) strs[i] = '{' + strs[i];
                else strs[i] = '{' + strs[i] + '}';
                Vector3 test = JsonUtility.FromJson<Vector3>(strs[i]);
                newPositions.Add(test);
            }

            List<Vector3> poss = new List<Vector3>();
            for (int s = 0; s < spheres.Length; s++)
            {
                //spheres[s].transform.localPosition = newPositions[s];
                poss.Add(newPositions[s]);
            }

            positions.Add(poss);
            dataReceived = true;
            dataRequested = false;
            if(positions.Count > 1 && smoothen)
                Smoothen();
            //print(localId);
            //print(positions[positions.Count-1][0]);
        }
      
    }

    IEnumerator Restarts()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://agentesinteligentes-e4.us-south.cf.appdomain.cloud");
        yield return www.Send();        
    }


    IEnumerator SendLights()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://agentesinteligentes-e4.us-south.cf.appdomain.cloud/trafficLight");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);

            string txt = www.downloadHandler.text.Replace('\'', '\"');
            //Debug.Log(txt[11] + " " + txt[25] + " " + txt[39] + " " + txt[53]);
            changeColor(0, txt[11] - '0');
            changeColor(1, txt[25] - '0');
            changeColor(2, txt[39] - '0');
            changeColor(3, txt[53] - '0');
        }
    }

    void changeColor(int index, int state)
    {
        GameObject red= lights[index].transform.GetChild(0).GetChild(0).gameObject;
        GameObject yellow= lights[index].transform.GetChild(0).GetChild(1).gameObject;
        GameObject green = lights[index].transform.GetChild(0).GetChild(2).gameObject;
        red.SetActive(false);
        yellow.SetActive(false);
        green.SetActive(false);
        if (state == 0)
        {
            red.SetActive(true);
        }
        else if(state == 1)
        {
            yellow.SetActive(true);
        }
        else
        {
            green.SetActive(true);
        }
    }

    // Start Method()
    void Start()
    {
        positions = new List<List<Vector3>>();
#if UNITY_EDITOR
        StartCoroutine(Restarts());
        for(int i = 0; i < bufferSize; i++)
            StartCoroutine(SendData());
        StartCoroutine(SendLights());
        timer = timeToUpdate;
        timer2 = timer;
#endif
    }

    // Update Method()
    void Update()
    {
        // Update timers
        timer -= Time.deltaTime;
        timer2 -= Time.deltaTime;

        // Set timer when data is requested
        if(dataRequested)
            timer3 += Time.deltaTime;

        // End timer and consider it's duration for providing the following dataset on time
        if(dataReceived)
        {
            timer2 -= timer3;
            timer3 = 0;
        }

        // Ask for a new dataset
        if (timer2 < 0)
        {
#if UNITY_EDITOR
            //dt = 1;
            timer2 = timeToUpdate; // reset the timer 
            //timerRestarted = true;         
            StartCoroutine(SendData());
            StartCoroutine(SendLights());
            dataRequested = true;
#endif
        }

        // Change the data that is currently interpolating
        if(timer < 0)
        {
            timer = timeToUpdate;
            posCount++;
        }

        dt = 1.0f - (timer / timeToUpdate);

        if (positions.Count > bufferSize-1)
        {
            for (int s = 0; s < spheres.Length; s++)
            {
                List<Vector3> last = positions[posCount + 1];
                List<Vector3> prevLast = positions[posCount];
 
                Vector3 interpolated = Vector3.Lerp(prevLast[s], last[s], dt);
                spheres[s].transform.localPosition = interpolated;

                // direction changed?
                if (last[s] != prevLast[s])
                {
                    Vector3 dir = last[s] - prevLast[s];
                    spheres[s].transform.rotation = Quaternion.LookRotation(dir);
                }
                
            }
        }
        dataReceived = false;
    }
}
