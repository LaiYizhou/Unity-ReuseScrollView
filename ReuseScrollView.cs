using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Vertical,
    Horizontal
}

[RequireComponent(typeof(RectTransform), typeof(ScrollRect))]
public class ReuseScrollView : MonoBehaviour {

    public Direction ScrollDirection;
    
    private GameObject itemObject;
    private RectTransform rectTransform;
    private ScrollRect scrollRect;
    private Vector2 scrollPosition;

    private readonly LinkedList<ReuseItem> itemsLinkedList = new LinkedList<ReuseItem>();
    private List<ReuseItemData> itemDataList = new List<ReuseItemData>();

    public static float Spacing;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();

        scrollRect.horizontal = ScrollDirection == Direction.Horizontal;
        scrollRect.vertical = ScrollDirection == Direction.Vertical;

        RectTransform contentRectTransform = scrollRect.content.GetComponent<RectTransform>();
        if (ScrollDirection == Direction.Vertical)
        {
            contentRectTransform.anchorMin = Vector2.up;
            contentRectTransform.anchorMax = Vector2.one;
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            contentRectTransform.anchorMin = Vector2.zero;
            contentRectTransform.anchorMax = Vector2.up;
        }
        contentRectTransform.anchoredPosition = Vector2.zero;
        contentRectTransform.sizeDelta = Vector2.zero;

        scrollRect.onValueChanged.AddListener(OnScrolled);
    }

    public void Nevigate(int index)
    {
        if(index < 0 || index >= itemDataList.Count)
            return;

        Vector2 target;
        if (ScrollDirection == Direction.Vertical)
        {
            float offset = (index - 1) * (GetItemSize() + Spacing) + GetItemSize() * (1 - itemObject.GetComponent<RectTransform>().pivot.y);
            offset -= scrollRect.viewport.rect.height * 0.5f;
            target = new Vector2(0.0f, Mathf.Clamp01(1 - offset / (scrollRect.content.rect.height - scrollRect.viewport.rect.height)));
        }
        else
        {
            float offset = (index - 1) * (GetItemSize() + Spacing) + GetItemSize() * (1 - itemObject.GetComponent<RectTransform>().pivot.x);
            offset -= scrollRect.viewport.rect.width * 0.5f;
            target = new Vector2(Mathf.Clamp01(offset / (scrollRect.content.rect.width - scrollRect.viewport.rect.width)), 0.0f);
        }

        DOTween.To(() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, target, 1.0f);

    }

    public void BuildContent(GameObject go, List<ReuseItemData> dataList, float spacing = 20.0f)
    {
        this.itemObject = go;
        itemDataList = dataList;

        Spacing = spacing;

        ReloadData();
    }

    private void OnScrolled(Vector2 pos)
    {
        ReuseItems(pos - scrollPosition);
        FillItems();
        scrollPosition = pos;
    }

    private void ReloadData()
    {
        Vector2 sizeDelta = scrollRect.content.sizeDelta;
        float contentSize = 0;
        for (int i = 0; i < itemDataList.Count; i++)
        {
            contentSize += GetItemSize() + (i > 0 ? Spacing : 0);
        }
        
        if (ScrollDirection == Direction.Vertical)
        {
            sizeDelta.y = contentSize > rectTransform.rect.height ? contentSize : rectTransform.rect.height;
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            sizeDelta.x = contentSize > rectTransform.rect.width ? contentSize : rectTransform.rect.width;
        }
        scrollRect.content.sizeDelta = sizeDelta;

        foreach (ReuseItem cell in itemsLinkedList)
        {
            Destroy(cell.gameObject);
        }
        itemsLinkedList.Clear();

        scrollRect.normalizedPosition = scrollRect.content.GetComponent<RectTransform>().anchorMin;
        scrollRect.onValueChanged.Invoke(scrollRect.normalizedPosition);

    }

    private void CreateItem(int index)
    {
        ReuseItem reuseItem = Instantiate(itemObject).GetComponent<ReuseItem>();
        reuseItem.SetAnchors(scrollRect.content.anchorMin, scrollRect.content.anchorMax);
        reuseItem.transform.SetParent(scrollRect.content.transform, false);
        UpdateItem(reuseItem, index);

        if (ScrollDirection == Direction.Vertical)
        {
            reuseItem.Top = (itemsLinkedList.Count > 0 ? itemsLinkedList.Last.Value.Bottom - Spacing : 0);
            reuseItem.SetOffsetHorizontal(0, 0);
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            reuseItem.Left = (itemsLinkedList.Count > 0 ? itemsLinkedList.Last.Value.Right + Spacing : 0);
            reuseItem.SetOffsetVertical(0, 0);
        }

        itemsLinkedList.AddLast(reuseItem);
    }

    private void UpdateItem(ReuseItem cell, int index)
    {
        cell.DataIndex = index;
        if (cell.DataIndex >= 0 && cell.DataIndex < itemDataList.Count)
        {
            cell.UpdateSetDataToItem(itemDataList[cell.DataIndex]);
            cell.gameObject.SetActive(true);
        }
        else
        {
            cell.gameObject.SetActive(false);
        }
    }

    private void FillItems()
    {
        if (itemsLinkedList.Count == 0)
            CreateItem(0);

        while (CellsTailEdge + Spacing <= ActiveTailEdge)
        {
            CreateItem(itemsLinkedList.Last.Value.DataIndex + 1);
        }
    }

    private void ReuseItems(Vector2 scrollVector)
    {
        if (itemsLinkedList.Count == 0)
            return;

        if (ScrollDirection == Direction.Vertical)
        {
            if (scrollVector.y > 0)
            {
                while (CellsTailEdge - GetItemSize() >= ActiveTailEdge)
                {
                    MoveCellLastToFirst();
                }
            }
            else if (scrollVector.y < 0)
            {
                while (CellsHeadEdge + GetItemSize() <= ActiveHeadEdge)
                {
                    MoveCellFirstToLast();
                }
            }
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            if (scrollVector.x > 0)
            {
                while (CellsHeadEdge + GetItemSize() <= ActiveHeadEdge)
                {
                    MoveCellFirstToLast();
                }
            }
            else if (scrollVector.x < 0)
            {
                while (CellsTailEdge - GetItemSize() >= ActiveTailEdge)
                {
                    MoveCellLastToFirst();
                }
            }
        }
    }

    private void MoveCellFirstToLast()
    {
        if (itemsLinkedList.Count == 0) return;

        ReuseItem firstCell = itemsLinkedList.First.Value;
        ReuseItem lastCell = itemsLinkedList.Last.Value;
        UpdateItem(firstCell, lastCell.DataIndex + 1);

        if (ScrollDirection == Direction.Vertical)
        {
            firstCell.Top = lastCell.Bottom - Spacing;
            firstCell.SetOffsetHorizontal(0, 0);
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            firstCell.Left = lastCell.Right + Spacing;
            firstCell.SetOffsetVertical(0, 0);
        }

        itemsLinkedList.RemoveFirst();
        itemsLinkedList.AddLast(firstCell);
    }

    private void MoveCellLastToFirst()
    {
        if (itemsLinkedList.Count == 0) return;

        ReuseItem lastCell = itemsLinkedList.Last.Value;
        ReuseItem firstCell = itemsLinkedList.First.Value;
        UpdateItem(lastCell, firstCell.DataIndex - 1);

        if (ScrollDirection == Direction.Vertical)
        {
            lastCell.Bottom = firstCell.Top + Spacing;
            lastCell.SetOffsetHorizontal(0, 0);
        }
        else if (ScrollDirection == Direction.Horizontal)
        {
            lastCell.Right = firstCell.Left - Spacing;
            lastCell.SetOffsetVertical(0, 0);
        }

        itemsLinkedList.RemoveLast();
        itemsLinkedList.AddFirst(lastCell);
    }

    private float GetItemSize()
    {
        if (ScrollDirection == Direction.Vertical)
            return itemObject.GetComponent<RectTransform>().rect.height;
        else
            return itemObject.GetComponent<RectTransform>().rect.width;
    }

    private float ActiveHeadEdge
    {
        get
        {
            if (ScrollDirection == Direction.Vertical)
            {
                return scrollRect.content.anchoredPosition.y;
            }
            else if (ScrollDirection == Direction.Horizontal)
            {
                return -scrollRect.content.anchoredPosition.x;
            }
            return 0;
        }
    }

    private float ActiveTailEdge
    {
        get
        {
            if (ScrollDirection == Direction.Vertical)
            {
                return scrollRect.content.anchoredPosition.y + rectTransform.rect.height;
            }
            else if (ScrollDirection == Direction.Horizontal)
            {
                return -scrollRect.content.anchoredPosition.x + rectTransform.rect.width;
            }
            return 0;
        }
    }

    private float CellsHeadEdge
    {
        get
        {
            if (ScrollDirection == Direction.Vertical)
            {
                return itemsLinkedList.Count > 0 ? -itemsLinkedList.First.Value.Top : 0;
            }
            else if (ScrollDirection == Direction.Horizontal)
            {
                return itemsLinkedList.Count > 0 ? itemsLinkedList.First.Value.Left : 0;
            }
            return 0;
        }
    }

    private float CellsTailEdge
    {
        get
        {
            if (ScrollDirection == Direction.Vertical)
            {
                return itemsLinkedList.Count > 0 ? -itemsLinkedList.Last.Value.Bottom : 0;
            }
            else if (ScrollDirection == Direction.Horizontal)
            {
                return itemsLinkedList.Count > 0 ? itemsLinkedList.Last.Value.Right : 0;
            }
            return 0;
        }
    }
}
