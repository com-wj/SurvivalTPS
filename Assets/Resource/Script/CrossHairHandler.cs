using UnityEngine;

public class CrossHairHandler : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private PlayerShooter _playerShooter;
	[SerializeField] private Canvas _crossHairCanvas;
	[SerializeField] private SimpleCrosshair _crossHairGenerator;

	/*
	[Header("크로스헤어 세팅")]
	[SerializeField] private float _defaultGap = 10f;
	[SerializeField] private float _FireGap = 14f;
	*/
	#endregion

	#region 내부 변수
	
	#endregion

	private void Awake()
	{
		if (_playerController == null ||
			_playerShooter == null ||
			_crossHairCanvas == null ||
			_crossHairGenerator == null)
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
			_playerController.AimChanged += ToggleCrossHair;
		}
		if (_playerShooter != null)
		{
			//_playerShooter.Fire += OnFire;
		}
	}

	private void OnDisable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged -= ToggleCrossHair;
		}
		if (_playerShooter != null)
		{
			//_playerShooter.Fire -= OnFire;
		}
	}

	public void ToggleCrossHair(bool isAiming)
	{
		if (_crossHairCanvas == null) return;

		_crossHairCanvas.gameObject.SetActive(isAiming);
	}

	/*
	public void OnFire()
	{
		_crossHairGenerator.SetGap(_FireGap);
	}
	*/
}