using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class Widget : MonoBehaviour
	{
		[SF] private TMP_Text text;

		private int _rank;
		
		public RectTransform Rect { get; private set; }
		
		private void Awake()
		{
			Rect = transform as RectTransform;
		}

		public void Setup(int rank)
		{
			SetName($"{rank}");

			_rank = rank;
		}

		public void SetPosition(Vector2 newPosition)
		{
			// Rect.anchorMin = new Vector2(0.5f, 1f);
			// Rect.anchorMax = new Vector2(0.5f, 1f);
			// Rect.pivot = new Vector2(0.5f, 1f);
			
			Rect.anchoredPosition = newPosition;
		}

		public void SetScaleY(float value)
		{
			Rect.localScale = new Vector3(Rect.localScale.x, value, Rect.localScale.z);
		}

		public void UpdateRank(int rank)
		{
			SetName($"{rank}");

			_rank = rank;
		}
		
		public int GetRank() => _rank;
		
		public Vector2 GetPosition() => Rect.anchoredPosition;
		
		public float GetHeight() => Rect.rect.height;

		private void SetName(string name)
		{
			gameObject.name = name;
			text.SetText(name);
		}

		public void SetTag(string tag = "")
		{
			if (string.IsNullOrEmpty(tag))
			{
				SetName($"{_rank}");
			}
			else
			{
				gameObject.name = $"{gameObject.name} {tag}";
				text.SetText($"{text.text} {tag}");
			}
		}
	}
}
