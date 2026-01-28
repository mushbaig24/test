using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private GridLayoutGroup gridLayout;
    
    public List<Card> SpawnCards(int rows, int cols, List<CardData> cardDataList, Sprite backSprite)
    {
        ClearGrid();

        // Adjust Grid
        AdjustGrid(rows, cols);

        List<Card> spawnedCards = new List<Card>();
        int totalCards = rows * cols;
        
        // Prepare list of data (pairs)
        List<CardData> selectedData = new List<CardData>();
        int pairsNeeded = totalCards / 2;
        
        for (int i = 0; i < pairsNeeded; i++)
        {
            CardData data = cardDataList[i % cardDataList.Count];
            selectedData.Add(data);
            selectedData.Add(data);
        }

        if (totalCards % 2 != 0 && cardDataList.Count > 0)
        {
            selectedData.Add(cardDataList[0]);
        }

        // Shuffle
        for (int i = 0; i < selectedData.Count; i++)
        {
            CardData temp = selectedData[i];
            int randomIndex = UnityEngine.Random.Range(i, selectedData.Count);
            selectedData[i] = selectedData[randomIndex];
            selectedData[randomIndex] = temp;
        }

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = ObjectPooler.Instance.Get(container);
            cardObj.name = $"Card_{i}";
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(selectedData[i], backSprite);
            spawnedCards.Add(card);
        }

        return spawnedCards;
    }

    public void AdjustGrid(int rows, int cols)
    {
        if (container == null || gridLayout == null) return;

        // Force a layout rebuild to ensure container.rect is up to date
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);

        float width = container.rect.width;
        float height = container.rect.height;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = cols;

        float spacingX = gridLayout.spacing.x * (cols - 1);
        float spacingY = gridLayout.spacing.y * (rows - 1);

        float cellWidth = (width - gridLayout.padding.left - gridLayout.padding.right - spacingX) / cols;
        float cellHeight = (height - gridLayout.padding.top - gridLayout.padding.bottom - spacingY) / rows;

        // Keep aspect ratio square based on smallest dimension available
        float smallest = Mathf.Min(cellWidth, cellHeight);
        gridLayout.cellSize = new Vector2(smallest, smallest);
    }

    public void ClearGrid()
    {
        if (container == null) return;
        
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            if (child.TryGetComponent<Card>(out Card card))
            {
                ObjectPooler.Instance.ReturnToPool(child.gameObject);
            }
        }
    }
}
