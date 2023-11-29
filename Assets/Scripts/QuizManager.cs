using UnityEngine;
using UnityEngine.SceneManagement;
public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject navigationPanel;

    [SerializeField] private GameObject wrongAnswer;
    [SerializeField] private GameObject playAgain;
    [SerializeField] private GameObject submit;

    [SerializeField] private OptionManager[] options;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite defaltSprite;
    [SerializeField] private Sprite wrongSprite;
    public int OptionNumber
    {
        get
        {
            return options[1].isSelected ? 1 : 0;
        }
    }

    public static QuizManager Instance { private set; get; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        ClosePanel();
        guidePanel.SetActive(true);
    }

    private void ClosePanel()
    {
        guidePanel.SetActive(false);
        questionPanel.SetActive(false);
        navigationPanel.SetActive(false);
    }
    private void DefaultOption()
    {
        foreach (var op in options)
        {
            op.ChangeSprite(defaltSprite);
        }
    }
    public void OnSkipButton()
    {
        ClosePanel();
        submit.SetActive(true);
        DefaultOption();
        DeSelectOption();
        wrongAnswer.SetActive(false);
        playAgain.SetActive(false);
        questionPanel.SetActive(true);
    }
    public void OnSubmit()
    {
        ClosePanel();
        if (OptionNumber == 1)
        {
            navigationPanel.SetActive(true);
        }
        else
        {
            questionPanel.SetActive(true);
            submit.SetActive(false);
            playAgain.SetActive(true);
            wrongAnswer.SetActive(true);
            foreach (var op in options)
            {
                if (op.isSelected)
                {
                    op.ChangeSprite(wrongSprite);
                    break;
                }
            }
        }
    }
    private void DeSelectOption()
    {
        foreach (var op in options)
        {
            op.isSelected = false;
        }
    }
    public void SelectOption(OptionManager option)
    {
        DefaultOption();
        DeSelectOption();
        option.isSelected = true;
        option.ChangeSprite(selectedSprite);
    }
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
    public void CollectReward()
    {
        SceneManager.LoadScene(1);
    }
}
