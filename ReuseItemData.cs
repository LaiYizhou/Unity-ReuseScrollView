using UnityEngine;
using UnityEngine.UI;

public class ReuseItemData : MonoBehaviour
{
    // for example
    [HideInInspector] public string Id;
    [HideInInspector] public string Name;

    [SerializeField] private Text text;

    public ReuseItemData(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    // for example
    public void ShowData(ReuseItemData data)
    {
        Id = data.Id;
        Name = data.Name;

        text.text = string.Format("{0}\n{1}", Id, Name);
    }

}
