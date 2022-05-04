using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardAnimations
{
    public static void makeCircle(Vector3 deckLocation, List<GameObject> deck)
    {
        // TODO: this one gets kinda weird when flipping is turned on
        // maybe need to take into account final expected rotation?

        float angleSlice = (float)(360 / deck.Count);
        float currAngle = 0f;
        float centerX = deckLocation.x + 6; // these are arbitrary values. TODO: get the center coords of the screen?
        float centerY = deckLocation.y - 3;
        float radius = 4.5f;

        // just use this.deck.count to iterate through the cards since we're not arranging in a row format here
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            CardScript script = deck[i].GetComponent<CardScript>();

            float x = radius * (float)Math.Cos(currAngle * (Math.PI / 180));
            float y = radius * (float)Math.Sin(currAngle * (Math.PI / 180));

            script.setEndPosition(new Vector3(centerX + x, centerY + y, -2));

            // add 90 deg to orient the cards so they point towards the center of the circle
            script.setZRotationAngle((float)(currAngle * (Math.PI / 180)) + Mathf.PI / 2);

            currAngle += angleSlice;
        }
    }

    public static void makeRows(int numRows, int rowLength, List<GameObject> deck)
    {
        for (int i = 0; i < numRows; i++)
        {
            int offsetFromDeck = -6;
            int newX = offsetFromDeck;
            int newY = 3 - (i * 2);

            for (int j = i * rowLength; j < Math.Min((i * rowLength) + rowLength, deck.Count); j++)
            {
                newX += 2; // kinda need to know the width of a card to be able to space them properly?
                CardScript script =deck[deck.Count - 1 - j].GetComponent<CardScript>(); // start from top of deck
                script.setEndPosition(new Vector3(newX, newY, -2));
            }
        }
    }

    public static void makeSplayed(int numRows, int rowLength, List<GameObject> deck)
    {
        float zDelta = 0.05f; // how much to elevate about the z-axis for each card so they lay on top of each other properly when placed
        float zPos = -2;

        for (int i = 0; i < numRows; i++)
        {
            int offsetFromDeck = -6;
            float newX = offsetFromDeck;
            int newY = 3 - (int)(i * 1.2);
            float rotAngle = (float)Math.PI / 4;
            int count = 0;

            for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, deck.Count); j++)
            {
                newX += 2.2f;
                CardScript script = deck[deck.Count - 1 - j].GetComponent<CardScript>();
                script.setEndPosition(new Vector3(newX, newY, zPos - (i * zDelta)));
                script.setZRotationAngle(rotAngle - 0.15f * count++);
            }
        }
    }

    public static void makeSplayedSymmetrical(int numRows, int rowLength, List<GameObject> deck)
    {
        float zDelta = 0.05f; // how much to elevate about the z-axis for each card so they lay on top of each other properly when placed
        float zPos = -2;

        // cards should be angled kinda like: / -> | -> \
        float startAngle = (float)(Math.PI / 6); // start angle
        float arcAngle = (float)Math.PI / 3; // we want the cards spread over a 60 deg arc
        float angleSlice = arcAngle / (rowLength - 1); // the angle between each card

        for (int i = 0; i < numRows; i++)
        {
            int offsetFromDeck = -6;
            float newX = offsetFromDeck;
            int newY = 3 - (int)(i * 1.2);
            float currAngle = startAngle;

            for (int j = i * rowLength; j < Math.Min(i * rowLength + rowLength, deck.Count); j++)
            {
                newX += 2.2f;
                CardScript script = deck[deck.Count - 1 - j].GetComponent<CardScript>();
                script.setEndPosition(new Vector3(newX, newY, zPos - (i * zDelta)));
                script.setZRotationAngle(currAngle);
                currAngle -= angleSlice;
            }
        }
    }

    public static void makeRainbow(List<GameObject> deck)
    {
        // TODO: look at the arc that's formed for this. it doesn't seem symmetrical. adjust arc height maybe?
        // just use this.deck.count to iterate through the cards since we're not arranging in a row format here
        int offsetFromDeck = -6;
        int newX = offsetFromDeck;
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            newX += 2;
            float newZ = deck[deck.Count - 1 - i].transform.position.z; // top-most card should end up at bottom of new stack, etc.
            CardScript script = deck[i].GetComponent<CardScript>();
            script.setEndPosition(new Vector3(6, 4, newZ));
        }
    }

}
