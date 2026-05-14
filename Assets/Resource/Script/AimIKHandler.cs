using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimIKHandler : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private Rig _aimRig;
	[SerializeField] private float _aimSharpness = 10f;
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
	}

	private void Update()
	{
		UpdateAimIKWeigth();
	}

	// 상체 애니메이션 리깅 가중치 갱신.
	private void UpdateAimIKWeigth()
	{
		if (_playerController == null) return;

		float targetWeight = _playerController.IsAiming ? 1f : 0f;

		float t = 1f - Mathf.Exp(-_aimSharpness * Time.deltaTime);
		_aimRig.weight = Mathf.Lerp(_aimRig.weight, targetWeight, t);
	}
}