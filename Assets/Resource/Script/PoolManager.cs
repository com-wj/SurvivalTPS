using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	private readonly Dictionary<PooledObject, Queue<PooledObject>> _pools = new Dictionary<PooledObject, Queue<PooledObject>>(); // 다중 풀

	private readonly Dictionary<GameObject, Transform> _roots = new Dictionary<GameObject, Transform>(); // 하이어라키 정리용 트랜스폼

	public void Push(PooledObject prefab, PooledObject obj)
	{
		if (obj == null) return;

		obj.gameObject.SetActive(false);

		if (!_pools.ContainsKey(prefab))
		{
			_pools.Add(prefab, new Queue<PooledObject>());
			CreateRoot(prefab.gameObject);
		}

		obj.transform.SetParent(_roots[prefab.gameObject]);
		_pools[prefab].Enqueue(obj);
	}

	public PooledObject Pop(PooledObject prefab, Vector3 position, Quaternion rotation)
	{
		if (!_pools.ContainsKey(prefab))
		{
			_pools.Add(prefab, new Queue<PooledObject>());
			CreateRoot(prefab.gameObject);
		}

		PooledObject obj;

		if (_pools[prefab].Count > 0)
		{
			obj = _pools[prefab].Dequeue();
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
		}
		else
		{
			obj = Instantiate(prefab, position, rotation);
		}
		obj.Init(prefab);

		return obj;
	}

	// 하이어라키 루트 트랜스폼 생성
	private void CreateRoot(GameObject prefab)
	{
		if (_roots.ContainsKey(prefab)) return;

		GameObject root = new GameObject($"pool_{prefab.name}");
		_roots.Add(prefab, root.transform);
	}
}