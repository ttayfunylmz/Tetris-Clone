using UnityEngine;
using DG.Tweening;

public class ImagePolishController : MonoBehaviour
{
    private float scaleDuration = 0.1f;
    
    public void OnImageHoverEnter(Transform targetObject)
    {
        targetObject.DOScale(1.1f, scaleDuration);
        AudioManager.Instance.Play(Consts.Audio.HOVER_SOUND);
    }

    public void OnImageHoverExit(Transform targetObject)
    {
        targetObject.DOScale(1f, scaleDuration);
    }
}
