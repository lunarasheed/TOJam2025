using UnityEngine;
using TMPro;

public class Pickup : MonoBehaviour
{
	public AudioSource pickupSound; // Sound to play when the item is picked up

	[Header("Floating Text Settings")]
	[SerializeField] private GameObject floatingTextPrefab;
	[SerializeField] private float floatSpeed = 1f;
	[SerializeField] private float fadeSpeed = 1f;
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

	private void SpawnFloatingText()
	{
		if (floatingTextPrefab == null)
		{
			Debug.LogError("Floating text prefab not assigned!");
			return;
		}

		// Create the floating text object
		GameObject floatingTextObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
		TextMeshPro textMesh = floatingTextObj.GetComponentInChildren<TextMeshPro>();

		if (textMesh == null)
		{
			Debug.LogError("TextMeshPro component not found on prefab!");
			Destroy(floatingTextObj);
			return;
		}

		textMesh.text = "Sanity increased";
		textMesh.color = new Color(1f, 1f, 1f, 1f); // Start fully opaque

		// Start coroutine to animate the text
		StartCoroutine(AnimateFloatingText(floatingTextObj, textMesh));
	}

	private System.Collections.IEnumerator AnimateFloatingText(GameObject textObj, TextMeshPro textMesh)
	{
		float alpha = 1f;
		Vector3 startPos = textObj.transform.position;

		while (alpha > 0f)
		{
			// Move upward
			textObj.transform.position = new Vector3(
				startPos.x,
				startPos.y + (1f - alpha) * floatSpeed,
				startPos.z
			);

			// Fade out
			alpha -= Time.deltaTime * fadeSpeed;
			textMesh.color = new Color(1f, 1f, 1f, alpha);

			yield return null;
		}

		// Destroy the pickup object
		Destroy(gameObject);
		// Destroy the floating text object
		Destroy(textObj);
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
				SpawnFloatingText(); // Add this line here
			}
			else
			{
				Debug.LogWarning("PlayerController2D component not found on the player object.");
			}
		}
		else
		{
			Debug.LogWarning("Player tag not found on the object.");
		}
	}
}
