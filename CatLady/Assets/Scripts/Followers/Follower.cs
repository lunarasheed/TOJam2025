using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour
{
	[Header("Following Settings")]
	[SerializeField] private float moveSpeed = 3f;
	[SerializeField] private float minDistanceToPlayer = 1f;
	[SerializeField] private float maxDistanceToPlayer = 5f;

	[Header("Meowing Settings")]
	[SerializeField] private float meowInterval = 5f;
	[SerializeField] private float sanityLossPerMeow = 5f;
	[SerializeField] private AudioSource meowSound;

	private Transform playerTransform;
	private PlayerController2D playerController;
	private bool isSated = false;
	private float satedTimeRemaining = 0f;
	private const float SATED_DURATION = 30f;
	private float nextMeowTime = 0f;

	private void Start()
	{
		// Find the player
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			playerTransform = player.transform;
			playerController = player.GetComponent<PlayerController2D>();
		}
		else
		{
			Debug.LogError("Player not found! Make sure player has 'Player' tag.");
		}

		// Listen for pickup events
		InventoryManager.Instance.onItemPickedUp.AddListener(OnPlayerPickedUpItem);
	}

	private void Update()
	{
		if (playerTransform == null) return;

		// Update sated status
		if (isSated)
		{
			satedTimeRemaining -= Time.deltaTime;
			if (satedTimeRemaining <= 0f)
			{
				isSated = false;
			}
		}

		// Only meow and follow if not sated
		if (!isSated)
		{
			FollowPlayer();
			TryMeow();
		}
	}

	private void FollowPlayer()
	{
		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		// Only move if we're outside the minimum distance but within max distance
		if (distanceToPlayer > minDistanceToPlayer && distanceToPlayer < maxDistanceToPlayer)
		{
			Vector2 direction = (playerTransform.position - transform.position).normalized;
			transform.position = Vector2.MoveTowards(
				transform.position,
				playerTransform.position,
				moveSpeed * Time.deltaTime
			);
		}
	}

	private void TryMeow()
	{
		if (Time.time >= nextMeowTime)
		{
			Meow();
			nextMeowTime = Time.time + meowInterval;
		}
	}

	private void Meow()
	{
		if (playerController != null)
		{
			// Play meow sound if assigned
			if (meowSound != null)
			{
				meowSound.Play();
			}

			// Reduce player sanity
			float currentSanity = playerController.CurrentSanity;
			playerController.SubtractSanity(sanityLossPerMeow);
			playerController.onSanityChanged.Invoke(playerController.CurrentSanity);
		}
	}

	private void OnPlayerPickedUpItem(Pickup pickup)
	{
		isSated = true;
		satedTimeRemaining = SATED_DURATION;
	}

	private void OnDestroy()
	{
		// Clean up event listener
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.onItemPickedUp.RemoveListener(OnPlayerPickedUpItem);
		}
	}
}
