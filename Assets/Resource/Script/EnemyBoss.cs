using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private EnemyBase _enemyBase;
	#endregion

	private void OnEnable()
	{
		if (_enemyBase != null)
		{
			_enemyBase.Dead += OnDead;
		}
	}

	private void OnDisable()
	{
		if (_enemyBase != null)
		{
			_enemyBase.Dead -= OnDead;
		}
	}

	private void OnDead()
	{
		if (RoundManager.Instance != null)
		{
			RoundManager.Instance.OnRoundClear();
		}
	}
}