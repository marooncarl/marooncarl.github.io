// Programmer: Carl Childers
// Date: 9/8/2017
//
// Creates text child objects showing a list of text items.  Stats are displayed in a table,
// and there can be multiple pages which are cycled through automatically.
// This requires another script to add records to it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordPage {

	bool hidden;
	public bool Hidden {
		get { return hidden; }
	}

	RecordBook myRecordBook;
	public RecordBook MyRecordBook {
		get { return myRecordBook; }
	}

	float fadeAlpha;

	bool finishedFade = true;

	LinkedList<Text> textItems;
	public LinkedList<Text> TextItems {
		get
		{
			if (textItems == null)
			{
				textItems = new LinkedList<Text>();
			}
			return textItems;
		}
	}

	public RecordPage(bool startHidden, RecordBook inBook)
	{
		hidden = startHidden;
		myRecordBook = inBook;
		fadeAlpha = (hidden ? 0 : 1);
	}

	public void Show()
	{
		hidden = false;
		finishedFade = false;

		foreach (Text item in textItems)
		{
			item.gameObject.SetActive(true);
		}
	}

	public void Hide()
	{
		hidden = true;
		finishedFade = false;
	}

	public void Update(float deltaTime)
	{
		if (finishedFade)
			return;

		if (hidden)
		{
			// Fade out
			if (myRecordBook.FadeDuration == 0)
			{
				fadeAlpha = 0;
			}
			else
			{
				fadeAlpha = Mathf.Max(fadeAlpha - (deltaTime / myRecordBook.FadeDuration), 0);
			}

			if (fadeAlpha == 0)
			{
				foreach (Text item in textItems)
				{
					item.gameObject.SetActive(false);
				}
				finishedFade = true;
				return;
			}
		}
		else
		{
			// Fade in
			if (myRecordBook.FadeDuration == 0)
			{
				fadeAlpha = 1;
			}
			else
			{
				fadeAlpha = Mathf.Min(fadeAlpha + (deltaTime / myRecordBook.FadeDuration), 1);
			}

			if (fadeAlpha == 1)
			{
				finishedFade = true;
			}
		}

		foreach (Text t in textItems)
		{
			Color newColor = t.color;
			newColor.a = myRecordBook.TextColor.a * fadeAlpha;
			t.color = newColor;
		}
	}
}

public class RecordBook : MonoBehaviour {

	public Font TextFont;
	public Color TextColor = Color.white;
	public int TextSize = 14;
	public int Rows = 3;
	public int Columns = 2;
	public Vector2 RecordSize;
	public float RowSeperation = 6;
	public float ColumnSeperation = 32;
	public float PageDisplayDuration = 3;
	public float FadeDuration = 1;

	List<RecordPage> pages;

	float pageCycleCounter;
	int displayedPageIndex;

	// These keep track of where the next record should be placed.
	int currentPageIndex;
	int currentColumnIndex;
	int currentRowIndex;


	void Awake()
	{
		pageCycleCounter = PageDisplayDuration;
	}

	void Update()
	{
		if (pageCycleCounter > 0)
		{
			pageCycleCounter -= Time.deltaTime;
		}
		else
		{
			pageCycleCounter = PageDisplayDuration;
			ShowNextPage();
		}

		if (pages != null)
		{
			foreach (RecordPage page in pages)
			{
				page.Update(Time.deltaTime);
			}
		}
	}

	public void ShowNextPage()
	{
		if (pages != null && pages.Count > 0)
		{
			if (displayedPageIndex < pages.Count)
			{
				pages[displayedPageIndex].Hide();
			}

			displayedPageIndex++;
			if (displayedPageIndex >= pages.Count)
			{
				displayedPageIndex = 0;
			}

			pages[displayedPageIndex].Show();
		}
	}

	public void AddRecord(string recordText)
	{
		// Determine position of the new record
		Vector2 recordPosition = GetRecordPosition(currentColumnIndex, currentRowIndex);

		GameObject textObj = new GameObject("Record", typeof(RectTransform), typeof(Text));
		RectTransform rTransform = textObj.GetComponent<RectTransform>();
		rTransform.SetParent(transform);
		rTransform.anchoredPosition = recordPosition;
		rTransform.localScale = Vector3.one;
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RecordSize.x);
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, RecordSize.y);

		Text textComp = textObj.GetComponent<Text>();
		textComp.font = TextFont;
		textComp.color = TextColor;
		textComp.fontSize = TextSize;
		textComp.text = recordText;
		textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
		textComp.verticalOverflow = VerticalWrapMode.Overflow;
		textComp.alignment = TextAnchor.MiddleCenter;

		if (pages == null)
		{
			pages = new List<RecordPage>();
		}
		// Add a new page if necessary
		if (currentPageIndex >= pages.Count)
		{
			bool startHidden = (currentPageIndex != displayedPageIndex);
			pages.Add(new RecordPage(startHidden, this));
		}
		pages[currentPageIndex].TextItems.AddLast(textComp);

		// Determine whether the new item should start out hidden or displayed
		textObj.SetActive( !pages[currentPageIndex].Hidden );

		MoveToNextRecordPosition();
	}

	void MoveToNextRecordPosition()
	{
		currentRowIndex++;
		if (currentRowIndex >= Rows)
		{
			currentRowIndex = 0;
			currentColumnIndex++;
			if (currentColumnIndex >= Columns)
			{
				currentColumnIndex = 0;
				currentPageIndex++;
			}
		}
	}

	public float GetPageWidth()
	{
		return Columns * RecordSize.x + ColumnSeperation * (Columns - 1);
	}

	public float GetPageHeight()
	{
		return Rows * RecordSize.y + RowSeperation * (Rows - 1);
	}

	protected virtual Vector2 GetRecordPosition(int column, int row)
	{
		Vector2 recordPos = Vector2.zero;

		recordPos.x -= GetPageWidth() / 2f;
		recordPos.x += column * (RecordSize.x + ColumnSeperation) + RecordSize.x / 2f;

		recordPos.y += GetPageHeight() / 2f;
		recordPos.y -= row * (RecordSize.y + RowSeperation) + RecordSize.y / 2f;

		return recordPos;
	}

	// Sets the time left for the currently displayed page
	public void SetCurrentPageDuration(float inTime)
	{
		pageCycleCounter = inTime;
	}
}
