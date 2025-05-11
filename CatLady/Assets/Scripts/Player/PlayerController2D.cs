/**
 * PlayerController2D.cs
 * This script controls the player's movement and actions in a 2D game.
 * It handles input for movement, action button, and animation parameters.
 */

using UnityEngine;
using UnityEngine.Events;

public class PlayerController2D : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] private float moveSpeed = 5f;

	[Header("Action Settings")]
	[SerializeField] private KeyCode actionKey = KeyCode.Space;
	[SerializeField] private float actionCooldown = 0.5f;

	private Rigidbody2D rb;
	private Vector2 movement;
	private float lastActionTime;
	private Animator animator;
	private float sanity = 100f;

	public float CurrentSanity => sanity;
	public UnityEvent<float> onSanityChanged = new UnityEvent<float>();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		// Get input
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		// Normalize movement to prevent faster diagonal movement
		if (movement.magnitude > 1)
		{
			movement.Normalize();
		}

		// Handle action button press
		if (Input.GetKeyDown(actionKey) && Time.time > lastActionTime + actionCooldown)
		{
			PerformAction();
			lastActionTime = Time.time;
		}

		// Update animation parameters if animator exists
		if (animator != null)
		{
			animator.SetFloat("Horizontal", movement.x);
			animator.SetFloat("Vertical", movement.y);
			animator.SetFloat("Speed", movement.magnitude);
		}

		// Calculate the sanity of the player
		float sanity = CalculateSanity();
	}

	private float CalculateSanity()
	{
		sanity -= Time.deltaTime * 0.1f; // Decrease sanity over time
		sanity = Mathf.Clamp(sanity, 0f, 100f); // Clamp sanity between 0 and 100
		onSanityChanged.Invoke(sanity);
		return sanity;
	}

	public float AddSanity(float amount)
	{
		sanity += amount; // Increase sanity
		sanity = Mathf.Clamp(sanity, 0f, 100f); // Clamp sanity between 0 and 100
		return sanity;
	}

	public float SubtractSanity(float amount)
	{
		sanity -= amount; // Decrease sanity
		sanity = Mathf.Clamp(sanity, 0f, 100f); // Clamp sanity between 0 and 100
		return sanity;
	}

	private void FixedUpdate()
	{
		// Move the character
		rb.linearVelocity = movement * moveSpeed;
	}

	private void PerformAction()
	{
		// Play action animation if animator exists
		if (animator != null)
		{
			animator.SetTrigger("Action");
		}

		// Add action logic here
		Debug.Log("Action performed!");
		// Play action animation
		PlayActionAnimation();
	}

	private void PlayActionAnimation()
	{
		// Play action animation if animator exists
		if (animator != null)
		{
			Debug.Log("Action animation triggered!");
			animator.SetTrigger("Action");
		}
	}
}
