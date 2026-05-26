using TMPro;
using UnityEngine;

public class PlayerHPUI : MonoBehaviour
{
	[SerializeField] private PlayerBase _playerBase;
	[SerializeField] private TMP_Text _currentHpText;
	[SerializeField] private TMP_Text _maxHpText;

	private void Awake()
	{
		if (_playerBase == null ||
			_currentHpText == null ||
			_maxHpText == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_playerBase != null)
		{
			_playerBase.HPChanged += RefreshHPUI;
		}
	}

	private void OnDisable()
	{
		if (_playerBase != null)
		{
			_playerBase.HPChanged -= RefreshHPUI;
		}
	}

	private void RefreshHPUI(float currentHp, float maxHp)
	{
		_currentHpText.text = currentHp.ToString("F0");
		_maxHpText.text = maxHp.ToString("F0");

		Color c = Color.white;
		if (currentHp <= maxHp * 0.1f)
		{
			c = Color.red;
		}
		else if (currentHp <= maxHp * 0.3f)
		{
			c = Color.yellow;
		}
		_currentHpText.color = c;
	}
}