using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct VFXEntry
{
	public string key;
	public PooledObject prefab;
}

[CreateAssetMenu(fileName = "VFXDataListSO_", menuName = "ScriptableObjects/VFX Data List (SO)")]
public class VFXDataListSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private List<VFXEntry> _vfxEntries = new List<VFXEntry>();
	#endregion

	#region 프로퍼티
	public IReadOnlyList<VFXEntry> VFXEntries => _vfxEntries;
	#endregion
}
