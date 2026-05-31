using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
	#region 인스펙터
	[Header("스폰 포인트")]
	[SerializeField] private Transform[] _spawnPoints;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	[SerializeField] private bool _drawSpawnPointGizmos = false;
	[SerializeField] private bool _forceSpawnOnce = false;
	[SerializeField] private PooledObject _forceSpawnPrefab;

	[Header("활성화된 적")]
	[SerializeField] private List<PooledObject> _aliveEnemyList = new List<PooledObject>();

	[Header("의존성")]
	[SerializeField] private PlayerBase _playerBase;
	#endregion

	#region 내부 변수
	private Coroutine _routine;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_spawnPoints == null ||
			_spawnPoints.Length == 0 ||
			_playerBase == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
		_aliveEnemyList.Clear();
	}

	private void OnEnable()
	{
		if (_playerBase != null)
		{
			_playerBase.Dead += StopSpawn;
		}
	}

	private void OnDisable()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		if (_playerBase != null)
		{
			_playerBase.Dead -= StopSpawn;
		}
	}

#if UNITY_EDITOR
	// For Test
	private void Update()
	{
		if (_forceSpawnOnce || Input.GetKeyDown(KeyCode.F1))
		{
			if (_forceSpawnPrefab == null) return;
			_forceSpawnOnce = false;

			StartSpawn(_forceSpawnPrefab, 3);
		}
	}
#endif

	// 범위 검사 및 생성 명령
	// spawnCount = -1 : 무한 생성
	public void StartSpawn(PooledObject prefab, int UnitCountPerSpawn = 1, float spawnInterval = 0, int spawnCount = 1)
	{
		if (_spawnPoints == null ||
			_spawnPoints.Length == 0) return;

		if (prefab == null) return;
		if (UnitCountPerSpawn <= 0 ||
			spawnInterval < 0 ||
			(spawnCount <= 0 && spawnCount != -1)) return;

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_Spawning(prefab, UnitCountPerSpawn, spawnInterval, spawnCount));
	}

	private IEnumerator Co_Spawning(PooledObject prefab, int UnitCountPerSpawn, float spawnInterval, int spawnCount)
	{
		if (PoolManager.Instance == null)
		{
			_routine = null;
			yield break;
		}

		bool isInfinite = (spawnCount == -1);
		int logSpawnCount = spawnCount;

		while(isInfinite || spawnCount > 0)
		{
			// 회당 스폰 수
			for (int i = 0; i < UnitCountPerSpawn; i++)
			{
				Spawn(prefab);
			}

			if (_printLog)
			{
				Debug.Log($"[{name}] ({prefab.name}) {UnitCountPerSpawn}마리 생성");
			}

			if (!isInfinite)
			{
				spawnCount--;
			}

			float elapsed = 0;
			while (elapsed < spawnInterval)
			{
				elapsed += Time.deltaTime;
				yield return null;
			}
		}

		if (_printLog && !isInfinite)
		{
			Debug.Log($"[{name}] 스폰 루틴 종료. 총 스폰량 : {UnitCountPerSpawn * logSpawnCount}");
		}
		_routine = null;
	}

	// 1회 생성
	private void Spawn(PooledObject prefab)
	{
		int randomIndex = Random.Range(0, _spawnPoints.Length);
		Transform spawnPoint = _spawnPoints[randomIndex];

		PooledObject obj = PoolManager.Instance.Pop(prefab, spawnPoint.position, Quaternion.identity);
		obj.Init(prefab);

		_aliveEnemyList.Add(obj);
	}

	public void RemoveEnemyFromList(PooledObject obj)
	{
		_aliveEnemyList.Remove(obj); // 자동으로 방어됨.
	}

	public void StopAndClearEnemies()
	{
		StopSpawn();
		ClearEnemies();
	}

	private void StopSpawn()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
			if (_printLog)
			{
				Debug.Log($"[{name}] 생성 중단.");
			}
		}
	}

	private void ClearEnemies()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] 모든 몹 클리어.");
		}

		for (int i = _aliveEnemyList.Count - 1; i >= 0; i--)
		{
			PooledObject pobj = _aliveEnemyList[i];
			if (pobj == null || !pobj.gameObject.activeSelf) continue;

			EnemyBase enemy = pobj as EnemyBase;
			if (enemy == null) continue;

			enemy.Die(false);
		}

		_aliveEnemyList.Clear();
	}

#if UNITY_EDITOR
	// 스폰 포인트 표시
	private void OnDrawGizmos()
	{
		if (!_drawSpawnPointGizmos) return;

		Gizmos.color = Color.green;

		for (int i = 0; i < _spawnPoints.Length; i++)
		{
			Gizmos.DrawWireSphere(_spawnPoints[i].position, 1f);
		}
	}
#endif
}