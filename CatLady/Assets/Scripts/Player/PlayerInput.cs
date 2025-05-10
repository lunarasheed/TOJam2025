/**
 * PlayerInput.cs
 *
 * A C# character input controller for a Unity 2D URP game engine.
 */

using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Header("Input Settings")]
	[SerializeField] private KeyCode actionKey = KeyCode.Space;

	public event Action OnActionPerformed;

	private Vector2 _movementInput;
	private bool _actionPerformed;

	public Vector2 MovementInput => _movementInput;
	public bool ActionPerformed => _actionPerformed;

	private void Update()
	{
		// Get horizontal and vertical inputs
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");

		// Normalize the movement vector to prevent faster diagonal movement
		_movementInput = new Vector2(horizontalInput, verticalInput).normalized;

		// Check for action button press
		if (Input.GetKeyDown(actionKey))
		{
			_actionPerformed = true;
			OnActionPerformed?.Invoke();
		}
		else
		{
			_actionPerformed = false;
		}
	}
}
