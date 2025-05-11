using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
		while (floatingTextObj == null)
		{
			Debug.LogError("Failed to instantiate floating text prefab!");
			return;
		}

		Canvas canvas = floatingTextObj.GetComponent<Canvas>();

		// Set the canvas to world space
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.sortingOrder = 10; // Set a higher sorting order to ensure it's on top

		// Set the canvas to the same size as the pickup
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		canvasRect.sizeDelta = new Vector2(1, 1); // Adjust size as needed

		// Start coroutine to animate the text
		StartCoroutine(AnimateFloatingText(floatingTextObj));
	}

	private System.Collections.IEnumerator AnimateFloatingText(GameObject textObj)
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
			CanvasRenderer canvasRenderer = textObj.GetComponentInChildren<CanvasRenderer>();
			if (canvasRenderer != null)
			{
				canvasRenderer.SetAlpha(alpha);
			}

			// Wait for the next frame
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
				Renderer renderer = GetComponent<Renderer>();
				if (renderer != null)
				{
					renderer.enabled = false; // Hide the pickup object
				}
				else
				{
					Debug.LogWarning("Renderer component not found on the pickup object.");
				}
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
