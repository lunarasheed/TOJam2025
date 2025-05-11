using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

	[SerializeField] private GameObject pickupPrefab;
	[SerializeField] private GameObject scoreCanvas;

	private AudioSource audioSource;
	private PlayerController2D playerInstance;
	private bool isGameOver = false;
	private bool spawnedPickup = false;
	private int score = 0;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		isGameOver = false;
		score = 0;
		spawnedPickup = false;

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

	private void Update()
	{
		UpdateScore();
	}

	private void UpdateScore()
	{
		if (!isGameOver)
		{
			score += (int)(Time.deltaTime * 10);
		}
		else
		{
			score = 0; // Reset score on game over
		}

		UpdateScoreUI();
	}

	private void UpdateScoreUI()
	{
		// If the ScoreUI object is not assigned, spawn a new CanvasGroup with a Text component
		if (scoreCanvas == null)
		{
			// Search for an existing Canvas in the scene
			Canvas canvas = FindAnyObjectByType<Canvas>();
			if (canvas == null)
			{
				Debug.LogError("No Canvas found in the scene!");
				return;
			}
			scoreCanvas = canvas.GetComponentInParent<GameObject>();
			if (scoreCanvas == null)
			{
				Debug.LogError("No GameObject found with a Canvas component!");
				return;
			}
			GameObject textObj = new GameObject("ScoreText");
			textObj.transform.SetParent(scoreCanvas.transform);
			TextMeshPro scoreText = textObj.AddComponent<TextMeshPro>();
			scoreText.fontSize = 24;
			scoreText.color = Color.white;
			scoreText.alignment = TextAlignmentOptions.Center;
			scoreText.text = "Score: " + score;
			scoreText.font = Resources.Load<TMP_FontAsset>("ZCOOLKuaiLe-Regular SDF");
			scoreText.fontSharedMaterial = Resources.Load<Material>("ZCOOLKuaiLe-Regular SDF");
			// move score canvas to the top left corner
			RectTransform rectTransform = scoreCanvas.GetComponent<RectTransform>();
			rectTransform.position = new Vector3(10, Screen.height - 10, 0);
		}
	}

	private void CheckPlayerSanity(float sanity)
	{
		UpdateScore();

		if (sanity <= 0f && !isGameOver)
		{
			StartCoroutine(HandleGameOver());
		}

		// if the player sanity is low, spawn a pickup
		if (sanity <= 20f && !spawnedPickup)
		{
			spawnedPickup = true;
			// Spawn a pickup
			SpawnPickup();
		}
		else if (sanity > 20f && spawnedPickup)
		{
			spawnedPickup = false;
		}

		// if player sanity is below 45, and there are no pickups, spawn a pickup
		int pickupCount = GameObject.FindGameObjectsWithTag("Pickup").Length;
		if (sanity <= 45f && pickupCount == 0)
		{
			spawnedPickup = true;
			// Spawn a pickup
			SpawnPickup();
		}
		else if (sanity > 45f && spawnedPickup)
		{
			spawnedPickup = false;
		}
	}


	public void SpawnPickup()
	{
		if (pickupPrefab != null && player != null)
		{
			// Get a list of nearby floor tiles
			GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("FloorTile");
			if (floorTiles.Length == 0)
			{
				Debug.LogError("No floor tiles found in the scene!");
				return;
			}
			// Randomly select a floor tile
			int randomIndex = Random.Range(0, floorTiles.Length);
			GameObject randomFloorTile = floorTiles[randomIndex];
			Vector3 spawnPosition = randomFloorTile.transform.position;

			// Instantiate the pickup prefab at the spawn position
			GameObject pickup = Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
			if (pickup == null)
			{
				Debug.LogError("Pickup prefab is not assigned!");
				return;
			}
		}
		else
		{
			Debug.LogError("Pickup prefab or player is not assigned!");
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

		// Destroy all objects in the current scene
		DestroyAllObjectsInScene();
		yield return new WaitForSeconds(0.1f);

		// Unload current scene
		Scene currentScene = SceneManager.GetActiveScene();
		if (currentScene != null)
		{
			yield return SceneManager.UnloadSceneAsync(currentScene);
		}
	}

	public void DestroyAllObjectsInScene()
	{
		// Destroy player
		if (player != null)
		{
			Destroy(player);
		}
		// Destroy all followers
		GameObject[] followers = GameObject.FindGameObjectsWithTag("Follower");
		foreach (GameObject follower in followers)
		{
			Destroy(follower);
		}
		// Destroy all pickups
		GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
		foreach (GameObject pickup in pickups)
		{
			Destroy(pickup);
		}
		// Destroy all interactables
		GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
		foreach (GameObject interactable in interactables)
		{
			Destroy(interactable);
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
