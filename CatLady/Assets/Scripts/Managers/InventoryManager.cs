using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events; // Add this line

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager Instance { get; private set; }

	[SerializeField]
	private int maxInventorySize = 20;

	private List<Pickup> inventory = new List<Pickup>();

	// Add to InventoryManager.cs class definition
	public UnityEvent<Pickup> onItemPickedUp = new UnityEvent<Pickup>();

	private void Awake()
	{
		// Singleton pattern implementation
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public bool AddItem(Pickup item)
	{
		if (inventory.Count >= maxInventorySize)
		{
			Debug.LogWarning("Inventory is full!");
			return false;
		}

		inventory.Add(item);
		onItemPickedUp.Invoke(item);  // Add this line
		Debug.Log($"Added item to inventory. Current count: {inventory.Count}");
		return true;
	}

	public bool RemoveItem(Pickup item)
	{
		return inventory.Remove(item);
	}

	public List<Pickup> GetInventory()
	{
		return inventory;
	}

	public bool HasItem(Pickup item)
	{
		return inventory.Contains(item);
	}

	public void ClearInventory()
	{
		inventory.Clear();
		Debug.Log("Inventory cleared");
	}
}