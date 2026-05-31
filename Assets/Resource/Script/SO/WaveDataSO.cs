using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataSO_", menuName = "ScriptableObjects/Wave Data (SO)")]
public class WaveDataSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private string _name;
	
	[Header("웨이브 지속 시간")]
	[SerializeField] private float _duration = 30f; // 웨이브 지속 시간

	[Header("등장 적 정보")]
	[SerializeField] private PooledObject _prefab;
	[SerializeField] private int _unitCountPerSpawn;
	[SerializeField] private float _spawnInterval;

	[Header("보스 웨이브")]
	[SerializeField] private bool _isBossWave = false;
	#endregion

	#region 프로퍼티
	public string Name => _name;
	public float Duration
	{
		get
		{
			return Mathf.Max(0.1f, _duration);
		}
	}
	public PooledObject Prefab => _prefab;
	public int UnitCountPerSpawn =>	_unitCountPerSpawn;
	public float SpawnInterval => _spawnInterval;
	public bool IsBossWave => _isBossWave;
	#endregion
}
