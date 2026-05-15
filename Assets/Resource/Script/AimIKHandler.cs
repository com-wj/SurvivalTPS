using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimIKHandler : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private Rig _aimRig;
	[SerializeField] private float _aimSharpness = 10f;
	#endregion

	#region 내부 변수
	private float _targetWeight = 0f;
	#endregion

	private void Awake()
	{
		if (_playerController == null ||
			_aimRig == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지");
			gameObject.SetActive(false);
			return;
		}

		_aimRig.weight = _targetWeight;
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

	private void Update()
	{
		UpdateRigWeigth();
	}

	private void OnAimChanged(bool isAiming)
	{
		_targetWeight = isAiming ? 1f : 0f;
	}

	// 상체 애니메이션 리깅 가중치 갱신.
	private void UpdateRigWeigth()
	{
		if (_aimRig.weight == _targetWeight) return;

		float t = 1f - Mathf.Exp(-_aimSharpness * Time.deltaTime);
		_aimRig.weight = Mathf.Lerp(_aimRig.weight, _targetWeight, t);

		if (Mathf.Abs(_aimRig.weight - _targetWeight) < 0.001f)
		{
			_aimRig.weight = _targetWeight;
		}
	}
}