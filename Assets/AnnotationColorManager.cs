using UnityEngine;

public class AnnotationColorManager : MonoBehaviour
{
    private int colorPickIndex;

    public Color AnnotationColor { get; set; }

    [SerializeField]
    private Color[] colors;
    public Color[] Colors { get { return this.colors; } }

    public static AnnotationColorManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AnnotationColor = Colors[0];
    }

    public void GoToNextColor()
    {
        colorPickIndex++;
        colorPickIndex %= Colors.Length;
        AnnotationColor = Colors[colorPickIndex];
    }
}