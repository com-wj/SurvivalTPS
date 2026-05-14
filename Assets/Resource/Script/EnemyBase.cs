using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
	[SerializeField] private float _currentHp;
	[SerializeField] private bool _printLog;

	private bool _isDead = false;

	public void TakeDamage(float damage)
	{
		if (_isDead) return;

		_currentHp -= damage;
		if (_printLog)
		{
			Debug.Log($"[{name}] {damage} 피해 입음.");
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