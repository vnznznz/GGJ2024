using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Bar : MonoBehaviour
{
    public float maxHeight = 512;
    public float currentValue = 0;

    public Color minColor = Color.white;
    public Color maxColor = Color.red;

    private RectTransform rectTransform;
    private UnityEngine.UI.Image image;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentValue * maxHeight);

        this.image.color = Color.Lerp(minColor, maxColor, currentValue);
    }
}
