using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    public GameObject cardPrefab;

    private List<GameObject> deck = new List<GameObject>();

    private void createDeck(int numCards, Vector3 deckLocation)
    {
        for (int i = 0; i < numCards; i++)
        {
            // make sure the cards are stacked (so change the z-value for each card)
            float newZ = deckLocation.z - (i+1)*0.05f;
            Vector3 startPos = new Vector3(deckLocation.x, deckLocation.y, newZ);
            GameObject card = Instantiate(cardPrefab, startPos, Quaternion.Euler(0, 0, 0));
            card.AddComponent<CardScript>();
            card.GetComponent<CardScript>().setStartPosition(startPos);

            this.deck.Add(card);
        }
    }

    private void setCardEndPositions(string configuration)
    {
        // TODO: pass in an enum representing some kind of pattern for the cards to end up in?
        // default can be some m x n configuration? have different parameters?

        int rowLength = 5; // place cards in rows of 5
        int numRows = (int)Math.Ceiling(this.deck.Count / (float)rowLength);

        if (configuration == "rows")
        {
            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                int newX = offsetFromDeck;
                int newY = 3 - (i * 2);

                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2; // kinda need to know the width of a card??
                    CardScript script = this.deck[j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(newX, newY, -2));
                    script.toggleFlip(false);
                }
            }
        }else if(configuration == "splayed")
        {
            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                float newX = offsetFromDeck;
                int newY = 3 - (int)(i * 1.2);
                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2.2f;
                    CardScript script = this.deck[j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(newX, newY, -2));

                    // we're taking advantage of the different rates of y-rotation and rotating about x
                    // as well to produce a varying skewed placement of each card about the z-axis.
                    script.setXRotationAngle((float)Math.PI/3);

                    script.toggleFlip(false);
                }
            }
        }
        else if (configuration == "splayed-symmetrical")
        {
            // cards should be angled like so: / -> | -> \
            // figure out max and min angles (about the z-axis) that the cards should be at
            // how many for each row, how much to space out
            // for each subsequent row, stack on preceding row? just slightly moving towards the center each row (so in each subsequent row the cards will look closer together)
            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                float newX = offsetFromDeck;
                int newY = 3 - (int)(i * 1.2);
                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2.2f;
                    CardScript script = this.deck[j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(newX, newY, -2));

                    // TODO
                }
            }
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

    // Start is called before the first frame update
    void Start()
    {
        // create a deck of cards
        createDeck(15, new Vector3(-7, 3, -2));
        setCardEndPositions("splayed");
        StartCoroutine("placeCard");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
