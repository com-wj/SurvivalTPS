using UnityEngine;

public class AimIKHandler : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Animator _animator;
	[SerializeField] private Transform _camTr;

	[Header("Head Look Settings")]
	[SerializeField, Range(0, 1)] private float overallLookAtWeight = 1.0f;
	[SerializeField, Range(0, 1)] private float bodyLookAtWeight = 0.15f;
	[SerializeField, Range(0, 1)] private float headLookAtWeight = 0.8f;
	[SerializeField, Range(0, 1)] private float eyesLookAtWeight = 1.0f;
	[SerializeField, Range(0, 1)] private float clampLookAtWeight = 0.5f;
	[SerializeField] private float lookAtTargetDistance = 10f;
	#endregion

	private void Awake()
	{
		if (_camTr == null)
		{
			_camTr = Camera.main.transform;
		}

		if (_animator == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지");
			gameObject.SetActive(false);
			return;
		}
	}

	void OnAnimatorIK(int layerIndex)
	{
		if (_animator == null || _camTr == null) return;

		if(layerIndex == 1)
		{
			Vector3 lookAtTargetPosition = _camTr.position + _camTr.forward * lookAtTargetDistance;
			_animator.SetLookAtWeight(overallLookAtWeight, bodyLookAtWeight, headLookAtWeight, eyesLookAtWeight, clampLookAtWeight);
			_animator.SetLookAtPosition(lookAtTargetPosition);
		}
	}
}