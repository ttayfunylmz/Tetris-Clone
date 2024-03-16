using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameBoard board;
    [SerializeField] private Button gameOverButton;

    private CanvasGroup canvasGroup;
    private float fadeDuration = 1f;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();    
    }

    private void Start() 
    {
        board.OnGameOver += GameBoard_OnGameOver;

        Hide();

        gameOverButton.onClick.AddListener(() =>
        {
            Hide();
            board.RestartGame();
            AudioManager.Instance.Play(Consts.Audio.CLICK_SOUND);
        });
    }

    private void GameBoard_OnGameOver()
    {
        Show();
    }

    private void Show()
    {
        gameOverButton.gameObject.SetActive(true);
        canvasGroup.DOFade(1f, fadeDuration);
    }

    private void Hide()
    {
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            gameOverButton.gameObject.SetActive(false);
        });
    }

    private void OnDisable() 
    {
        board.OnGameOver -= GameBoard_OnGameOver;    
    }
}
