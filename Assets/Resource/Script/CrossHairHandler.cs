using UnityEngine;

public class CrossHairHandler : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private Canvas _crossHairCanvas;
	#endregion

	private void Awake()
	{
		if (_playerController == null ||
			_crossHairCanvas == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		_crossHairCanvas.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		if (_playerController != null)
		{
			_playerController.OnAimChanged += ToggleCrossHair;
		}
	}

	private void OnDisable()
	{
		if (_playerController != null)
		{
			_playerController.OnAimChanged -= ToggleCrossHair;
		}
	}

	public void ToggleCrossHair(bool isAiming)
	{
		if (_crossHairCanvas == null) return;

		_crossHairCanvas.gameObject.SetActive(isAiming);
	}
}