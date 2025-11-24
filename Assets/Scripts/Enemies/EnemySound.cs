using UnityEngine;

public class EnemySound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }

    public void PlayHitAt(Vector3 pos)
{
    AudioSource.PlayClipAtPoint(hitSound, pos);
}

public void PlayDeathAt(Vector3 pos)
{
    AudioSource.PlayClipAtPoint(deathSound, pos);
}
}
