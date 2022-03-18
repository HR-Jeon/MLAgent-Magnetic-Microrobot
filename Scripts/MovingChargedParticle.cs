using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingChargedParticle : MonoBehaviour
{
    public float charge = 0.000001f;

    void Start()
    {
        UpdateColor();
    }

    public void UpdateColor(){
        Color color = charge > 0 ? Color.green : Color.red;
        GetComponent<Renderer>().material.color = color;
    }
}
