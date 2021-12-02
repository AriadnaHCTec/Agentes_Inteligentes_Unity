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

    IEnumerator SendData()
    {   
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
        StartCoroutine(SendLights());
        timer = timeToUpdate;
#endif
    }

    // Update Method()
    void Update()
    {
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
#if UNITY_EDITOR
            timer = timeToUpdate; // reset the timer       
            StartCoroutine(SendData());
            StartCoroutine(SendLights());
            dataRequested = true;
#endif
        }

        dt = 1.0f - (timer / timeToUpdate);

        //if(dataRequested && !dataReceived)
            //dt = 1.0f;

        if (positions.Count > 2)
        {
            for (int s = 0; s < spheres.Length; s++)
            {
                List<Vector3> last;
                List<Vector3> prevLast;
                if(dataRequested && !dataReceived)
                {
                    last = positions[positions.Count - 1];
                    prevLast = positions[positions.Count - 2];
                }
                else
                {   
                    last = positions[positions.Count - 2];
                    prevLast = positions[positions.Count - 3];
                }

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
