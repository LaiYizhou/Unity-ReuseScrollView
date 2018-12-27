using UnityEngine;

public class ReuseItemData : MonoBehaviour
{
    // for example
    [HideInInspector] public string Id;
    [HideInInspector] public string Name;

    public ReuseItemData(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

}
