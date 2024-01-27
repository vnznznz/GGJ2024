using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private ComedyAction joke;

    ComedyActionsLoader comedyActionsLoader;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        comedyActionsLoader = FindObjectOfType<ComedyActionsLoader>();
    }

    private void Start()
    {
        newPos = rectTransform.position;
        newScale = rectTransform.localScale;
        MakeJoke();
    }

    private void Update()
    {  
        rectTransform.position = Vector3.Lerp(rectTransform.position, newPos, Time.deltaTime * cardManager.lerpTime);
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, newScale, Time.deltaTime * cardManager.lerpTime);
    }

    public void OnPointerEnterEvent()
    {
        newPos = rectTransform.position + new Vector3(0, cardManager.selectOffset, 0);
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
        cardManager.LoadNewCard(index);
    }

    public void UseOldCard()
    {
        GameManager.Instance.TellAJoke(joke);
        Destroy(gameObject);
    }

    private void MakeJoke()
    {
        joke = comedyActionsLoader.comedyActions[UnityEngine.Random.Range(0, comedyActionsLoader.comedyActions.Length)];
        text.text = joke.text;
        title.text = joke.title;

    }
}
