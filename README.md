# Unity-ReuseScrollView
Unity3D使用ScrollView时，实现Item内容循环重复使用（**简单版本，Add即用，无需继承**）

#### 1. 简介

在Unity3D中，使用ScrollView时，会生成一系列子物体进行滑动

（注：这些子物体在本文档及代码中称为 “Item”）

当Item过多时，可以根据**重复循环使用**视图内的Item，以节约资源

其中关键点，大致两点：

**第一，滑出去的Item 重新移位，衔接滑进来的效果**

**第二，随着一系列Item的不断滑动，要准确定位当前Item的索引**

脚本 `ReuseScrollView.cs`、`ReuseItem.cs`、 `ReuseItemData.cs`   则可以实现此功能。

#### 2. 建议用法

- 第一步：将 `ReuseScrollView.cs` 挂在ScrollView上

![20181227163105](Image/20181227163105.jpg)

其中，**Scroll Direction 为方向，Spacing 为间距**

- 第二步：在准备用来做 Item 的Prefab 挂上 `ReuseItem.cs`、 `ReuseItemData.cs`  

![20181227163137](Image/20181227163137.jpg)

- 第三步：自定义 `ReuseItemData.cs` 数据 和 `UpdateSetDataToItem()` 方法

  示例：

  ```c#
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
  ```

- 第四步：调用 `BuildContent()` 即可

  ```c#
  void BuildContent(GameObject go, List<ReuseItemData> dataList, float spacing = 20.0f)
  ```

  示例：

  ```c#
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
  ```

#### 3. Demo及题外话

![demo](Image/demo.gif)

在 AssetStore 中也有类似的插件：

1. https://assetstore.unity.com/packages/tools/gui/infinity-scrollview-for-ugui-67771
2. https://assetstore.unity.com/packages/tools/gui/reusescroller-106279
