using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFall : MonoBehaviour
{
    public float speed = 5f; 

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            particles[i].position += Vector3.down * speed * Time.deltaTime;
        }

        ps.SetParticles(particles, count);
    }
}
