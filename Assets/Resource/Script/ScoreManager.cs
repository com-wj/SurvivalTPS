using System;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
	#region 인스펙터
	[SerializeField] private int _killCount;
	[SerializeField] private int _totalKillCount;

	[SerializeField] private float _survivalTime;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region 내부 변수
	private bool _isTimerRunning = false;

	private bool _isSurvive = false;
	#endregion

	public event Action<int> KillCountChanged;

	public int KillCount => _killCount;
	public int TotalKillCount => _totalKillCount;
	public float SurvivalTime => _survivalTime;

	public bool IsSurvive => _isSurvive;

	private void Update()
	{
		if (!_isTimerRunning) return;

		_survivalTime += Time.deltaTime;
	}

	public void Init()
	{
		_killCount = 0;
		_totalKillCount = 0;

		_survivalTime = 0;
	}

	public void StartTimer()
	{
		_isTimerRunning = true;
	}

	public void StopTimer()
	{
		_isTimerRunning = false;
	}

	public void SetSurvive(bool isSurvive)
	{
		_isSurvive = isSurvive;
	}

	public void AddKill(int count = 1)
	{
		_killCount += count;
		_totalKillCount += count;

		if (_printLog)
		{
			Debug.Log($"[{name}] {count}킬. 누적 {_killCount}킬");
		}

		//KillCountChanged?.Invoke(_killCount);
	}

	public void ResetKillCount()
	{
		_killCount = 0;
	}
}
