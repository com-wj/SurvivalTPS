using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private CharacterController _controller;
	[SerializeField] private PlayerAnimator _playerAnimator; // cs

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

	[Header("디버그")]
	[SerializeField] bool ForceAiming;
	#endregion

	#region 내부 변수
	private Transform _camTr;

	private Vector3 _horizontalVel; // 수평(xz) 속도
	private float _verticalVel; // 수직 속도(y)
	#endregion

	private void Awake()
	{
		if (_controller == null ||
			_playerAnimator == null)
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

		bool isAiming = ForceAiming || Input.GetMouseButton(1);
		bool isRunning = 
			(
			Input.GetKey(_runKey) &&
			!isAiming &&
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
			speed *= isAiming ? _aimingMoveSpeedMultiplier : 1f;
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
		h *= !isAiming && isRunning ? 2f : 1f;
		v *= !isAiming && isRunning ? 2f : 1f;
		_playerAnimator.OnMove(h, v);
		_playerAnimator.UpdateAirParam(_controller.isGrounded, _verticalVel);

		_playerAnimator.OnAim(isAiming);
		if (isAiming)
		{
			if (Input.GetMouseButton(0))
			{
				_playerAnimator.OnFire();
			}
		}
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
}