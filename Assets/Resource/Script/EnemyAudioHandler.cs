using UnityEngine;

public class EnemyAudioHandler : MonoBehaviour
{
	[Header("공격 오디오")]
	[SerializeField] private AudioClip _attackClip;

	[Header("사망 오디오 목록")]
	[SerializeField] private AudioClip[] _deathClips;

	public void PlayAttackClip()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot(_attackClip, 1f);
		}
	}

	public void PlayDeathClipRandom()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShotRandom(_deathClips, 0.5f);
		}
	}
}