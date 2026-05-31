using Cinemachine;
using UnityEngine;

public class CameraPriorityHandler : MonoBehaviour
{
	#region 인스펙터
	[Header("의존성")]
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private PlayerBase _playerBase;

	[Header("가상 카메라")]
	[SerializeField] private CinemachineVirtualCamera _normalCam;
	[SerializeField] private CinemachineVirtualCamera _aimCam;
	[SerializeField] private CinemachineVirtualCamera _deathCam;
	[SerializeField] private int _enablePriority = 10;
	[SerializeField] private int _disablePriority = 5;

	[Header("카메라 앵커")]
	[SerializeField] private CameraController _normalAnchor;
	[SerializeField] private CameraController _aimAnchor;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	private void Awake()
	{
		if (_playerController == null ||
			_playerBase == null ||
			_normalCam == null ||
			_aimCam == null ||
			_deathCam == null ||
			_normalAnchor == null ||
			_aimAnchor == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged += ChangeCamera;
		}
		if (_playerBase != null)
		{
			_playerBase.Dead += OnPlayerDead;
		}
	}

	private void OnDisable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged -= ChangeCamera;
		}
		if (_playerBase != null)
		{
			_playerBase.Dead -= OnPlayerDead;
		}
	}

	private void ChangeCamera(bool isAiming)
	{
		if (_normalCam == null ||
			_aimCam == null ||
			_normalAnchor == null ||
			_aimAnchor == null) return;

		if (_printLog)
		{
			Debug.Log($"[{name}] 조준 카메라 : {isAiming}");
		}

		if (isAiming) // 조준 카메라 활성화
		{
			_normalCam.Priority = _disablePriority;
			_aimCam.Priority = _enablePriority;

			_aimAnchor.CameraPitchUpdate(_normalAnchor);
		}
		else
		{
			_normalCam.Priority = _enablePriority;
			_aimCam.Priority = _disablePriority;

			_normalAnchor.CameraPitchUpdate(_aimAnchor);
		}
	}

	private void OnPlayerDead()
	{
		if (_deathCam == null) return;

		_deathCam.Priority = 100;

		if (_printLog)
		{
			Debug.Log($"[{name}] 사망 카메라 활성화");
		}
	}
}