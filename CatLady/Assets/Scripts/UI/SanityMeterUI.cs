using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanityMeterUI : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private PlayerController2D player;
	[SerializeField] private Slider sanitySlider;
	[SerializeField] private TextMeshProUGUI sanityText;
	[SerializeField] private Image fillImage;

	[Header("Color Settings")]
	[SerializeField] private Color highSanityColor = Color.green;
	[SerializeField] private Color mediumSanityColor = Color.yellow;
	[SerializeField] private Color lowSanityColor = Color.red;
	[SerializeField] private float mediumSanityThreshold = 60f;
	[SerializeField] private float lowSanityThreshold = 30f;

	private void Start()
	{
		if (player == null)
			player = FindObjectOfType<PlayerController2D>();

		if (player != null)
		{
			player.onSanityChanged.AddListener(UpdateSanityMeter);
			InitializeSanityMeter();
		}
		else
		{
			Debug.LogError("PlayerController2D not found!");
		}
	}

	private void InitializeSanityMeter()
	{
		if (sanitySlider != null)
		{
			sanitySlider.minValue = 0f;
			sanitySlider.maxValue = 100f;
			UpdateSanityMeter(player.CurrentSanity);
		}
	}

	private void UpdateSanityMeter(float sanityValue)
	{
		if (sanitySlider != null)
			sanitySlider.value = sanityValue;

		if (sanityText != null)
			sanityText.text = $"Sanity: {Mathf.RoundToInt(sanityValue)}%";

		if (fillImage != null)
			UpdateFillColor(sanityValue);
	}

	private void UpdateFillColor(float sanityValue)
	{
		Color newColor;
		if (sanityValue > mediumSanityThreshold)
			newColor = highSanityColor;
		else if (sanityValue > lowSanityThreshold)
			newColor = mediumSanityColor;
		else
			newColor = lowSanityColor;

		fillImage.color = newColor;
	}
}
