using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
	[SerializeField] private PlayerShooter _playerShooter;
	[SerializeField] private TMP_Text _currentAmmoText;
	[SerializeField] private TMP_Text _totalAmmoText;

	private void Awake()
	{
		if (_playerShooter == null ||
			_currentAmmoText == null ||
			_totalAmmoText == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_playerShooter != null)
		{
			_playerShooter.AmmoChange += RefreshAmmoUI;
		}
	}

	private void OnDisable()
	{
		if (_playerShooter != null)
		{
			_playerShooter.AmmoChange -= RefreshAmmoUI;
		}
	}

	private void RefreshAmmoUI(int currentAmmo, int totalAmmo)
	{
		_currentAmmoText.text = currentAmmo.ToString();
		_totalAmmoText.text = totalAmmo.ToString();

		_currentAmmoText.color = (currentAmmo == 0) ? Color.red : Color.white;
		_totalAmmoText.color = (currentAmmo == 0 && totalAmmo == 0) ? Color.red : Color.white;
	}
}