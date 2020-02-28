using UnityEngine;

public class ToolSwitchUiManager : MonoBehaviour
{
    private void Update()
    {
        float targetOffset = MainUiScript.Instance.Tool == MainUiScript.ToolMode.Arrows ? 0 : -72;
        float newX = Mathf.Lerp(transform.localPosition.x, targetOffset, Time.deltaTime * 10);
        transform.localPosition = new Vector3(newX, 0, 0);
    }
}