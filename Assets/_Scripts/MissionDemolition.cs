﻿// Author: Omar Elansary
// CSC476
// Mission Demolition

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;
    static public int score0 = 15;
    static public int score1 = 20;
    static public int score2 = 25;

    [Header("Set in Inspector")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitButton;
    public Text uitScore;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";
 
    // Use this for initialization
    void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void Awake()
    {      
        if (PlayerPrefs.HasKey("HighScore"))
        {
            score0 = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", score0);

        if (PlayerPrefs.HasKey("HighScore1"))
        {
            score1 = PlayerPrefs.GetInt("HighScore1");
        }
        PlayerPrefs.SetInt("HighScore1", score1);

        if (PlayerPrefs.HasKey("HighScore2"))
        {
            score2 = PlayerPrefs.GetInt("HighScore2");
        }
        PlayerPrefs.SetInt("HighScore2", score2);      
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    public void UpdateGUI()
    {

        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;

        if (level == 0)
        {
            uitScore.text = "Shots to Beat: " + score0;
        }

        if (level == 1)
        {
            uitScore.text = "Shots to Beat: " + score1;
        }

        if (level == 2)
        {
            uitScore.text = "Shots to Beat: " + score2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(score1);
        // sets the new highscore if play shots is less than 
        // playerprefs value, this also changes depending on level.
        if (level == 0)
        {
            if (shotsTaken < PlayerPrefs.GetInt("HighScore") && Goal.goalMet)
            {
                PlayerPrefs.SetInt("HighScore", shotsTaken);
                uitScore.text = "Shots to Beat: " + shotsTaken;
            }
        }

        if (level == 1)
        {
            if (shotsTaken < PlayerPrefs.GetInt("HighScore1") && Goal.goalMet)
            {
                PlayerPrefs.SetInt("HighScore1", shotsTaken);
                uitScore.text = "Shots to Beat: " + shotsTaken;
            }
        }

        if (level == 2)
        {
            if (shotsTaken < PlayerPrefs.GetInt("HighScore2") && Goal.goalMet)
            {
                PlayerPrefs.SetInt("HighScore2", shotsTaken);
                uitScore.text = "Shots to Beat: " + shotsTaken;
            }
        }

        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);

        }
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
            // gets and sets the previously saved highscores, 
            // otherwise they would reset to the default highscores on new game.
            Awake();
        }

        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    public static void ShotFired()
    {
        S.shotsTaken++;
    }
}
