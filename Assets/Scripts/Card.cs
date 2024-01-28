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

    public NamedImage[] icons;

    public Image iconMaleBoomer;
    public Image iconFemaleBoomer;
    public Image iconMaleMillenial;
    public Image iconFemaleMillenial;
    public Image iconMaleGenz;
    public Image iconFemaleGenz;

    public Sprite questionSprite;

    private Dictionary<string, Sprite> iconImages = new Dictionary<string, Sprite>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        comedyActionsLoader = FindObjectOfType<ComedyActionsLoader>();
        iconMaleBoomer = transform.Find("effectIcon1_male_boomer").GetComponent<Image>();
        iconFemaleBoomer = transform.Find("effectIcon1_female_boomer").GetComponent<Image>();
        iconMaleMillenial = transform.Find("effectIcon1_male_millenial").GetComponent<Image>();
        iconFemaleMillenial = transform.Find("effectIcon1_female_millenial").GetComponent<Image>();
        iconMaleGenz = transform.Find("effectIcon1_male_genz").GetComponent<Image>();
        iconFemaleGenz = transform.Find("effectIcon1_female_genz").GetComponent<Image>();

        foreach (var item in icons)
        {
            iconImages[item.name] = item.image;
        }
    }

    private void Start()
    {
        text.text = joke.text;
        title.text = joke.title;
        LoadTitleImage();

    }

    public void SetIcons()
    {
        int remaingTips = 3;
        foreach (var gender in new string[] { "male", "female" })
        {
            foreach (var age in new string[] { "boomer", "millenial", "genz" })
            {

                var enjoyment = 0;
                foreach (var result in joke.result)
                {
                    if (result.audience == age || result.audience == gender)
                    {
                        enjoyment += result.value;
                    }
                }

                var modifier = "";
                if (enjoyment > 0)
                {
                    modifier = "happy";
                }
                else if (enjoyment < 0)
                {
                    modifier = "unhappy";
                }
                Image currentIcon = iconMaleBoomer;
                var bla = gender + "_" + age;
                if (bla == "female_boomer")
                {
                    currentIcon = iconFemaleBoomer;
                }
                else if (bla == "male_millenial")
                {
                    currentIcon = iconMaleMillenial;
                }
                else if (bla == "female_millenial")
                {
                    currentIcon = iconFemaleMillenial;
                }
                else if (bla == "male_genz")
                {
                    currentIcon = iconMaleGenz;
                }
                else if (bla == "female_genz")
                {
                    currentIcon = iconFemaleGenz;
                }

                var audience = gender + "_" + age + "_" + modifier;
                if (modifier != "")
                {


                    if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    {

                        currentIcon.sprite = iconImages[audience];
                        remaingTips -= 1;
                    }
                    else
                    {


                    }
                }
                else
                {


                }

                if (remaingTips <= 0)
                {
                    return;
                }

            }
        }

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

            case "Story":
                image.sprite = lastImage;
                break;
        }
    }
}
