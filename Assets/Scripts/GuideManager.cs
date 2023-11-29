using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GuideManager : MonoBehaviour
{
    [SerializeField] private Image[] guideLine;
    [SerializeField] private TextMeshProUGUI des;
    [SerializeField] private Color selectedColor, defaultColor;
    [SerializeField] private string[] guideDetails;
    private int index = 0;
    private void OnEnable()
    {
        index = -1;
        OnNext();
    }
    private void DefaultLine()
    {
        foreach (var line in guideLine)
        {
            line.color = defaultColor;
        }
    }
    public void OnNext()
    {
        if (index < guideDetails.Length - 1)
        {
            index++;
            DefaultLine();
            des.text = guideDetails[index];
            guideLine[index].color = selectedColor;
        }
        else
        {
            QuizManager.Instance.OnSkipButton();
        }
    }
}
