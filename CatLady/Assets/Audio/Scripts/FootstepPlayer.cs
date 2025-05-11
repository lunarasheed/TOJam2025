using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float speedThreshold = 0.1f; // Minimum movement speed to trigger sound
    public float footstepInterval = 0.4f; // Time between footsteps

    private Rigidbody2D rb;
    private float footstepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footstepTimer = 0f;
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > speedThreshold)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f && footstepClips.Length > 0)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // Reset timer so footsteps don't play immediately after stopping
        }
    }

    void PlayFootstep()
    {
        int index = Random.Range(0, footstepClips.Length);
        footstepSource.clip = footstepClips[index];
        footstepSource.Play();
    }
}
