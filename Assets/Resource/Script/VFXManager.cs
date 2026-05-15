using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
	#region 인스펙터
	[Header("VFX 리스트 SO")]
	[SerializeField] private VFXDataListSO _vfxDataList;
	#endregion

	#region 내부 변수
	private readonly Dictionary<string, PooledObject> _vfxDict = new Dictionary<string, PooledObject>(); // key, Prefab
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_vfxDataList == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		// 캐싱
		for (int i = 0; i < _vfxDataList.VFXEntries.Count; i++)
		{
			VFXEntry vfx = _vfxDataList.VFXEntries[i];

			if (string.IsNullOrEmpty(vfx.key)) continue;

			if (!_vfxDict.ContainsKey(vfx.key))
			{
				_vfxDict[vfx.key] = vfx.prefab;
			}
		}
	}

	public void PlayVFX(string key, Vector3 pos, Quaternion rot)
	{
		if (_vfxDict.TryGetValue(key, out PooledObject prefab))
		{
			if (PoolManager.Instance != null)
			{
				PoolManager.Instance.Pop(prefab, pos, rot);
			}
			else
			{
				Debug.LogWarning($"[{name}] 싱글톤 인스턴스 null");
			}
		}
		else
		{
			Debug.LogWarning($"[{name}] {key}가 존재하지 않습니다.");
		}
	}

	public void PlayHitImpact(RaycastHit hit)
	{
		string targetTag = hit.collider.tag;
		int targetLayer = hit.collider.gameObject.layer;

		Vector3 spawnPos = hit.point + (hit.normal * 0.001f);
		PlayVFX(targetTag, spawnPos, Quaternion.LookRotation(hit.normal));

		if (targetLayer == LayerMask.NameToLayer("Environment"))
		{
			switch (targetTag)
			{
				case "Concrete":
				case "Fabric":
					PlayVFX("Decal", spawnPos, Quaternion.LookRotation(hit.normal));
					break;
			}
		}
	}
}
