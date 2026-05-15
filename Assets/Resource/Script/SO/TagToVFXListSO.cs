using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TagToVFX
{
	public string tag;
	public string vfxKey;
}

[CreateAssetMenu(fileName = "TagToVFXListSO_", menuName = "ScriptableObjects/Tag To VFX List (SO)")]
public class TagToVFXListSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private List<TagToVFX> _tagToVFXEntries = new List<TagToVFX>();
	#endregion

	#region 프로퍼티
	public IReadOnlyList<TagToVFX> TagToVFXEntries => _tagToVFXEntries;
	#endregion
}
