using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CardManager : MonoBehaviour
{
    Card[] cards;
    public GameObject cardPrefab;

    private float defaultYPos = -350;
    private float xPadding = 350;

    public float sizeIncrease;
    public float selectOffset;
    public float lerpTime;
    public float maxCards = 3;

    public int activeCard = -1;

    private Bar timeBar;

    private Bar audienceBar;

    private TextMeshProUGUI gameTimeText;
    public float pauseTime = 3;
    public float selectionTime = 3;
    private float selectionAccu = 0;

    private bool cardsBlocked = false;
    private void Start()
    {
        timeBar = transform.Find("TimebarBar").GetComponent<Bar>();
        audienceBar = transform.Find("AudienceBar").GetComponent<Bar>();
        gameTimeText = transform.Find("TimeLeft").GetComponent<TextMeshProUGUI>();

        cardsBlocked = true;
        Invoke("LoadCards", 3);
    }
    private void Update()
    {

        selectionAccu += Time.deltaTime;

        if (selectionAccu > selectionTime)
        {
            selectionAccu = 0;
            GameManager.Instance.MissJoke();
            ClearCards();
            cardsBlocked = true;
            Invoke("LoadCards", pauseTime);
        }

        if (cardsBlocked)
        {
            selectionAccu = 0;
        }

        timeBar.currentValue = selectionAccu / selectionTime;
        audienceBar.currentValue = GameManager.Instance.getAudienceSatisfaction();
        gameTimeText.text = $"Time left:\n {GameManager.Instance.getTimeLeft():.00}";

        if (transform.GetComponentsInChildren<Card>().Length == 0 && GameManager.Instance.gameState == GameManager.GameState.Round)
        {
            //LoadCards();
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            GetCardByIndex(1);
        }
    }

    private void ClearCards()
    {
        foreach (var card in transform.GetComponentsInChildren<Card>())
        {
            Destroy(card.gameObject);
        }
    }
    private void LoadCards()
    {
        selectionAccu = 0;
        float xPos = -xPadding;
        var jokes = GameManager.Instance.comedyActionsLoader.GetRandomComedyActions((uint)Mathf.FloorToInt(maxCards));

        for (int i = 0; i < maxCards; i++)
        {
            GameObject cardInstance = Instantiate(cardPrefab, this.transform);
            Card card = cardInstance.GetComponent<Card>();
            RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();
            var position = new Vector3(xPos, defaultYPos, 0);
            rectTransform.localPosition = new Vector3(-590, -800, 0);

            card.cardManager = this;
            card.startPos = position;
            card.startScale = rectTransform.localScale;
            card.newPos = position;
            card.newScale = rectTransform.localScale;
            card.index = i;
            card.joke = jokes[i];
            xPos += xPadding;
        }
        cardsBlocked = false;
    }

    public void TriggerCardActivation(Card card)
    {

        GameManager.Instance.TellAJoke(card.joke);
        ClearCards();
        cardsBlocked = true;
        Invoke("LoadCards", pauseTime);
    }

    public void LoadNewCard(int index)
    {
        Card oldCard = GetCardByIndex(index);
        Vector3 pos = oldCard.startPos;
        Vector3 scale = oldCard.startScale;
        Vector3 startPos = oldCard.startPos;
        Vector3 startScale = oldCard.startScale;

        GameObject newCard = Instantiate(cardPrefab, pos - new Vector3(0, 500, 0), cardPrefab.transform.rotation, this.transform);
        newCard.GetComponent<Card>().cardManager = this;
        newCard.GetComponent<Card>().index = index;

        StartCoroutine(SetNewPos(newCard, pos, scale, startPos, startScale));
        oldCard.UseOldCard();

    }


    private Card GetCardByIndex(int index)
    {
        foreach (Transform child in this.transform)
        {
            Card card = child.GetComponent<Card>();
            if (card.index == index)
            {
                return card;
            }
        }
        return null;
    }

    public void Awake()
    {

    }

    private IEnumerator SetNewPos(GameObject cardObj, Vector3 newPos, Vector3 newScale, Vector3 oldPos, Vector3 oldScale)
    {
        Card card = cardObj.GetComponent<Card>();
        yield return new WaitForSeconds(1);
        card.newPos = newPos;
        card.newScale = newScale;
        card.startPos = oldPos;
        card.startScale = oldScale;
    }
}
