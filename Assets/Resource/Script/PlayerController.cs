using System;
using UnityEngine;

/// <summary>
/// 입력에 따른 캐릭터 트랜스폼 조작과 파라미터 전송
/// </summary>
public class PlayerController : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private CharacterController _controller;
	[SerializeField] private PlayerAnimator _playerAnimator; // cs
	[SerializeField] private PlayerShooter _playerShooter; // cs

	[Header("캐릭터 회전")]
	[SerializeField] private CameraController _cameraController; // cs
	[SerializeField] private Transform _characterMeshTr;
	[SerializeField] private Vector3 _rotateOffset;
	[SerializeField] private float _aimRotSharpness = 10f;

	[Header("이동 속도")]
	[SerializeField] private float _walkSpeed = 5.0f;
	[SerializeField] private float _landingMoveSpeedMultiplier = 0f;
	[SerializeField] private float _aimingMoveSpeedMultiplier = 0.5f;
	[SerializeField] private float _sideMoveSpeedMultiplier = 0.5f;

	[Header("달리기")]
	[SerializeField] KeyCode _runKey = KeyCode.LeftShift;
	[SerializeField] private float _runMultiplier = 1.8f;

	[Header("점프")]
	[SerializeField] KeyCode _jumpKey = KeyCode.Space;
	[SerializeField] private float _jumpHeight = 1.2f;
	[SerializeField] private float _gravity = -9.81f;
	[SerializeField] private float _groundStick = -2.0f;

	[Header("조준")]
	[SerializeField] private bool _isAiming = false;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	[SerializeField] private bool _forceAiming;
	#endregion

	#region 내부 변수
	private Transform _camTr;

	private Vector3 _horizontalVel; // 수평(xz) 속도
	private float _verticalVel; // 수직 속도(y)

	private bool _isAimingSequence = false;
	#endregion

	//public bool IsAiming => _isAiming;

	public event Action<bool> AimChanged; // 조준 상태 변화 이벤트

	private void Awake()
	{
		if (_controller == null ||
			_playerAnimator == null ||
			_playerShooter == null ||
			_characterMeshTr == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		_camTr = Camera.main.transform;
	}

	private void Update()
	{
		TickGravity();
		Move();
	}

	private void TickGravity()
	{
		if (_controller.isGrounded)
		{
			if (_verticalVel < 0.0f)
			{
				_verticalVel = _groundStick;
			}
		}
		else
		{
			_verticalVel += _gravity * Time.deltaTime;
		}
	}

	private void Move()
	{
		// 입력
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		Vector3 input = new Vector3(h, 0, v);
		input = Vector3.ClampMagnitude(input, 1.0f);

		bool isSideMove = (h != 0) && (v == 0);

		bool currentAimInput = _forceAiming || Input.GetMouseButton(1);
		if (_isAiming != currentAimInput)
		{
			_isAiming = currentAimInput;
			_isAimingSequence = true;
			if (_printLog)
			{
				Debug.Log($"[{name}] 조준 시퀀스 활성화.");
			}
			AimChanged?.Invoke(_isAiming);
		}

		bool isRunning = 
			(
			Input.GetKey(_runKey) &&
			!_isAiming &&
			v > 0 &&
			!isSideMove
			);
		bool isjumpKeyDown = Input.GetKeyDown(_jumpKey);

		bool isLanding = _playerAnimator.IsLanding();

		if (_controller.isGrounded)
		{
			// 이동속도 계산
			Vector3 moveDir = (input.sqrMagnitude > 0.0001f) ? BuildMoveDirection(input) : Vector3.zero;

			float speed = _walkSpeed;
			speed *= isLanding ? _landingMoveSpeedMultiplier : 1f;
			speed *= isRunning ? _runMultiplier : 1f;
			speed *= _isAiming ? _aimingMoveSpeedMultiplier : 1f;
			speed *= isSideMove ? _sideMoveSpeedMultiplier : 1f;

			_horizontalVel = moveDir * speed;

			if (!isLanding && isjumpKeyDown)
			{
				TryJump();
			}
		}

		Vector3 _finalVelocity = _horizontalVel;
		_finalVelocity.y = _verticalVel; // 중력 적용

		_controller.Move(_finalVelocity * Time.deltaTime);

		// 파라미터 적용
		h *= !_isAiming && isRunning ? 2f : 1f;
		v *= !_isAiming && isRunning ? 2f : 1f;
		_playerAnimator.OnMove(h, v);
		_playerAnimator.UpdateAirParam(_controller.isGrounded, _verticalVel);

		RotateCharacter();
	}
	
	// 이동 방향 설정
	private Vector3 BuildMoveDirection(Vector3 input)
	{
		Vector3 camF = Vector3.ProjectOnPlane(_camTr.forward, Vector3.up).normalized;
		Vector3 camR = Vector3.ProjectOnPlane(_camTr.right, Vector3.up).normalized;

		Vector3 dir = camF * input.z + camR * input.x;

		return dir.normalized;
	}

	private void TryJump()
	{
		if (_controller.isGrounded)
		{
			_verticalVel = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);

			_playerAnimator.OnJump();
		}
	}

	// 이동 중이거나 조준 중 캐릭터 메쉬 회전
	private void RotateCharacter()
	{
		if (_cameraController == null) return;
		if (_characterMeshTr == null) return;

		bool isMoving = (_horizontalVel.sqrMagnitude > 0.0001f); // 이동 중
		float targetYaw = _cameraController.CurrentYaw;

		if (_isAimingSequence) // 조준 시퀀스
		{
			Vector3 Rot = new Vector3(0, targetYaw, 0);
			Rot += _isAiming ? _rotateOffset : Vector3.zero;

			Quaternion targetRot = Quaternion.Euler(Rot);

			Quaternion currentRot = _characterMeshTr.localRotation;

			float t = 1f - Mathf.Exp(-_aimRotSharpness * Time.deltaTime);
			_characterMeshTr.localRotation = Quaternion.Slerp(currentRot, targetRot, t);

			// 보간 종료
			if (Quaternion.Angle(_characterMeshTr.localRotation, targetRot) < 0.1f)
			{
				_characterMeshTr.localRotation = targetRot;
				_isAimingSequence = false;
			}

			return;
		}

		if (_isAiming || isMoving) // 조준 중이거나, 이동 중이면
		{
			Vector3 targetRot = new Vector3(0, targetYaw, 0);
			targetRot += _isAiming ? _rotateOffset : Vector3.zero; // 조준 중 오프셋 보정

			_characterMeshTr.localRotation = Quaternion.Euler(targetRot);
		}
	}
}