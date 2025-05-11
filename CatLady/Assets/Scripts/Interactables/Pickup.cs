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
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			// Add the item to the player's inventory
			InventoryManager.Instance.AddItem(this);
			// Play the pickup sound
			if (pickupSound != null)
			{
				pickupSound.Play();
			}
			else
			{
				Debug.LogWarning("Pickup sound not assigned.");
			}
			// Make the player sanity increase
			PlayerController2D playerController = other.GetComponent<PlayerController2D>();
			if (playerController != null)
			{
				playerController.AddSanity(10f); // Increase sanity by 10
				playerController.onSanityChanged.Invoke(playerController.CurrentSanity);
			}
			else
			{
				Debug.LogWarning("PlayerController2D component not found on the player object.");
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
