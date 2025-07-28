using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class Widget : MonoBehaviour
	{
		[SF] private TMP_Text text;

		private RectTransform _rectT;

		private int _index;
		
		public bool IsNewPlayer { get; private set; }
		
		public bool IsLastPlayer { get; private set; }

		private void Awake()
		{
			_rectT = transform as RectTransform;
		}

		public void SetName(int value)
		{
			gameObject.name = $"{value}";
			text.SetText($"{value}");

			_index = value;
		}

		public int GetIndex() => _index;

		public void SetPosition(Vector2 newPosition)
		{
			_rectT.anchorMin = new Vector2(0.5f, 1f);
			_rectT.anchorMax = new Vector2(0.5f, 1f);
			_rectT.pivot = new Vector2(0.5f, 1f);
			
			_rectT.anchoredPosition = newPosition;
		}

		public void SetIsLastPlayer(bool isLastPlayer = false)
		{
			IsLastPlayer = isLastPlayer;
		}
		
		public void SetIsNewPlayer(bool isNewPlayer = false)
		{
			IsNewPlayer = isNewPlayer;
		}
		
		public Vector2 GetPosition() => _rectT.anchoredPosition;
		
		public float GetHeight() => _rectT.rect.height;
	}
}
