using System;

[Serializable]
public class SaveData
{
	public float MasterVolume;
	public float BGMVolume;
	public float SFXVolume;

	public float MouseSensitivity;

	public SaveData()
	{
		MasterVolume = 0.1f;
		BGMVolume = 0.1f;
		SFXVolume = 0.1f;
		MouseSensitivity = 50f;
	}
}
