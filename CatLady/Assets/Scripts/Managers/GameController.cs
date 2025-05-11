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
	[SerializeField] private PlayerController2D player;
	[SerializeField] private CanvasGroup fadeCanvasGroup;

	private AudioSource audioSource;
	private bool isGameOver = false;

	private void Start()
	{
		// Find player if not assigned
		if (player == null)
		{
			player = FindAnyObjectByType<PlayerController2D>();
			if (player == null)
			{
				Debug.LogError("No PlayerController2D found in scene!");
				return;
			}
		}

		// Listen for sanity changes
		player.onSanityChanged.AddListener(CheckPlayerSanity);

		// Setup audio
		audioSource = gameObject.AddComponent<AudioSource>();
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
			player.enabled = false;
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
	}

	private void OnDestroy()
	{
		if (player != null)
		{
			player.onSanityChanged.RemoveListener(CheckPlayerSanity);
		}
	}
}
