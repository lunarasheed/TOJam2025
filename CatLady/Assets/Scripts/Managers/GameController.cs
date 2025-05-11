using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
	[Header("Game Over Settings")]
	[SerializeField] private float gameOverDelay = 2f;
	[SerializeField] private string gameOverSceneName = "GameOver";
	[SerializeField] private AudioClip gameOverSound;

	[Header("References")]
	[SerializeField] private GameObject player;
	[SerializeField] private Transform playerSpawnPoint;
	[SerializeField] private GameObject followerPrefab;
	[SerializeField] private Transform followerSpawnPoint;
	[SerializeField] private CanvasGroup fadeCanvasGroup;

	private AudioSource audioSource;
	private PlayerController2D playerInstance;
	private bool isGameOver = false;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		isGameOver = false;

		// Instantiate player at spawn point
		player = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
		if (player == null)
		{
			Debug.LogError("Player prefab is not assigned!");
			return;
		}

		playerInstance = player.GetComponent<PlayerController2D>();
		if (playerInstance == null)
		{
			Debug.LogError("PlayerController2D component not found on player prefab!");
			return;
		}

		// Reset player state
		if (player != null)
		{
			playerInstance.enabled = true;
			playerInstance.ResetSanity();
		}

		// Instantiate follower at spawn point
		if (followerPrefab != null && followerSpawnPoint != null)
		{
			GameObject follower = Instantiate(followerPrefab, followerSpawnPoint.position, Quaternion.identity);
			if (follower == null)
			{
				Debug.LogError("Follower prefab is not assigned!");
			}
		}
		else
		{
			Debug.LogError("Follower prefab or spawn point is not assigned!");
		}

		// Reset fade canvas
		if (fadeCanvasGroup != null)
		{
			fadeCanvasGroup.alpha = 0f;
		}

		// Reset inventory
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.ClearInventory();
		}

		// Setup event listeners
		playerInstance.onSanityChanged.AddListener(CheckPlayerSanity);

		// Setup audio
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		Debug.Log("Game initialized");
	}

	private void CheckPlayerSanity(float sanity)
	{
		if (sanity <= 0f && !isGameOver)
		{
			StartCoroutine(HandleGameOver());
		}
	}

	private IEnumerator HandleGameOver()
	{
		isGameOver = true;

		// Play game over sound
		if (gameOverSound != null)
		{
			audioSource.PlayOneShot(gameOverSound);
		}

		// Freeze player movement
		if (player != null)
		{
			playerInstance.enabled = false;
		}

		// Fade to black
		if (fadeCanvasGroup != null)
		{
			float elapsedTime = 0f;
			while (elapsedTime < gameOverDelay)
			{
				elapsedTime += Time.deltaTime;
				fadeCanvasGroup.alpha = elapsedTime / gameOverDelay;
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(gameOverDelay);
		}

		// Load game over scene
		SceneManager.LoadScene(gameOverSceneName);

		// Unload current scene
		Scene currentScene = SceneManager.GetActiveScene();
		if (currentScene != null)
		{
			yield return SceneManager.UnloadSceneAsync(currentScene);
		}
	}

	private void OnDestroy()
	{
		if (player != null)
		{
			playerInstance.onSanityChanged.RemoveListener(CheckPlayerSanity);
		}
	}
}
