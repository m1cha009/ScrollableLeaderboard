using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class InfinityScrollController : MonoBehaviour
	{
		[SF] private Widget _widgetPrefab;
		[SF] private RectTransform _viewPort;
		[SF] private RectTransform _contentRect;
		[SF] private float _spacing;
		
		[SF] private int _lastPlayerRank;
		[SF] private int _newPlayerRank;

		[SF] private int _totalPlayers;

		private int _previousPlayerRank;
		private float _widgetHeight;
		
		private int _firstWidgetIndex;
		private int _lastWidgetIndex;
		
		private Vector2 _previousContentPosition = Vector2.zero;
		
		private readonly List<Widget> _widgets = new();

		public int LastPlayerRank => _lastPlayerRank;
		public int NewPlayerRank => _newPlayerRank;

		private void Start()
		{
			_previousContentPosition = _contentRect.anchoredPosition;
		}

		private void Update()
		{
			if (_lastPlayerRank != _previousPlayerRank)
			{
				Setup(_lastPlayerRank, _newPlayerRank);
				
				_previousPlayerRank = _lastPlayerRank;
				
				_previousContentPosition = _contentRect.anchoredPosition;
			}

			if (IsContentNeedUpdate())
			{
				_previousContentPosition = _contentRect.anchoredPosition;
			}
		}

		private void CleanWidgets()
		{
			foreach (var widget in _widgets)
			{
				Destroy(widget.gameObject);
			}
			
			_widgets.Clear();
		}

		private void Setup(int lastPlayerRank, int newPlayerRank)
		{
			CleanWidgets();
			
			var widgetsAmount = GetMaxWidgetsAmountInViewPort();
			
			SpawnWidgets(lastPlayerRank, newPlayerRank, widgetsAmount);
			
			SetupContentSize();

			AlignContentCurrentPlayerWithViewPort();

		}

		public float GetPlayerCenterPosition()
		{
			var currentWidgetPosition = GetPlayerPosition(_newPlayerRank);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;
			
			var contentPosition = currentWidgetPosition - viewPortCenter + widgetHalfHeight;

			return contentPosition;
		}

		public float GetPlayerPosition(int playerRank)
		{
			return playerRank * (_widgetHeight + _spacing);
		}
		
		public bool TryGetPlayerWidget(int playerRank, out Widget outWidget)
		{
			foreach (Widget widget in _widgets)
			{
				if (widget.GetRank() == playerRank)
				{
					outWidget = widget;
					return true;
				}
			}
			
			outWidget = null;
			return false;
		}

		public List<Widget> GetBelowPlayerWidgets()
		{
			var widgets = new List<Widget>();
			
			foreach (Widget widget in _widgets)
			{
				if (widget.GetRank() > _lastPlayerRank)
				{
					widgets.Add(widget);
				}
			}

			return widgets;
		}

		public void RemoveWidget(Widget playerWidget)
		{
			foreach (var widget in _widgets)
			{
				if (widget == playerWidget)
				{
					Destroy(widget.gameObject);
					_widgets.Remove(widget);

					_lastWidgetIndex--;
					
					return;
				}
			}
		}

		private int GetMaxWidgetsAmountInViewPort()
		{
			_widgetHeight = _widgetPrefab.GetComponent<RectTransform>().rect.height;
			var viewPort = _viewPort.rect.height;
			var widgetCount = Mathf.FloorToInt(viewPort / _widgetHeight);

			return widgetCount;
		}
		
		private void SpawnWidgets(int lastPlayerRank, int newPlayerRank, int widgetsAmount)
		{
			var extraWidget = 2;
			
			var halfAmount = Mathf.FloorToInt(widgetsAmount / 2f);
			var startIndex = lastPlayerRank - halfAmount - extraWidget;
			var endIndex = lastPlayerRank + halfAmount + extraWidget + 1;

			Debug.Log($"Current Player Index: {lastPlayerRank} Start: {startIndex}, End: {endIndex}, Amount: {widgetsAmount}");
			
			for (int i = startIndex; i < endIndex; i++)
			{
				var widget = Instantiate(_widgetPrefab, _contentRect);
				widget.SetName(i);
				widget.SetPosition(new Vector2(0, -i * (_widgetHeight + _spacing)));
				
				_widgets.Add(widget);
			}
			
			_firstWidgetIndex = 0;
			_lastWidgetIndex = _widgets.Count - 1;
		}

		private void SetupContentSize()
		{
			var totalHeight = _totalPlayers * (_widgetHeight + _spacing);
			_contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, totalHeight);
		}

		private void AlignContentCurrentPlayerWithViewPort()
		{
			var currentWidgetPosition = _lastPlayerRank * (_widgetHeight + _spacing);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;

			var contentPosition = currentWidgetPosition - viewPortCenter + widgetHalfHeight;
			
			_contentRect.anchoredPosition = new Vector2(_contentRect.anchoredPosition.x, contentPosition);
		}

		private bool IsContentNeedUpdate()
		{
			var upperThreshold = _previousContentPosition.y + _widgetHeight + _spacing;
			var lowerThreshold = _previousContentPosition.y - _widgetHeight - _spacing;
			
			if (_contentRect.anchoredPosition.y > upperThreshold)
			{
				MoveWidgetToBottom();

				return true;
			}
			
			if (_contentRect.anchoredPosition.y < lowerThreshold)
			{
				MoveWidgetToTop();
				
				return true;
			}

			return false;
		}

		private void MoveWidgetToBottom()
		{
			var firstWidget = _widgets[_firstWidgetIndex];
			var lastWidget = _widgets[_lastWidgetIndex];
			
			var newPosition = lastWidget.GetPosition().y + -lastWidget.GetHeight() + _spacing;
			
			firstWidget.SetPosition(new Vector2(0, newPosition));
			firstWidget.SetName(lastWidget.GetRank() + 1);
			
			_lastWidgetIndex = _firstWidgetIndex;
			_firstWidgetIndex = (_firstWidgetIndex + 1) % _widgets.Count;
		}
		
		private void MoveWidgetToTop()
		{
			var firstWidget = _widgets[_firstWidgetIndex];
			var lastWidget = _widgets[_lastWidgetIndex];
			
			var newPosition = firstWidget.GetPosition().y + firstWidget.GetHeight() + _spacing;
			
			lastWidget.SetPosition(new Vector2(0, newPosition));
			lastWidget.SetName(firstWidget.GetRank() - 1);
			
			_firstWidgetIndex = _lastWidgetIndex;
			_lastWidgetIndex = (_lastWidgetIndex - 1 + _widgets.Count) % _widgets.Count;
		}
		
	}
}