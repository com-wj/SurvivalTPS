using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	#region 인스펙터
	[Header("스폰 포인트")]
	[SerializeField] private Transform[] _spawnPoints;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	[SerializeField] private bool _drawSpawnPointGizmos = false;
	[SerializeField] private bool _forceSpawnOnce = false;
	[SerializeField] private PooledObject _forceSpawnPrefab;
	#endregion

	#region 내부 변수
	private Coroutine _routine;
	#endregion

	private void Awake()
	{
		if (_spawnPoints == null ||
			_spawnPoints.Length == 0)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	// For test
	private void Start()
	{
		if (_forceSpawnPrefab == null) return;
		StartSpawn(_forceSpawnPrefab, 6);
	}

	private void OnDisable()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
	}

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

	// 범위 검사 및 생성 명령
	public void StartSpawn(PooledObject prefab, int UnitCountPerSpawn = 1, float spawnInterval = 0, int SpawnCount = 1)
	{
		if (_spawnPoints == null ||
			_spawnPoints.Length == 0) return;

		if (prefab == null) return;
		if (UnitCountPerSpawn <= 0 ||
			spawnInterval < 0 ||
			SpawnCount <= 0) return;

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_Spawning(prefab, UnitCountPerSpawn, spawnInterval, SpawnCount));
	}

	private IEnumerator Co_Spawning(PooledObject prefab, int UnitCountPerSpawn, float spawnInterval, int SpawnCount)
	{
		if (PoolManager.Instance == null)
		{
			_routine = null;
			yield break;
		}

		for (int i = 0; i < SpawnCount; i++)
		{
			// 회당 스폰 수
			for (int j = 0; j < UnitCountPerSpawn; j++)
			{
				Spawn(prefab);
			}

			if (_printLog)
			{
				Debug.Log($"[{name}] ({prefab}) {SpawnCount}마리 생성");
			}

			float elapsed = 0;
			while (elapsed < spawnInterval)
			{
				elapsed += Time.deltaTime;
				yield return null;
			}
		}

		if (_printLog)
		{
			Debug.Log($"[{name}] 스폰 루틴 종료. 총 스폰량 : {UnitCountPerSpawn * SpawnCount}");
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
	}

#if UNITY_EDITOR
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