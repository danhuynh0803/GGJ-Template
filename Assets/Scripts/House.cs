﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour
{
    public int maxScore = 12;
    public GameObject[] houseProgressionModels;
    public GameObject chimneyObject; // Serves as the final object
    public Material p1Color, p2Color;

    private int currScore = 0;
    private int redPoint = 0;
    private int bluePoint = 0;

    [Header("Player point images")]
    public Image redBar, blueBar;

    [Header("Camera stuff for win panel")]
    public GameObject cameraOne;
    public GameObject cameraTwo;
    public GameObject cameraThree;
    public GameObject playerRed;
    public GameObject playerBlue;
    public GameObject redWins;
    public GameObject blueWins;
    public GameObject blueWinner;
    public GameObject redWinner;

    [Header("Final round parameters")]
    public int  finalRoundTimer = 30;
    private float timer = 0.0f;
    private bool isFinalRound = false;
    private bool hasDisplayedFinalRoundText = false;
    private bool isGameOver = false;

    private Stack<GameObject> redObjects, blueObjects;

    void Start()
    {
        currScore = 0;
        timer = finalRoundTimer;

        redObjects = new Stack<GameObject>();
        blueObjects = new Stack<GameObject>();

        cameraThree.SetActive(false);
        redWins.SetActive(false);
        redWinner.SetActive(false);
        blueWins.SetActive(false);
        blueWinner.SetActive(false);
    }

    void Update()
    {
        // Update player fill bars
        redBar.fillAmount = (float)redPoint / (float)maxScore;
        blueBar.fillAmount = (float)bluePoint / (float)maxScore;

        if (isFinalRound && !isGameOver)
        {
            if (!hasDisplayedFinalRoundText)
            {
                InfoBoard.instance.AddText("FINAL ROUND!", 1.0f);
                hasDisplayedFinalRoundText = true;
            }

            timer -= Time.deltaTime;

            InfoBoard.instance.DisplayText(Mathf.CeilToInt(timer).ToString());

            // Make chimney match color of the current winning player
            chimneyObject.SetActive(true);
            if (redPoint > bluePoint)
            {
                chimneyObject.GetComponent<Renderer>().material = p1Color;
            }
            else
            {
                chimneyObject.GetComponent<Renderer>().material = p2Color;
            }

            if (timer <= 0)
            {
                SoundController.Play(SFX.whistle, 0.5f);
                StartCoroutine("EndGame");
                InfoBoard.instance.AddText("FINISHED!", 1.0f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Absorb fragments
        // TODO refactor so that it doesnt need to
        // check that the object was fired
        if (other.gameObject.GetComponent<Orbiting>() != null)
        {
            // TODO play a construction noise (hammers, saws, etc)
            // Add a wall segment and change the color
            Player controllingPlayer = other.gameObject.GetComponent<Orbiting>().controllingPlayer;

            if (!isFinalRound)
            {
                if (controllingPlayer == Player.P1)
                {
                    houseProgressionModels[currScore].GetComponent<Renderer>().material = p1Color;
                    redObjects.Push(houseProgressionModels[currScore]);
                    redPoint++;
                }
                else if (controllingPlayer == Player.P2)
                {
                    houseProgressionModels[currScore].GetComponent<Renderer>().material = p2Color;
                    blueObjects.Push(houseProgressionModels[currScore]);
                    bluePoint++;
                }

                houseProgressionModels[currScore].SetActive(true);
                currScore++;
            }
            else
            {
                if (controllingPlayer == Player.P1)
                {
                    if (blueObjects.Count > 0)
                    {
                        bluePoint--;
                        redPoint++;
                        GameObject wall = blueObjects.Pop();
                        wall.GetComponent<Renderer>().material = p1Color;
                        redObjects.Push(wall);
                    }
                }
                else if (controllingPlayer == Player.P2)
                {
                    if (redObjects.Count > 0)
                    {
                        redPoint--;
                        bluePoint++;
                        GameObject wall = redObjects.Pop();
                        wall.GetComponent<Renderer>().material = p2Color;
                        blueObjects.Push(wall);
                    }
                }
            }

            Destroy(other.gameObject);

            SoundController.Play(5, 0.1f); //play point noise

            if (currScore >= maxScore)
            {
                isFinalRound = true;
            }
        }
    }
    IEnumerator EndGame()
    {
        isGameOver = true;

        yield return new WaitForSeconds(3.0f);

        // Clear out timer text
        InfoBoard.instance.DisplayText("");

        //cameraOne.SetActive(false);
        cameraOne.GetComponent<Camera>().enabled = false;
        cameraTwo.SetActive(false);
        cameraThree.SetActive(true);
        playerRed.SetActive(false);
        playerBlue.SetActive(false);

        if (redPoint > bluePoint)
        {
            redWins.SetActive(true);
            redWinner.SetActive(true);
        }
        else
        {
            blueWins.SetActive(true);
            blueWinner.SetActive(true);
        }

    }
}
