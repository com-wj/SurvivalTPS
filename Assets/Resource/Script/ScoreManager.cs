using System;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
	[SerializeField] private bool _printLog = false;

	private int _killCount;

	public event Action<int> KillCountChanged;

	public int KillCount => _killCount;

	public void AddKill(int count = 1)
	{
		_killCount += count;

		if (_printLog)
		{
			Debug.Log($"[{name}] {count}킬. 누적 {_killCount}킬");
		}

		KillCountChanged?.Invoke(_killCount);
	}

	public void ResetKillCount()
	{
		_killCount = 0;
	}
}
