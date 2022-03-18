using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachGoal : MonoBehaviour
{
    public GameObject agent;
    public GameObject Arm1;
    public GameObject Arm2;
    public GameObject Arm3;

    public void OnTriggerEnter(Collider other){
        if(other.gameObject == Arm1 || other.gameObject == Arm2 || other.gameObject == Arm3){
            print("Touched");
            agent.GetComponent<MagnetAgent>().SetReward(1.0f); 
            agent.GetComponent<MagnetAgent>().EndEpisode();             
        }
    }
}
