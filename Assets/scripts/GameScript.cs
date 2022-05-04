using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject cardPrefab;
    public Camera cam;

    private List<GameObject> deck = new List<GameObject>();
    private Vector3 deckLocation;

    public int deckSize = 15; // num of cards total
    public int rowLength = 5; // num of cards for a row
    public bool flip = false; // do flip animation

    // TODO
    // add 3d collider to each card
    // allow user to click on card - if card !isMoving, flip the card
    // create an interface to allow playing each animation, maybe modify num cards, etc.

    public enum configOptions {
        Rows,
        Splayed, 
        SplayedSymmetrical,
        Rainbow,
        Circle
    };

    public configOptions configuration;

    private void createDeck(int numCards, Vector3 deckLocation)
    {
        this.deckLocation = deckLocation;

        for (int i = 0; i < numCards; i++)
        {
            // make sure the cards are stacked (so change the z-value for each card)
            float newZ = deckLocation.z - (i+1)*0.05f;
            Vector3 startPos = new Vector3(deckLocation.x, deckLocation.y, newZ);

            GameObject card = Instantiate(cardPrefab, startPos, Quaternion.Euler(0, 0, 0));

            card.AddComponent<BoxCollider>();
            card.AddComponent<CardScript>();
            card.GetComponent<CardScript>().setStartPosition(startPos);
            card.GetComponent<CardScript>().Name = "card" + i;

            this.deck.Add(card);
        }
    }

    private void setFlip(bool flip)
    {
        foreach(GameObject card in this.deck){
            CardScript script = card.GetComponent<CardScript>();
            if (flip)
            {
                script.toggleFlip(true);
            }
            else
            {
                script.toggleFlip(false);
            }
        }
    }

    private void setCardEndPositions(configOptions configuration)
    {
        // TODO: each configuration logic should be broken out into its own function

        int rowLength = this.rowLength;
        int numRows = (int)Math.Ceiling(this.deck.Count / (float)rowLength);

        switch (configuration)
        {
            case configOptions.Rows:
                CardAnimations.makeRows(numRows, rowLength, this.deck);
                break;
            case configOptions.Rainbow:
                CardAnimations.makeRainbow(deck);
                break;
            case configOptions.Splayed:
                CardAnimations.makeSplayed(numRows, rowLength, deck);
                break;
            case configOptions.SplayedSymmetrical:
                CardAnimations.makeSplayedSymmetrical(numRows, rowLength, deck);
                break;
            case configOptions.Circle:
                CardAnimations.makeCircle(deckLocation, deck);
                break;
        }
    }

    IEnumerator placeCard()
    {
        // notice we start backwards because we want to start at the top of the deck first
        for(int i = this.deck.Count - 1; i >= 0; i--)
        {
            this.deck[i].GetComponent<CardScript>().placeCard();

            // add a delay between each card
            yield return new WaitForSeconds(0.2f);
        }
    }

    // for selecting a card and flipping it with a mouse-click
    void shootRay()
    {
        RaycastHit hit;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out hit))
        {
            if (hit.collider != null)
            {
                CardScript cardScript = hit.transform.GetComponent<CardScript>();
                if (cardScript != null)
                {
                    //Debug.Log("ray hit: " + hit.transform.GetComponent<CardScript>().Name);
                    //Debug.Log("is static: " + hit.transform.GetComponent<CardScript>().isStatic());
                    if (cardScript.isStatic())
                    {
                        // TODO: flip the card
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        createDeck(this.deckSize, new Vector3(-7, 4, -2));
        setCardEndPositions(configuration);
        setFlip(this.flip);
        StartCoroutine("placeCard");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shootRay();
        }
    }
}
