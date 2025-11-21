using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogoGameManager : MonoBehaviour
{
    [Header("Фигуры (UI Image префабы)")]
    public GameObject square;
    public GameObject circle;
    public GameObject triangle;
    public GameObject rectangle;

    [Header("Настройки")]
    public RectTransform canvas;
    public int shapeCount = 5;

    [Header("UI")]
    public GameObject palettePanel;
    public Color[] colorPalette = new Color[5];
    public Button[] colorButtons;
    public Button doneButton;

    public List<DraggableUILogo> allShapes = new List<DraggableUILogo>();
    public Color? selectedColor = null;

    void Start()
    {
        palettePanel.SetActive(false);
        doneButton.gameObject.SetActive(false);

        for (int i = 0; i < colorButtons.Length && i < colorPalette.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => SelectColor(colorPalette[index]));
        }

        GenerateRandomShapes();
    }

    void GenerateRandomShapes()
    {
        // Используем ТОЛЬКО те, что объявлены
        List<GameObject> availablePrefabs = new List<GameObject>
        {
            square,
            circle,
            triangle,
            rectangle
        };

        // Удаляем null-элементы (на случай, если что-то не назначено)
        availablePrefabs.RemoveAll(prefab => prefab == null);

        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("Нет доступных префабов фигур!");
            return;
        }

        for (int i = 0; i < shapeCount; i++)
        {
            GameObject randomPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

            Vector2 spawnPos = new Vector2(
                Random.Range(-250f, 250f),
                Random.Range(-150f, 150f)
            );

            GameObject obj = Instantiate(randomPrefab, canvas);
            obj.GetComponent<RectTransform>().anchoredPosition = spawnPos;

            DraggableUILogo shape = obj.GetComponent<DraggableUILogo>();
            if (shape != null)
            {
                shape.manager = this;
                allShapes.Add(shape);
                obj.GetComponent<Image>().color = Color.white;
            }
        }

        palettePanel.SetActive(true);
    }

    public void SelectColor(Color color)
    {
        selectedColor = color;
        palettePanel.SetActive(false);
    }

    public void CheckCompletion()
    {
        if (allShapes.Count == 0) return;

        foreach (var shape in allShapes)
        {
            if (shape == null) continue; // Проверяем, что сам компонент не null
            
            Image img = shape.GetComponent<Image>();
            if (img != null && img.color == Color.white)
                return;
        }

        doneButton.gameObject.SetActive(true);
    }

    public void OnDoneButton()
    {
        Debug.Log("✅ Логотип готов. Сохраняй в галерее.");
    }

    public void ResetGame()
    {
        // Удаляем все созданные фигуры
        foreach (var shape in allShapes)
        {
            if (shape != null && shape.gameObject != null)
            {
                DestroyImmediate(shape.gameObject);
            }
        }
        allShapes.Clear();
        
        // Сбрасываем состояние
        selectedColor = null;
        palettePanel.SetActive(false);
        doneButton.gameObject.SetActive(false);
        
        // Перезапускаем генерацию
        GenerateRandomShapes();
    }
}
