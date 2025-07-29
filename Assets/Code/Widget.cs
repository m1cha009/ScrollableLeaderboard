using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class Widget : MonoBehaviour
	{
		[SF] private TMP_Text text;

		private int _rank;
		
		public RectTransform Rect { get; set; }
		
		private void Awake()
		{
			Rect = transform as RectTransform;
		}

		public void SetName(int value)
		{
			gameObject.name = $"{value}";
			text.SetText($"{value}");

			_rank = value;
		}

		public int GetRank() => _rank;

		public void SetPosition(Vector2 newPosition)
		{
			Rect.anchorMin = new Vector2(0.5f, 1f);
			Rect.anchorMax = new Vector2(0.5f, 1f);
			Rect.pivot = new Vector2(0.5f, 1f);
			
			Rect.anchoredPosition = newPosition;
		}
		
		public Vector2 GetPosition() => Rect.anchoredPosition;
		
		public float GetHeight() => Rect.rect.height;
	}
}
