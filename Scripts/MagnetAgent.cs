using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;

public class MagnetAgent : Agent
{
    public float MaxForce = 10000.0f;
    private float cycleInterval = 0.01f;
    private List<ChargedParticle> chargedParticles;
    private List<MovingChargedParticle> movingChargedParticles;
    public MovingChargedParticle Magnet1;
    public MovingChargedParticle Magnet2;
    Rigidbody r_magnet1;
    Rigidbody r_magnet2;

    public GameObject Arm1;
    public GameObject Arm2;
    public GameObject Arm3;

    public Transform Target;

    public override void Initialize(){
        chargedParticles = new List<ChargedParticle>(FindObjectsOfType<ChargedParticle>());
        movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());
        movingChargedParticles[0] = Magnet1;
        movingChargedParticles[1] = Magnet2;

        r_magnet1 = Magnet1.GetComponent<Rigidbody>();
        r_magnet2 = Magnet2.GetComponent<Rigidbody>();

        // for(int i = 0; i<8; i++){
        //     print(chargedParticles[i].name);
        // }
    }

    public override void OnEpisodeBegin(){
        Arm1.transform.position = new Vector3(0f, 2f, 0f);
        Arm1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Arm2.transform.position = new Vector3(1.47f, 2.2f, 0f);
        Arm2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Arm3.transform.position = new Vector3(-1.47f, 2.2f, 0f);
        Arm3.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Magnet1.transform.position = new Vector3(0.72f, 2.1f, 0f);
        Magnet1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        r_magnet1.velocity = Vector3.zero;
        r_magnet1.angularVelocity = Vector3.zero;

        Magnet2.transform.position = new Vector3(-0.72f, 2.1f, 0f);
        Magnet2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        r_magnet2.velocity = Vector3.zero;
        r_magnet2.angularVelocity = Vector3.zero;

        Target.localPosition = new Vector3(Random.Range(10f, 18f),
                                           2f,
                                           Random.Range(10f, 18f));
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(Arm1.transform.localPosition);
        sensor.AddObservation(Arm1.transform.rotation);

        sensor.AddObservation(Arm2.transform.localPosition);
        sensor.AddObservation(Arm2.transform.rotation);

        sensor.AddObservation(Magnet1.transform.localPosition);
        sensor.AddObservation(Magnet1.transform.rotation);
        sensor.AddObservation(r_magnet1.velocity);
        sensor.AddObservation(r_magnet1.angularVelocity);

        sensor.AddObservation(Magnet2.transform.localPosition);
        sensor.AddObservation(Magnet2.transform.rotation);
        sensor.AddObservation(r_magnet2.velocity);
        sensor.AddObservation(r_magnet2.angularVelocity);

        sensor.AddObservation(Target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers){
        var continuousActions = actionBuffers.ContinuousActions;

        var charge_value1 = Mathf.Clamp(continuousActions[0], 0f, 1f) * 0.005f;
        var charge_value2 = Mathf.Clamp(continuousActions[1], 0f, 1f) * 0.005f;

        for(int i = 0; i < 8; i++){
            if(chargedParticles[i].name == "Wall1-1" || chargedParticles[i].name == "Wall1-2"){
                chargedParticles[i].charge = charge_value1;
            }
            else if(chargedParticles[i].name == "Wall3-1" || chargedParticles[i].name == "Wall3-2"){
                chargedParticles[i].charge = -1*charge_value1;
            }
            else if(chargedParticles[i].name == "Wall2-1" || chargedParticles[i].name == "Wall2-2"){
                chargedParticles[i].charge = charge_value2;
            }
            else{
                chargedParticles[i].charge = -1*charge_value2;
            }
        }

        float distanceToTarget = Vector3.Distance(Arm1.transform.localPosition, Target.localPosition);

        if (distanceToTarget < 3f)
        {
            SetReward(0.5f);
        }

        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if(Arm1.transform.position.y > 2.1f || Arm1.transform.position.y < -2.1f ||
           Arm1.transform.position.x > 19.5f || Arm1.transform.position.x < -19.5f || 
           Arm1.transform.position.z > 19.5f || Arm1.transform.position.z < -19.5f){
            SetReward(-0.005f);
            EndEpisode();
        }
    }

    void FixedUpdate(){
        var magCount = chargedParticles.Count;

        for (int i = 0; i < 2; i++)
        {
            var m1 = movingChargedParticles[i];
            var accF1 = Vector3.zero;
            var accF2 = Vector3.zero;

            for (int j = 0; j < magCount; j++)
            {
                var m2 = chargedParticles[j];
                var f = CalculateGilbertForce(m1, m2);

                accF1 += f * cycleInterval;
            }

            if (accF1.magnitude > MaxForce)
            {
                accF1 = accF1.normalized * MaxForce;
            }

            if(i == 0){r_magnet1.AddForceAtPosition(accF1, m1.transform.position);}
            else{r_magnet2.AddForceAtPosition(accF1, m1.transform.position);}
        }
    }

    Vector3 CalculateGilbertForce(MovingChargedParticle mcp, ChargedParticle cp)
    {
        float distance = Vector3.Distance(mcp.transform.position, cp.transform.position);
        float force = 9*Mathf.Pow(10,9)*mcp.charge*cp.charge / Mathf.Pow(distance,2);

        Vector3 direction = mcp.transform.position - cp.transform.position;
        direction.Normalize();

        return force * direction;
    }
}
