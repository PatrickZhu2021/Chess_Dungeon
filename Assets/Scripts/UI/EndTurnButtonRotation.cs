using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndTurnButtonRotation : MonoBehaviour
{
    private Button endTurnButton;
    private RectTransform buttonRect;
    private bool isRotated = false;
    
    void Start()
    {
        endTurnButton = GetComponent<Button>();
        buttonRect = GetComponent<RectTransform>();
        
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
    }
    
    void OnEndTurnClicked()
    {
        if (!isRotated)
        {
            StartCoroutine(RotateButton(180f));
            isRotated = true;
        }
    }
    
    public void OnPlayerTurnStart()
    {
        if (isRotated)
        {
            StartCoroutine(RotateButton(-180f));
            isRotated = false;
        }
    }
    
    private IEnumerator RotateButton(float targetRotation)
    {
        float startRotation = buttonRect.eulerAngles.z;
        float endRotation = startRotation + targetRotation;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentRotation = Mathf.Lerp(startRotation, endRotation, elapsed / duration);
            buttonRect.rotation = Quaternion.Euler(0, 0, currentRotation);
            yield return null;
        }
        
        buttonRect.rotation = Quaternion.Euler(0, 0, endRotation);
    }
}