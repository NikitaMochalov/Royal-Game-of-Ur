using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicesScoreScript : MonoBehaviour
{
    public GameObject[] dices;
    public GameObject gameController;
    protected Transform startPos;

    public int score;

    public bool previousPlayerGotZero;

    public bool throwEnded;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController");
        score = -1;
        previousPlayerGotZero = false;
        throwEnded = false;
    }

    void Update()
    {
        //if(throwInProcces)
        if(!previousPlayerGotZero)
        score = Count();
    }

    public void Throw()
    {
        throwEnded = false;

        gameController.GetComponent<GameControllerScript>().moveWhenDicesWasThrownLast = gameController.GetComponent<Events>().move;

        if (previousPlayerGotZero)
            previousPlayerGotZero = false;

        gameController.GetComponent<GameControllerScript>().DehighlightEverything();

        foreach (GameObject dice in dices)
        {
            dice.transform.position = new Vector3(dice.GetComponent<DiceScript>().startPos.x, dice.GetComponent<DiceScript>().startPos.y, dice.GetComponent<DiceScript>().startPos.z);
            dice.GetComponent<Rigidbody>().rotation = UnityEngine.Random.rotation;
        }

    }

    int Count()
    {
        int diceScore = 0;

        foreach (GameObject dice in dices)
        {
            Vector3 diceVelocity = dice.GetComponent<Rigidbody>().velocity;

            if (diceVelocity.x != 0 ||
                diceVelocity.y != 0 ||
                diceVelocity.z != 0)
                return -1;
            else if (!dice.GetComponent<DiceScript>().IsStandingStraight().Item1)
            {
                Vector3 oldPos = dice.GetComponent<Transform>().position;
                dice.transform.position = new Vector3(Random.Range(oldPos.x - 1, oldPos.x + 1), oldPos.y + 1, Random.Range(oldPos.z - 1, oldPos.z + 1));
                dice.GetComponent<Rigidbody>().rotation = UnityEngine.Random.rotation;
                return -2;
            }
            else if (dice.GetComponent<DiceScript>().GivesPoint())
                diceScore++;
        }

        if (gameController != null && DicesStopped())
        {
            if (diceScore == 0)
            {
                gameController.GetComponent<GameControllerScript>().ScoreInCurrentThrow(diceScore);
                gameController.GetComponent<GameControllerScript>().PlayerGotZero();
                previousPlayerGotZero = true;
            }
            else if (!gameController.GetComponent<GameControllerScript>().MovingIsPossible(diceScore)
                && gameController.GetComponent<GameControllerScript>().moveWhenDicesWasThrownLast == gameController.GetComponent<Events>().move)
            {
                gameController.GetComponent<GameControllerScript>().ScoreInCurrentThrow(diceScore);
                gameController.GetComponent<GameControllerScript>().PlayerCantMove();
                previousPlayerGotZero = true;
            }
            else 
            {
                gameController.GetComponent<GameControllerScript>().ScoreInCurrentThrow(diceScore);

                //Если сейчас ходит бот, он может сходить после остановки костей
                //if (gameController.GetComponent<GameControllerScript>().currentPlayer == 2)
                    //&& gameController.GetComponent<GameControllerScript>().DiceWasThrownThisTurn())
                //{
                    //gameController.GetComponent<GameControllerScript>().BotMove();
                //}
            }
            
            //gameController.GetComponent<GameControllerScript>().moveWhenDicesWasThrownLast = gameController.GetComponent<Events>().move;
        }

        return diceScore; 
    }

    public bool DicesStopped()
    {
        foreach (GameObject dice in dices)
        {
            Vector3 diceVelocity = dice.GetComponent<Rigidbody>().velocity;

            if (diceVelocity.x != 0 ||
                diceVelocity.y != 0 ||
                diceVelocity.z != 0)
                return false;
            else if (!dice.GetComponent<DiceScript>().IsStandingStraight().Item1)
            {
                dice.GetComponent<Transform>().position = new Vector3(dice.GetComponent<Transform>().position.x, dice.GetComponent<Transform>().position.y + 1, dice.GetComponent<Transform>().position.z);
                dice.GetComponent<Rigidbody>().rotation = UnityEngine.Random.rotation;
                return false;
            }
            else if (dice.GetComponent<Transform>().position.y > 0)
                return false;
        }

        throwEnded = true;

        return true;
    }

    void ClearScore()
    {
        score = 0;
    }
}
