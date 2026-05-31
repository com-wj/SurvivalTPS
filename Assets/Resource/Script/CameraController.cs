using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region 인스펙터
	[Header("회전 감도")]
	[SerializeField] private float _sensitivity = 2.0f;

	[Header("각도 설정")]
	[SerializeField] private float _startPitch = 0f;
	[SerializeField] private float _startYaw = -20f;
	[SerializeField] private float _minPitch = -30f;
	[SerializeField] private float _maxPitch = 45f;

	[Header("디버그")]
	[SerializeField] private bool _mouseVisible = false;
	[SerializeField] private bool _printLog = false;

	[Header("의존성")]
	[SerializeField] private PlayerBase _playerBase;
	#endregion

	#region 내부 변수
	private float _pitch; // x
	private float _yaw; // y
	#endregion

	public float CurrentYaw => _yaw;
	public float CurrentPitch => _pitch;

	private void Awake()
	{
		if (_playerBase == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지");
			gameObject.SetActive(false);
			return;
		}

		if (!_mouseVisible)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		transform.localRotation = Quaternion.Euler(_startPitch, _startYaw, 0);

		_yaw = _startYaw;
		_pitch = _startPitch;
	}

	private void LateUpdate()
	{
		if (Cursor.lockState != CursorLockMode.Locked) return; // 일시 정지면 반환

		if (_playerBase.IsDead) return;

		float mx = Input.GetAxis("Mouse X") * _sensitivity;
		float my = Input.GetAxis("Mouse Y") * _sensitivity;

		_yaw += mx;

		_pitch -= my;
		_pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch); // 상하 제한

		transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
	}

	public void CameraPitchUpdate(CameraController prevCam)
	{
		_pitch = Mathf.Clamp(prevCam.CurrentPitch, _minPitch, _maxPitch);
		if (_printLog)
		{
			Debug.Log($"[{name}] Pitch 업데이트 : {prevCam.CurrentPitch} → {_pitch}");
		}

		transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
	}
}