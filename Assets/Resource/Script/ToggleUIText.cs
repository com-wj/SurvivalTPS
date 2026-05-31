using UnityEngine;

public class ToggleUIText : MonoBehaviour
{
	[SerializeField] private GameObject _targetUI;
	[SerializeField] private KeyCode _toggleKey;

	private void Awake()
	{
		if (_targetUI == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(_toggleKey))
		{
			bool targetEnable = !_targetUI.activeSelf;
			_targetUI.SetActive(targetEnable);
		}
	}
}