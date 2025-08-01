using System;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class InfinityScrollController : MonoBehaviour
	{
		[SF] private Widget _widgetPrefab;
		[SF] private Widget _currentPlayerWidget;
		[SF] private RectTransform _viewPort;
		[SF] private RectTransform _contentRect;
		[SF] private float _spacing;
		
		[SF] private int _lastPlayerRank;
		[SF] private int _newPlayerRank;

		[SF] private int _totalPlayers;

		private int _previousPlayerRank;
		private float _widgetHeight;

		private int _centerPlayerRank;
		
		private int _firstWidgetIndex;
		private int _lastWidgetIndex;
		
		private int _extraWidget = 2;
		
		private Vector2 _previousContentPosition = Vector2.zero;
		
		private readonly List<Widget> _widgets = new();
		private readonly Dictionary<int, Widget> _widgetsByRankDict = new();

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
				Setup(_lastPlayerRank);
				
				_previousPlayerRank = _lastPlayerRank;
				
				_previousContentPosition = _contentRect.anchoredPosition;
			}

			UpdateWidgets();
		}

		private void CleanWidgets()
		{
			foreach (var widget in _widgets)
			{
				Destroy(widget.gameObject);
			}
			
			_widgets.Clear();
			_widgetsByRankDict.Clear();
		}

		private void Setup(int lastPlayerRank)
		{
			CleanWidgets();
			
			var widgetsAmount = GetMaxWidgetsAmountInViewPort();
			
			SpawnWidgets(lastPlayerRank, widgetsAmount);

			SpawnCurrentPlayerWidget(lastPlayerRank);
			
			SetupContentSize();

			CenterPlayerByRank(lastPlayerRank);
			
			_centerPlayerRank = LastPlayerRank;
			_firstWidgetIndex = 0;
			_lastWidgetIndex = _widgets.Count - 1;

		}

		public float GetPlayerYCenterPositionInViewPort(int playerRank)
		{
			var currentWidgetPosition = GetPlayerPosition(playerRank);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;
			
			var contentPosition =  currentWidgetPosition - viewPortCenter + widgetHalfHeight;

			return contentPosition;
		}

		public float GetPlayerPosition(int playerRank)
		{
			return playerRank * (_widgetHeight + _spacing);
		}

		public Widget GetInactiveWidget()
		{
			var firstInactiveWidgetFromBottom = _centerPlayerRank + _extraWidget;

			if (TryGetPlayerWidget(firstInactiveWidgetFromBottom, out Widget inactiveWidget))
			{
				return inactiveWidget;
			}

			return null;
		}

		public bool TryGetPlayerWidget(int playerRank, out Widget outWidget)
		{
			return _widgetsByRankDict.TryGetValue(playerRank, out outWidget);
		}

		public List<Widget> GetPlayerWidgetsBelowCurrentRank(int playerRank)
		{
			var widgets = new List<Widget>();
			
			foreach (Widget widget in _widgets)
			{
				if (widget.GetRank() > playerRank)
				{
					widgets.Add(widget);
				}
			}

			return widgets;
		}

		public void RemoveWidget(Widget playerWidget)
		{
			if (playerWidget == null)
				return;
			
			Destroy(playerWidget.gameObject);
			
			_widgets.Remove(playerWidget);
			_widgetsByRankDict.Remove(playerWidget.GetRank());
			
			_lastWidgetIndex--;
		}
		
		public void UpdateWidgetData(int centerPlayerRank)
		{
			if (TryGetPlayerWidget(centerPlayerRank, out var centerWidget))
			{
				centerWidget.Setup(centerPlayerRank + 1);
			}
		}

		private int GetMaxWidgetsAmountInViewPort()
		{
			_widgetHeight = _widgetPrefab.GetComponent<RectTransform>().rect.height;
			var viewPort = _viewPort.rect.height;
			var widgetCount = Mathf.FloorToInt(viewPort / _widgetHeight);

			return widgetCount;
		}
		
		private void SpawnWidgets(int previousPlayerRank, int widgetsAmount)
		{
			var halfAmount = Mathf.FloorToInt(widgetsAmount / 2f);
			var startIndex = previousPlayerRank - halfAmount - _extraWidget;
			var endIndex = previousPlayerRank + halfAmount + _extraWidget + 1;

			for (int i = startIndex; i < endIndex; i++)
			{
				var widget = Instantiate(_widgetPrefab, _contentRect);
				widget.Setup(i);
				widget.SetPosition(new Vector2(0, -i * (_widgetHeight + _spacing)));
				
				_widgets.Add(widget);
				_widgetsByRankDict.Add(i, widget);
			}
		}

		private void SpawnCurrentPlayerWidget(int playerRank)
		{
			_currentPlayerWidget.Setup(playerRank);
		}

		private void SetupContentSize()
		{
			var totalHeight = _totalPlayers * (_widgetHeight + _spacing);
			_contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, totalHeight);
		}

		private bool UpdateWidgets()
		{
			var upperThreshold = _previousContentPosition.y + _widgetHeight + _spacing;
			var lowerThreshold = _previousContentPosition.y - _widgetHeight - _spacing;
			
			if (_contentRect.anchoredPosition.y > upperThreshold)
			{
				MoveWidgetToBottom();
				
				_centerPlayerRank++;
				
				UpdateWidgetData(_centerPlayerRank);
				_currentPlayerWidget.UpdateRank(_centerPlayerRank);
				
				_previousContentPosition.y = upperThreshold;

				return true;
			}
			
			if (_contentRect.anchoredPosition.y < lowerThreshold)
			{
				MoveWidgetToTop();
				
				_centerPlayerRank--;

				UpdateWidgetData(_centerPlayerRank);
				_currentPlayerWidget.UpdateRank(_centerPlayerRank);
				
				var positionY = _contentRect.anchoredPosition.y + (lowerThreshold - _contentRect.anchoredPosition.y);
				_previousContentPosition = new Vector2(0, positionY);

				return true;
			}

			return false;
		}

		private void MoveWidgetToBottom()
		{
			var firstWidget = _widgets[_firstWidgetIndex];
			var lastWidget = _widgets[_lastWidgetIndex];
			
			var newPosition = lastWidget.GetPosition().y + -lastWidget.GetHeight() + _spacing;

			_widgetsByRankDict.Remove(firstWidget.GetRank());
			
			firstWidget.SetPosition(new Vector2(0, newPosition));
			firstWidget.Setup(lastWidget.GetRank() + 1);
			
			_widgetsByRankDict[firstWidget.GetRank()] = firstWidget;
			
			_lastWidgetIndex = _firstWidgetIndex;
			_firstWidgetIndex = (_firstWidgetIndex + 1) % _widgets.Count;
		}
		
		private void MoveWidgetToTop()
		{
			var firstWidget = _widgets[_firstWidgetIndex];
			firstWidget.SetTag();
			
			var lastWidget = _widgets[_lastWidgetIndex];
			
			_widgetsByRankDict.Remove(lastWidget.GetRank());
			
			var newPosition = firstWidget.GetPosition().y + lastWidget.GetHeight() + _spacing;
			
			lastWidget.SetPosition(new Vector2(0, newPosition));
			lastWidget.Setup(firstWidget.GetRank() - 1);
			lastWidget.SetTag("Last");
			
			_widgetsByRankDict[lastWidget.GetRank()] = lastWidget;
			
			_firstWidgetIndex = _lastWidgetIndex;
			_lastWidgetIndex = (_lastWidgetIndex - 1 + _widgets.Count) % _widgets.Count;
		}

		private void CenterPlayerByRank(int playerRank)
		{
			var playerPosY = GetPlayerYCenterPositionInViewPort(playerRank);
			_contentRect.anchoredPosition = new Vector2(_contentRect.anchoredPosition.x, playerPosY);
		}
		
	}
}