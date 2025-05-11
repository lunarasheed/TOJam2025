using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public AudioSource audioSource; // Optional — only used if you prefer playing from this object
    public AudioClip[] pickupSounds;

    [Header("Floating Text Settings")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;

    private void Start()
    {
        // Assign AudioSource if not manually set
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("AudioSource not assigned and not found on pickup object.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add the item to the player's inventory
            InventoryManager.Instance.AddItem(this);

            // Play a random pickup sound using a temporary object
            if (pickupSounds != null && pickupSounds.Length > 0)
            {
                AudioClip randomClip = pickupSounds[Random.Range(0, pickupSounds.Length)];

                GameObject tempAudio = new GameObject("PickupSound");
                tempAudio.transform.position = transform.position;

                AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
                tempSource.clip = randomClip;
                tempSource.Play();

                Destroy(tempAudio, randomClip.length);
            }
            else
            {
                Debug.LogWarning("Pickup sounds not assigned.");
            }

            // Increase player sanity
            PlayerController2D playerController = other.GetComponent<PlayerController2D>();
            if (playerController != null)
            {
                playerController.AddSanity(10f);
                playerController.onSanityChanged.Invoke(playerController.CurrentSanity);

                SpawnFloatingText();

                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
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

    private void SpawnFloatingText()
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("Floating text prefab not assigned!");
            return;
        }

        GameObject floatingTextObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        if (floatingTextObj == null)
        {
            Debug.LogError("Failed to instantiate floating text prefab!");
            return;
        }

        Canvas canvas = floatingTextObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1, 1);

        StartCoroutine(AnimateFloatingText(floatingTextObj));
    }

    private System.Collections.IEnumerator AnimateFloatingText(GameObject textObj)
    {
        float alpha = 1f;
        Vector3 startPos = textObj.transform.position;

        while (alpha > 0f)
        {
            textObj.transform.position = new Vector3(
                startPos.x,
                startPos.y + (1f - alpha) * floatSpeed,
                startPos.z
            );

            alpha -= Time.deltaTime * fadeSpeed;
            CanvasRenderer canvasRenderer = textObj.GetComponentInChildren<CanvasRenderer>();
            if (canvasRenderer != null)
            {
                canvasRenderer.SetAlpha(alpha);
            }

            yield return null;
        }

        Destroy(gameObject);    // Destroy the pickup
        Destroy(textObj);       // Destroy the floating text
    }
}
