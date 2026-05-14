using UnityEngine;

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
	private Vector3 _currentTargetPos;
	#endregion

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

	private void Update()
	{
		if (_playerController.IsAiming)
		{
			UpdateTargetPoint();
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

	public void TryShoot()
	{
		if (_currentGun == null)
		{
			Debug.LogWarning($"[{name}] 장착중인 총기가 없습니다.");
			return;
		}

		if (!_currentGun.CanFire) return;

		_currentGun.OnFire(_currentTargetPos);
	}
}