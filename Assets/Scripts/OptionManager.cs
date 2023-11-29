using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionManager : MonoBehaviour
{
    [SerializeField] private Image optionImage;
    public bool isSelected = false;
    public void ChangeSprite(Sprite sprite)
    {
        optionImage.sprite = sprite;
    }

    public void OnSelect()
    {
        QuizManager.Instance.SelectOption(this);
    }
}
