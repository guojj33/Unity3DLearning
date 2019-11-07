using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerHalo : MonoBehaviour
{

    public ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particleArray;
    private int haloResolution = 3500;
    private float minRadius = 2.5F;
    private float maxRadius = 4F;
    private HaloParticle[] haloParticle;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = this.GetComponent<ParticleSystem> ();
        particleArray = new ParticleSystem.Particle[haloResolution];
        haloParticle = new HaloParticle[haloResolution];
        particleSystem.Emit(haloResolution);
        particleSystem.GetParticles(particleArray);
        for(int i = 0; i < haloResolution; ++i){
            float shiftMinRadius = Random.Range(1, (maxRadius + minRadius) / 2 / minRadius);
            float shiftMaxRadius = Random.Range((maxRadius + minRadius) / 2 / maxRadius, 1);
            float radius = Random.Range(minRadius * shiftMinRadius, maxRadius * shiftMaxRadius);

            float angle = Random.Range(0, Mathf.PI * 2);

            haloParticle[i] = new HaloParticle(radius, angle);
            particleArray[i].position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
        }
        particleSystem.SetParticles(particleArray, particleArray.Length);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < haloResolution; ++i){
           haloParticle[i].angle -= Random.Range(0, 1F/360) / 2;
           float angle = haloParticle[i].angle;
           float radius = haloParticle[i].radius;
           particleArray[i].position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
        }
        particleSystem.SetParticles(particleArray, particleArray.Length);
    }
}
