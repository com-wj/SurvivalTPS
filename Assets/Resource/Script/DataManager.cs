using System.IO;
using UnityEngine;

public static class DataManager
{
	private const string _saveFileName = "settings.json";
	private static readonly string SAVE_PATH = GetSavePath();

	private static SaveData _cachedData = null;

	// 저장 경로 설정
	private static string GetSavePath()
	{
		string path = Directory.GetParent(Application.dataPath).FullName;
		return Path.Combine(path, _saveFileName);
	}

	public static void Save(SaveData data)
	{
		if (_cachedData != data)
		{
			_cachedData = data;
		}

		string json;
#if UNITY_EDITOR
		json = JsonUtility.ToJson(data, true); // 클래스 데이터 → JSON
#else
		json = JsonUtility.ToJson(data);
#endif
		File.WriteAllText(SAVE_PATH, json); // 파일로 저장
	}

	public static SaveData Load()
	{
		if (_cachedData != null)
		{
			return _cachedData;
		}

		if (!File.Exists(SAVE_PATH)) // 경로에 세이브 데이터가 없으면
		{
			_cachedData = new SaveData();
			Save(_cachedData);
			return _cachedData;
		}

		string json = File.ReadAllText(SAVE_PATH); // 파일 읽기
		_cachedData = JsonUtility.FromJson<SaveData>(json); // JSON → 클래스 데이터
		return _cachedData;
	}
}