using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private ReuseScrollView reuseScrollView;
    [SerializeField] private GameObject itemObject;
    
    void Start () {

        List<ReuseItemData> list = new List<ReuseItemData>()
        {
            new ReuseItemData("001", "Alice"),
            new ReuseItemData("002", "Bob"),
            new ReuseItemData("003", "Candy"),
            new ReuseItemData("004", "David"),
            new ReuseItemData("005", "Eric"),
            new ReuseItemData("006", "Fread"),
            new ReuseItemData("007", "Lobby"),
            new ReuseItemData("008", "Momo")
        };
       
        reuseScrollView.BuildContent(itemObject, list);
    }
	
	
}
