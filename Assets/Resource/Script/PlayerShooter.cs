using System;
using UnityEngine;

/// <summary>
/// 조준과 사격 관련 처리 로직
/// </summary>
public class PlayerShooter : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private Gun _currentGun; // 장착한 총기

	[Header("조준될 위치")]
	[SerializeField] private Transform _aimTarget;

	[Header("Raycast Setting")]
	[SerializeField] private LayerMask _targetLayer;
	[SerializeField] private float _maxDistance = 100f;
	#endregion

	#region 내부 변수
	private bool _isAiming;

	private Vector3 _currentTargetPos;
	#endregion

	public bool CanAim
	{
		get
		{
			if (_currentGun != null && _currentGun.IsReloading)
				return false;
			
			return true;
		}
	}

	public event Action Fire; // 격발 이벤트
	public event Action<float> Reload; // 장전 이벤트.

	private void Awake()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}

		if (_mainCamera == null ||
			_aimTarget == null ||
			_playerController == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged += OnAimChanged;
		}
	}

	private void OnDisable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged -= OnAimChanged;
		}
	}

	public void OnAimChanged(bool isAiming)
	{
		_isAiming = isAiming;
	}

	public void EquipWeapon(Gun newGunPrefab)
	{
		if (_currentGun != null)
		{
			
		}
	}

	private void Update()
	{
		bool mouseLeftButton = Input.GetMouseButton(0);
		bool reloadInput = Input.GetKeyDown(KeyCode.R);

		if (reloadInput) // 재장전 입력 시 장전
		{
			TryReload();
			return;
		}

		if (_isAiming)
		{
			UpdateTargetPoint();

			if (mouseLeftButton)
			{
				TryShoot();
			}
			else if(_currentGun != null && _currentGun.IsEmptyAmmo) // 조준 중 좌클릭이 떼졌을 때 자동 장전
			{
				TryReload();
				return;
			}
		}
		else if (_currentGun != null && _currentGun.IsEmptyAmmo) // 비조준 중 탄약이 없으면 자동 장전
		{
			TryReload();
			return;
		}
	}

	private void UpdateTargetPoint()
	{
		// 목표 지점 계산
		// 조준 위치를 카메라 화면 중앙 조준점과 일치시키기 위함
		Ray cameraRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 카메라 중앙 레이

		if (Physics.Raycast(cameraRay, out RaycastHit hit, _maxDistance, _targetLayer))
		{
			_currentTargetPos = hit.point;
		}
		else
		{
			_currentTargetPos = cameraRay.GetPoint(_maxDistance);
		}

		if (_aimTarget != null)
		{
			_aimTarget.position = _currentTargetPos;
		}
	}

	private void TryShoot()
	{
		if (_currentGun == null)
		{
			Debug.LogWarning($"[{name}] 장착중인 총기가 없습니다.");
			return;
		}

		if (!_currentGun.CanFire) return;

		_currentGun.OnFire(_currentTargetPos);
		Fire?.Invoke();
	}

	// 자동 장전
	private void TryReload()
	{
		if (_currentGun == null) return;

		if(_currentGun.IsReloading) return;

		if (!_currentGun.CanReload) return;

		float AnimSpeedMultiplier = _currentGun.OnReload();
		if (AnimSpeedMultiplier <= 0f)
		{
			Debug.LogWarning($"[{name}] 재장전 애니메이션 속도 이상. {AnimSpeedMultiplier}");
		}
		Reload?.Invoke(AnimSpeedMultiplier);
	}
}