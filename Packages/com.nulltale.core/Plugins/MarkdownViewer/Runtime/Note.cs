using System;
using UnityEngine;

[ExecuteAlways]
public class Note: MonoBehaviour 
{
	public bool _showHeader = true;
	public View _view;
	[TextArea]
	public string _text;

	public bool _wasCreated;

	public enum View
	{
		Note,
		Cross,
		CheckBox,
		Star,
		None
	}

	private void OnValidate()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
			return;
		
		if (_wasCreated)
			return;
		
		_wasCreated = true;
		PluginMaster.PlayModeSave.Add(this, PluginMaster.PlayModeSave.SaveCommand.SAVE_ON_EXITING_PLAY_MODE, false, true);
#endif
	}
}