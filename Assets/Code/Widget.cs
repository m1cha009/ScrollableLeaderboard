using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class Widget : MonoBehaviour
	{
		[SF] private TMP_Text text;

		private int _index;
		
		public RectTransform Rect { get; set; }
		
		public bool IsNewPlayer { get; private set; }
		
		public bool IsLastPlayer { get; private set; }

		private void Awake()
		{
			Rect = transform as RectTransform;
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
			Rect.anchorMin = new Vector2(0.5f, 1f);
			Rect.anchorMax = new Vector2(0.5f, 1f);
			Rect.pivot = new Vector2(0.5f, 1f);
			
			Rect.anchoredPosition = newPosition;
		}

		public void SetIsLastPlayer(bool isLastPlayer = false)
		{
			IsLastPlayer = isLastPlayer;
		}
		
		public void SetIsNewPlayer(bool isNewPlayer = false)
		{
			IsNewPlayer = isNewPlayer;
		}
		
		public Vector2 GetPosition() => Rect.anchoredPosition;
		
		public float GetHeight() => Rect.rect.height;
	}
}
