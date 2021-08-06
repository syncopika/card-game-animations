using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    public GameObject cardPrefab;

    private List<GameObject> deck = new List<GameObject>();
    private Vector3 deckLocation;

    private void createDeck(int numCards, Vector3 deckLocation)
    {
        this.deckLocation = deckLocation;

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

                for (int j = i * rowLength; j < Math.Min((i * rowLength) + rowLength, this.deck.Count); j++)
                {
                    newX += 2; // kinda need to know the width of a card to be able to space them properly?
                    CardScript script = this.deck[this.deck.Count - 1 - j].GetComponent<CardScript>(); // start from top of deck
                    script.setEndPosition(new Vector3(newX, newY, -2));
                    script.toggleFlip(true);
                }
            }
        }
        else if(configuration == "rainbow")
        {
            // TODO: look at the arc that's formed for this. it doesn't seem symmetrical. adjust arc height maybe?
            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                int newX = offsetFromDeck;

                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2;
                    float newZ = this.deck[this.deck.Count - 1 - j].transform.position.z; // top-most card should end up at bottom of new stack, etc.
                    CardScript script = this.deck[j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(6, 3, newZ));
                    script.toggleFlip(true);
                }
            }
        }
        else if(configuration == "splayed")
        {
            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                float newX = offsetFromDeck;
                int newY = 3 - (int)(i * 1.2);
                float rotAngle = (float)Math.PI / 4;

                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2.2f;
                    CardScript script = this.deck[this.deck.Count - 1 - j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(newX, newY, -2));

                    // when flipping, we're taking advantage of the different rates of y-rotation and rotating about x
                    // as well to produce a varying skewed placement of each card about the z-axis.
                    script.setXRotationAngle(rotAngle);

                    script.toggleFlip(false);

                    // if card flip not toggled, change the rotAngle for each card
                    if (!script.isFlip())
                    {
                        rotAngle -= (float)(Math.PI / 14);
                    }
                }
            }
        }
        else if (configuration == "splayed-symmetrical")
        {
            // cards should be angled like so: / -> | -> \
            float startAngle = (float) (Math.PI / 6); // start angle
            float arcAngle = (float)Math.PI / 3; // 60 deg arc
            float angleSlice = arcAngle / (rowLength-1); // the angle between each card

            for (int i = 0; i < numRows; i++)
            {
                int offsetFromDeck = -6;
                float newX = offsetFromDeck;

                int newY = 3 - (int)(i * 1.2);

                float currAngle = startAngle;

                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    newX += 2.2f;
                    CardScript script = this.deck[this.deck.Count - 1 - j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(newX, newY, -2));

                    script.setXRotationAngle(currAngle);

                    currAngle -= angleSlice;

                    script.toggleFlip(false);
                }
            }
        }
        else if(configuration == "circle")
        {
            float angleSlice = (float)(360 / rowLength);
            float currAngle = 0f;
            float centerX = deckLocation.x + 6;
            float centerY = deckLocation.y - 2;
            // how about radius?
            float radius = 4.5f;
            for (int i = 0; i < numRows; i++)
            {
                for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, this.deck.Count); j++)
                {
                    CardScript script = this.deck[j].GetComponent<CardScript>();
                    script.setEndPosition(new Vector3(centerX + radius * (float)Math.Cos(currAngle), centerY + radius * (float)Math.Sin(currAngle), -2));
                    //script.setEndPosition(new Vector3(centerX, centerY, -2));

                    script.setXRotationAngle(currAngle); // we should orient the cards so they point towards the center of the circle, not along the tangent which this does

                    currAngle += angleSlice;
                    //script.toggleFlip(false);
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
        setCardEndPositions("circle");
        StartCoroutine("placeCard");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
