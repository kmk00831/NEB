//층 안내
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFloor : MonoBehaviour
{
    public Image floorImage; // Canvas 내의 이미지 오브젝트
    public Sprite b1Image;
    public Sprite f1Image;
    public Sprite f2Image;
    public Sprite f3Image;
    public Sprite f4Image;
    public Sprite f5Image;
    public Sprite f6Image;
    public Sprite f7Image;
    public Sprite f8Image;

    private Dictionary<int, Sprite> floorSprites;

    private void Start()
    {
        // 층 번호와 Sprite를 매핑
        floorSprites = new Dictionary<int, Sprite>
        {
            { -1, b1Image },
            {  1, f1Image },
            {  2, f2Image },
            {  3, f3Image },
            {  4, f4Image },
            {  5, f5Image },
            {  6, f6Image },
            {  7, f7Image },
            {  8, f8Image }
        };
    }

    public void UpdateFloorDisplay(int floor)
    {
        if (floorSprites.ContainsKey(floor))
        {
            floorImage.sprite = floorSprites[floor];
            floorImage.enabled = true;
        }
        else
        {
            Debug.LogWarning($"층 이미지 없음: {floor}");
            floorImage.enabled = false;
        }
    }
}
