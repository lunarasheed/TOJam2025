using UnityEngine;

public class Pickup : MonoBehaviour
{
    public AudioSource pickupSound; // Sound to play when the item is picked up
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if the AudioSource component is attached
        if (pickupSound == null)
        {
            pickupSound = gameObject.GetComponent<AudioSource>();

            if (pickupSound == null)
            {
                Debug.LogError("AudioSource component not found on the pickup object.");
            }
        }
    }

    // On Intersect with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add the item to the player's inventory
            InventoryManager.Instance.AddItem(this);
            // lay the pickup sound
            if (pickupSound != null)
            {
                pickupSound.Play();
            }
            else
            {
                Debug.LogWarning("Pickup sound not assigned.");
            }
            // Destroy the pickup object
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Player tag not found on the object.");
        }
    }
}
