using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EditorLabelMB : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Transform scaleTransform;
    public float levelScale;
    public Image image;

    public void Init(string text, float levelScale, Vector2 position, Color color)
    {
        this.levelScale = levelScale;
        gameObject.transform.position = position;
        scaleTransform.localScale = new Vector3(levelScale, levelScale, 1);
        label.text = text;
        image.color = color;
    }

    private void Awake()
    {
        Destroy(gameObject);
    }
}
