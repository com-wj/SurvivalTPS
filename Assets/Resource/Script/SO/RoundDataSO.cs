using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundDataSO_", menuName = "ScriptableObjects/Round Data (SO)")]
public class RoundDataSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private string _name;

	[Header("웨이브 목록")]
	[SerializeField] private List<WaveDataSO> _waveDatas = new List<WaveDataSO>();
	#endregion

	#region 프로퍼티
	public string Name => _name;
	public IReadOnlyList<WaveDataSO> WaveDatas => _waveDatas;
	#endregion
}
