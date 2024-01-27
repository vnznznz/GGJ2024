using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Schema;

public class Card : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }
    public CardManager cardManager;

    public Vector3 newPos;
    public Vector3 newScale;

    public Vector3 startPos;
    public Vector3 startScale;

    public int index;

    public TextMeshProUGUI text;
    public TextMeshProUGUI title;

    public ComedyAction joke;

    ComedyActionsLoader comedyActionsLoader;

    public Sprite punImage;
    public Sprite parodyImage;
    public Sprite blondeImage;
    public Sprite slapstickImage;
    public Sprite lastImage;

    public Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        comedyActionsLoader = FindObjectOfType<ComedyActionsLoader>();

    }

    private void Start()
    {
        text.text = joke.text;
        title.text = joke.title;
        LoadTitleImage();
    }

    private void Update()
    {
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, newPos, Time.deltaTime * cardManager.lerpTime);
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, newScale, Time.deltaTime * cardManager.lerpTime);
    }

    public void OnPointerEnterEvent()
    {
        newPos = rectTransform.localPosition + new Vector3(0, cardManager.selectOffset, 0);
        newScale = rectTransform.localScale + new Vector3(cardManager.sizeIncrease, cardManager.sizeIncrease, 0);
        cardManager.activeCard = index;
        print(index);
    }

    public void OnPointerExitEvent()
    {
        newPos = startPos;
        newScale = startScale;
        cardManager.activeCard = -1;
    }

    public void ClickEvent()
    {
        cardManager.TriggerCardActivation(this);
    }

    public void UseOldCard()
    {
        GameManager.Instance.TellAJoke(joke);
        Destroy(gameObject);
    }

    private void LoadTitleImage()
    {

        switch (title.text)
        {
            case "Pun":
                image.sprite = punImage;
                break;

            case "Parody":
                image.sprite = parodyImage;
                break;

            case "Blonde":
                image.sprite = blondeImage;
                break;

            case "Slapstick":
                image.sprite = slapstickImage;
                break;

            case "Wordplay":
                image.sprite = lastImage;
                break;
        }
    }
}
