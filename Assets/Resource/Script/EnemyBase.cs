using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
	#region 인스펙터
	[SerializeField] private float _currentHp;

	[Header("디버그")]
	[SerializeField] private bool _printLog;
	#endregion

	#region 인스펙터
	private bool _isDead = false;
	#endregion

	public void TakeDamage(float damage)
	{
		if (_isDead) return;

		_currentHp -= damage;
		if (_printLog)
		{
			Debug.Log($"[{name}] {damage} 피해 입음. 남은 체력:{_currentHp}");
		}

		if (_currentHp < 0)
		{
			_currentHp = 0;
			Die();
		}
	}

	private void Die()
	{
		_isDead = true;
		if (_printLog)
		{
			Debug.Log($"[{name}] 사망");
		}
	}
}