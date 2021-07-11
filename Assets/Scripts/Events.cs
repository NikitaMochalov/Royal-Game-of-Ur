 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Events : MonoBehaviour
{
    [Header("Механика ходов")]

    // Какой сейчас ход
    public int move;
    // Какой игрок совершает ход
    private int player;
    // Сколько выпало очков игроку
    private int diceScore;
    // На какую клетку сходил игрок
    int cell;
    // Приземлился на клетку дающую доп ход
    int doubleCell;
    // Срубил ли он фишку оппонента
    int сutPiece;

    // Информация о сделанных ходах в игре
    List<int[]> moves;

    void Start()
    {
        moves = new List<int[]>();
        move = 0;
        player = 1;
        diceScore = -1;
        cell = -1;
        сutPiece = -1;
        doubleCell = -1;

        gameEnded = false;

        popUpIndex = 0;

        if (!GameObject.Find("TutorialController").GetComponent<TutorialController>().tutorialOn)
            TutorialOff();
            
    }


    [Header("Список всплывающих окон")]

    // Список всех всплывающих окон по очереди
    public GameObject[] popUps;

    [Header("Обучение")]

    // Включено ли обучение
    public bool tutorialOn;

    // Индекс текущего всплывающего окна
    public int popUpIndex;

    // Указатели
    GameObject currentPointer;

    public GameObject playerChipsPointer;
    public GameObject enemyChipsPointer;

    public GameObject playerPathPointer;
    public GameObject enemyPathPointer;

    public GameObject doubleCellPointer;
    public GameObject centralCellPointer;

    public GameObject dicesPointer;
    //

    public bool dicesFocus;

    public bool gameFocus;

    // Метод повышающий индекс всплывающих окон
    public void NextPopUp()
    {
        popUpIndex++;
    }

    // Ответ на первый вопрос
    public void Answer1(bool answer)
    {
        if (answer)
        {
            popUps[15] = rightAnswer1;
        }
        else
        {
            popUps[15] = wrongAnswer1;
        }
        NextPopUp();
    }

    // Ответ на второй вопрос
    public void Answer2(bool answer)
    {
        if (answer)
        {
            popUps[16] = rightAnswer2;
        }
        else
        {
            popUps[16] = wrongAnswer2;
        }
        NextPopUp();
    }

    // Выключение туториала
    public void TutorialOff()
    {
        tutorialOn = false;

        for (int i = 0; i < popUps.Length; i++)
        {
            popUps[i].SetActive(false);
        }

        GetComponent<GameControllerScript>().touchBlock = false;
        statusWindow.SetActive(true);
        ChangeCameraTo(new Vector3(1f, 0f, 0f), "standart");
    }

    

    // объяснение 1 слайд
    public void Explanation()
    {
        // Показываем карточку о различии вероятностей двух разных костей
        NextPopUp();

        // Переносим камеру к первому слайду о двух костях
        ChangeCameraTo(new Vector3(-56.2f, -1f, -4f), "tutorial");

        firstSlide.SetActive(true);
    }

    // объяснение 2 слайд
    public void ExplanationSecond()
    {
        // Показываем карточку c с вопросом как посчитать вероятность на четырех костях
        NextPopUp();

        // Переносим камеру к первому слайду о двух костях
        ChangeCameraTo(new Vector3(-49.3f, -1f, -4f), "tutorial");

        firstSlide.SetActive(false);
        secondSlide.SetActive(true);
    }

    [Header("Туториал о вероятностях")]

    public GameObject formulasCube;

    // Показ формул
    public void ShowFormulas()
    {
        // Показываем карточку
        NextPopUp();

        formulasCube.SetActive(true);
    }

    public GameObject randomDices;
    public GameObject fourScoredDices;
    public GameObject zeroScoredDices;
    public GameObject threeScoredDices;

    public GameObject otherThreeProbabilities;
    public GameObject oneScoredProbabilities;

    public GameObject twoScoredDices;

    // Показ костей с четверкой
    public void ShowFourScoredDices()
    {
        // Показываем карточку
        NextPopUp();

        randomDices.SetActive(false);

        fourScoredDices.SetActive(true);
    }

    // Показ костей с четверкой
    public void ShowZeroScoredDices()
    {
        // Показываем карточку
        NextPopUp();

        zeroScoredDices.SetActive(true);
    }

    // Показ костей с четверкой
    public void ShowThreeScoredDices()
    {
        // Показываем карточку
        NextPopUp();

        fourScoredDices.SetActive(false);
        zeroScoredDices.SetActive(false);


        threeScoredDices.SetActive(true);
    }

    public void ShowThreeScoredProbabilities()
    {
        // Показываем карточку
        NextPopUp();

        otherThreeProbabilities.SetActive(true);
    }

    public void ShowOneScoredProbabilities()
    {
        // Показываем карточку
        NextPopUp();

        threeScoredDices.SetActive(false);
        otherThreeProbabilities.SetActive(false);

        oneScoredProbabilities.SetActive(true);
    }

    public void ShowTwoScoredDicess()
    {
        // Показываем карточку
        NextPopUp();

        oneScoredProbabilities.SetActive(false);

        twoScoredDices.SetActive(true);
    }

    public void ThirdSlide()
    {
        // Показываем карточку
        NextPopUp();

        secondSlide.SetActive(false);

        thirdSlide.SetActive(true);

        ChangeCameraTo(new Vector3(-45f, 0f, -4f), "standart");
    }

    public GameObject tutorialDices3;

    public void ThrowThree()
    {
        // Показываем карточку
        NextPopUp();

        tutorialDices3.SetActive(true);
    }

    public GameObject whitePiece2;

    public void PutSecondPiece()
    {
        // Показываем карточку
        NextPopUp();

        whitePiece2.SetActive(true);
    }

    public GameObject tutorialDices2;

    public GameObject blackPiece1;
    public GameObject blackPiece2;

    public void ThrowTwo()
    {
        // Показываем карточку
        NextPopUp();

        tutorialDices3.SetActive(false);

        tutorialDices2.SetActive(true);

        blackPiece1.SetActive(false);

        blackPiece2.SetActive(true);
    }


    [Header("Хз")]

    public GameObject mathPopUpBG;

    bool mathTutorialStarted;

    [Header("Камера")]

    // Ссылка на камеру
    public GameObject mainCameraPivot;
    public GameObject mainCamera;

    public void ChangeCameraTo(Vector3 position, string mode)
    {
        if (mode == "standart")
        {
            mainCamera.GetComponent<CameraOrbit>().StandartMode();
        }
        else if (mode == "tutorial")
        {
            mainCamera.GetComponent<CameraOrbit>().TutorialMode();
        }

        mainCameraPivot.transform.position = position;
    }

    public void StartMathTutorial()
    {
        if (mathTutorialStarted)
            return;

        NextPopUp();

        mathTutorialStarted = true;

        statusWindow.SetActive(false);

        //mathPopUpBG.SetActive(true);
    }

    [Header("Первый ответ")]

    public GameObject rightAnswer1;
    public GameObject wrongAnswer1;

    [Header("Второй ответ")]

    public GameObject rightAnswer2;
    public GameObject wrongAnswer2;

    [Header("Слайды")]

    public GameObject firstSlide;
    public GameObject secondSlide;
    public GameObject thirdSlide;

    void Update()
    {
        if (tutorialOn)
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                if (i == popUpIndex)
                {
                    popUps[i].SetActive(true);
                }
                else
                {
                    popUps[i].SetActive(false);
                }
            }

            if (popUpIndex == 1)
            {
                if (Input.GetMouseButtonUp(1))
                    popUpIndex++;
            }
            else if (popUpIndex == 2)
            {
                if (Input.mouseScrollDelta.y > 0.9 || Input.mouseScrollDelta.y < -0.9)
                    popUpIndex++;
            }
            else if (popUpIndex == 3)
            {
                currentPointer = playerChipsPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 4)
            {
                currentPointer.SetActive(false);
                currentPointer = enemyChipsPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 5)
            {
                currentPointer.SetActive(false);
                currentPointer = playerPathPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 6)
            {
                currentPointer.SetActive(false);
                currentPointer = enemyPathPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 7)
            {
                currentPointer.SetActive(false);
            }
            else if (popUpIndex == 8)
            {
                currentPointer = doubleCellPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 9)
            {
                currentPointer.SetActive(false);
                currentPointer = centralCellPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 10)
            {
                currentPointer.SetActive(false);
                currentPointer = dicesPointer;
                currentPointer.SetActive(true);
            }
            else if (popUpIndex == 11)
            {
                currentPointer.SetActive(false);
            }
            // Конец обучения, включение окна статуса ходов, выключения блокировки нажатий
            else if (popUpIndex == 13)
            {
                statusWindow.SetActive(true);
                GetComponent<GameControllerScript>().touchBlock = false;
            }
            
            // Включение рассказа о вероятностях
            else if (popUpIndex == 14)
            {
                GetComponent<GameControllerScript>().touchBlock = true;

                if (dicesFocus == false)
                ChangeCameraTo(new Vector3(7f, 0f, 0f), "tutorial");

                dicesFocus = true;
            }
            // После ответа на первый вопрос
            else if (popUpIndex == 23)
            {

            }
            // Выключение рассказа о вероятностях
            else if (popUpIndex == popUps.Length)
            {
                if(gameFocus == false)
                {
                    GetComponent<GameControllerScript>().touchBlock = false;
                    statusWindow.SetActive(true);

                    ChangeCameraTo(new Vector3(1f, 0f, 0f), "standart");

                    thirdSlide.SetActive(false);
                }
                
                gameFocus = true;
            }

            // При определенном move будет включаться рассказ о вероятностях
            if (move == 20)
            {
                StartMathTutorial();
            }
        }
        

        /////
        // Окно состояния 
        /////

        if (GetComponent<GameControllerScript>().currentPlayer == 1)
        {
            if (!GetComponent<GameControllerScript>().DiceWasThrownThisTurn())
                diceText.text = "Нажмите на кости, чтобы бросить их";
            else if (GetComponent<GameControllerScript>().dices.GetComponent<DicesScoreScript>().throwEnded)
                diceText.text = "Сделайте ход фишкой";
        }

        if (GetComponent<GameControllerScript>().currentPlayer == 2)
        {
            diceText.text = "";
        }

        if (!gameEnded)
        {
            if(GetComponent<GameControllerScript>().playerOneScore == 7)
            {
                //winDisplay.GetComponent<CanvasGroup>().alpha = 1;
                winDisplay.SetActive(true);
                gameEnded = true;
            }
            if (GetComponent<GameControllerScript>().playerTwoScore == 7)
            {
                //lostDisplay.GetComponent<CanvasGroup>().alpha = 1;
                lostDisplay.SetActive(true);
                gameEnded = true;
            }
        }
    }

    public void EndMove(int player, int diceScore, int cell, int doubleCell, int сutPiece)
    {
        // Записываем всю информации о прошедшем ходе
        moves.Add(new int[6] { move, player, diceScore, cell, doubleCell, сutPiece });

        // Информации о последнем ходе для дебага
        string moveLog = "Last move info: move #" + move + ", done by player " + player + ", to cell with index " + cell;

        if (doubleCell == 0)
            moveLog += ", that is usual cell";
        else
            moveLog += ", that gives another chance to move";

        if (сutPiece == 0)
            moveLog += " and was empty before player chip landed there.";
        else
            moveLog += " and player removed opponent's chip from it.";

        Debug.Log(moveLog);

        if (doubleCell == 0)
            GetComponent<GameControllerScript>().PassTurnToOtherPlayer();

        if (doubleCell == 0)
        {
            if (player == 1)
            {
                turnText.text = "Ходит противник...";
                turnText.color = Color.red;
            }
            else
            {
                turnText.text = "Ваш ход!";
                turnText.color = Color.green;
            }
        }

        // Итерация для следующего хода (может ходить тот же игрок)
        move++;
    }

    public int[] LastMove()
    {
        return moves[moves.Count - 1];
    }

    // UI

    [Header("Окно статуса")]

    public GameObject statusWindow;
    public TMP_Text turnText;
    public Text diceText;

    [Header("Завершение игры")]

    public bool gameEnded;
    public GameObject winDisplay;
    public GameObject lostDisplay;

}
