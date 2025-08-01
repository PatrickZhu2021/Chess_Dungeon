using UnityEngine;

public class GameMetricsInitializer : MonoBehaviour
{
    [Header("Auto-initialize GameMetrics")]
    public bool autoInitialize = true;
    
    private void Awake()
    {
        if (autoInitialize && GameMetrics.Instance == null)
        {
            GameObject metricsObject = new GameObject("GameMetrics");
            metricsObject.AddComponent<GameMetrics>();
            DontDestroyOnLoad(metricsObject);
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && GameMetrics.Instance != null)
        {
            // 游戏恢复时保存数据
            GameMetrics.Instance.EndSession(false);
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && GameMetrics.Instance != null)
        {
            // 失去焦点时保存数据
            GameMetrics.Instance.EndSession(false);
        }
    }
    
    private void OnDestroy()
    {
        if (GameMetrics.Instance != null)
        {
            GameMetrics.Instance.EndSession(false);
        }
    }
}