using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedParticle : MonoBehaviour
{
    [Range(-0.05f, 0.05f)]
    public float charge = 0f;

    void Start()
    {
        UpdateColor();
    }

    public void UpdateColor(){
        Color color = charge > 0 ? Color.green : Color.red;
        GetComponent<Renderer>().material.color = color;
    }
}
