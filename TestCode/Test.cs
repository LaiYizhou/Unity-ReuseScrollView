using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private ReuseScrollView reuseScrollView;
    [SerializeField] private GameObject prefab;

    [SerializeField] private InputField inputField;
    [SerializeField] private Button button;

	void Start ()
    {

        List<ReuseItemData> list = new List<ReuseItemData>()
        {
            new ReuseItemData("001", "Soil"),
            new ReuseItemData("002", "Sunshine"),
            new ReuseItemData("003", "Water"),
            new ReuseItemData("004", "Fertilizer"),
            new ReuseItemData("005", "Wheat"),
            new ReuseItemData("006", "Corn"),
            new ReuseItemData("007", "Soybean"),
            new ReuseItemData("008", "Sugarcane"),
            new ReuseItemData("009", "Egg"),
            new ReuseItemData("010", "Carrot"),

        };
        reuseScrollView.BuildContent(prefab, list);

        button.onClick.AddListener(OnButtonClicked);

	}

    private void OnButtonClicked()
    {
        int index = 0;
        if (int.TryParse(inputField.text, out index))
        {
            reuseScrollView.Nevigate(index);
        }
    }
	
	
}
