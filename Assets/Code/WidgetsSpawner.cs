using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class WidgetsSpawner : MonoBehaviour
	{
		[SF] private Widget _widgetPrefab;
		[SF] private int _widgetsAmount;
		[SF] private Transform _content;

		private void Start()
		{
			for (int i = 0; i < _widgetsAmount; i++)
			{
				var widget = Instantiate(_widgetPrefab, _content);
				widget.Setup(i);
			}
		}
	}
}