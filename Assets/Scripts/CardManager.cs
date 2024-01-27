using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        print(activeCard);
        LoadStartCards();

        if (Input.GetKeyDown(KeyCode.L))
        {
            GetCardByIndex(1);
        }
    }

    private void LoadStartCards()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float xPos = -xPadding;
            for (int i = 0; i < maxCards; i++)
            {
                GameObject cardInstance = Instantiate(cardPrefab, this.transform);
                Card card = cardInstance.GetComponent<Card>();
                RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(xPos, defaultYPos, 0);

                card.cardManager = this;
                card.startPos = rectTransform.position;
                card.startScale = rectTransform.localScale;
                card.index = i;

                xPos += xPadding;
            }
        }
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
                print(card.index);
                return card;
            }
        }
        return null;
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
