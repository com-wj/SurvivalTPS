using UnityEngine;

public class RandomFootSound : MonoBehaviour
{
	[Header("랜덤 재생 발소리 목록")]
	[SerializeField] private AudioClip[] clips;

	public void OnFootstepEvent()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShotRandom(clips, 0.2f, 0.85f, 1.15f);
		}
	}
}