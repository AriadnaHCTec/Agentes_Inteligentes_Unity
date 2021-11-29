using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Author: Equipo 4
// Description: Script que activa/desactiva las luces del semáforo según el estado en el que se encuentre.
// 29-11-2021
public class TrafficLight : MonoBehaviour
{
    public int state = 0; // Estado. Puede ser modificado por otros Scripts
    private GameObject red;
    private GameObject yellow;
    private GameObject green;

    void Start(){
        // Se asignan los gameObjects de las luces
        red = transform.GetChild(0).gameObject;
        yellow = transform.GetChild(1).gameObject;
        green = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        
        if(state == 0) // Red
        {
            red.active = true;
            yellow.active = false;
            green.active = false;
        }
        else if(state == 1) // Yellow
        {
            red.active = false;
            yellow.active = true;
            green.active = false;
        }
        else // Green
        {
            red.active = false;
            yellow.active = false;
            green.active = true;
        }
    }
}
