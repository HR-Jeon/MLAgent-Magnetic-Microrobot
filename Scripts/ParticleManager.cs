using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private float cycleInterval = 0.01f;
    private List<ChargedParticle> chargedParticles;
    private List<MovingChargedParticle> movingChargedParticles;
    public MovingChargedParticle Magnet1;
    public MovingChargedParticle Magnet2;
    Rigidbody r_magnet1;
    Rigidbody r_magnet2;
    // Start is called before the first frame update
    public void Start()
    {
        chargedParticles = new List<ChargedParticle>(FindObjectsOfType<ChargedParticle>());
        movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());
        movingChargedParticles[0] = Magnet1;
        movingChargedParticles[1] = Magnet2;

        r_magnet1 = Magnet1.GetComponent<Rigidbody>();
        r_magnet2 = Magnet2.GetComponent<Rigidbody>();
        
        foreach(MovingChargedParticle mcp in movingChargedParticles){
            StartCoroutine(Cycle(mcp));
        }
    }

    public IEnumerator Cycle(MovingChargedParticle mcp){
        bool isFirst = true;

        while(true){
            if(isFirst){
                isFirst = false;
                yield return new WaitForSeconds(Random.Range(0, cycleInterval));
            }

            ApplyMagneticForce(mcp);
            yield return new WaitForSeconds(cycleInterval);
        }
    }

    private void ApplyMagneticForce(MovingChargedParticle mcp){
        Vector3 newForce = Vector3.zero;
        foreach(ChargedParticle cp in chargedParticles){
            if(mcp == cp)
                continue;
            
            float distance = Vector3.Distance(mcp.transform.position, cp.transform.position);
            float force = 9*Mathf.Pow(10,9)*mcp.charge*cp.charge / Mathf.Pow(distance,2);

            Vector3 diretion = mcp.transform.position - cp.transform.position;
            diretion.Normalize();

            newForce += force*diretion*cycleInterval;

            if(float.IsNaN(newForce.x))
                newForce = Vector3.zero;

            if(mcp == Magnet1)
                r_magnet1.AddForce(newForce);

            else if(mcp == Magnet2)
                r_magnet2.AddForce(newForce);
        }
    }
}
