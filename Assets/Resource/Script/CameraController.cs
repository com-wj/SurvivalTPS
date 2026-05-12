using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region 인스펙터
	[Header("회전체 루트")]
	[SerializeField] private Transform _rotateRootTr;

	[Header("회전 감도")]
	[SerializeField] private float _sensitivity = 2.0f;

	[Header("각도 설정")]
	[SerializeField] private float _startPitch = 0f;
	[SerializeField] private float _startYaw = -20f;
	[SerializeField] private float _minPitch = -30f;
	[SerializeField] private float _maxPitch = 45f;

	[Header("디버그")]
	[SerializeField] private bool _mouseVisible = false;
	#endregion

	#region 내부 변수
	private float _pitch; // x
	private float _yaw; // y
	#endregion

	public float CurrentYaw => _yaw;

	private void Awake()
	{
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
		float mx = Input.GetAxis("Mouse X") * _sensitivity;
		float my = Input.GetAxis("Mouse Y") * _sensitivity;

		_yaw += mx;

		_pitch -= my;
		_pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch); // 상하 제한

		transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
	}
}