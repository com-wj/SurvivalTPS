using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform _rotateRootTr;

	[SerializeField] private float _sensitivity = 2.0f;

	[SerializeField] private float _startPitch = 0f;
	[SerializeField] private float _minPitch = -30f;
	[SerializeField] private float _maxPitch = 45f;

	private float _xRotation = 0f;

	private void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		transform.localRotation = Quaternion.Euler(_startPitch, 0, 0);
	}


	private void LateUpdate()
	{
		float mx = Input.GetAxis("Mouse X") * _sensitivity;
		float my = Input.GetAxis("Mouse Y") * _sensitivity;

		// 수평 회전(부모 회전)
		_rotateRootTr.Rotate(Vector3.up * mx);

		// 수직 회전
		_xRotation -= my;
		_xRotation = Mathf.Clamp(_xRotation, _minPitch, _maxPitch);
		transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
	}
}