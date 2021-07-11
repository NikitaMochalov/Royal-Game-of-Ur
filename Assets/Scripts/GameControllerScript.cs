using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    // Какой сейчас ход (необходимо для блокировки костей)
    public int moveWhenDicesWasThrownLast;

    // Клетка на которую можно встать
    public GameObject possibleCell;
    bool isPossibleCellHighlighted;

    // Результат броска костей
    public GameObject dices;
    public int currentScore = 0;

    // Количество доведенных до конца клеток
    public int playerOneScore;
    public int playerTwoScore;

    public void ScoreInCurrentThrow(int thisThrowScore)
    {
        currentScore = thisThrowScore;
    }

    public bool DiceWasThrownThisTurn()
    {
        return moveWhenDicesWasThrownLast == GetComponent<Events>().move;
    }

    public void PlayerGotZero()
    {
        Debug.Log("Player " + currentPlayer + " rolled zero!");
        GetComponent<Events>().EndMove(currentPlayer, currentScore, -1, 0, 0);
        possibleCell = null;
    }

    public void PlayerCantMove()
    {
        Debug.Log("Player " + currentPlayer + " can't move!");
        GetComponent<Events>().EndMove(currentPlayer, currentScore, -1, 0, 0);
        possibleCell = null;
    }

    public void PassTurnToOtherPlayer()
    {
        if (currentPlayer == 1)
        {
            currentPlayer = 2;
            //BotThrow();
        }
        else currentPlayer = 1;
    }

    // Код доски
    /* Игральная доска предстваленная двумерным массивом
     * Значения массива могут быть: 0 - пустая клетка, 1 - клетка с фишкой первого игрока, 2 - клетка с фишкой второго игрока
     * Доска разделена на три части:
     * [1]  [0]   \
     * [1]  [2]    |      Левая часть - клетки для первого игрока, правая - для второго
     * [0]  [0]    |
     * [0]* [2]*  /
     * 
     *  [ ][ ]       \
     *  [ ][ ]       |
     *  [ ][ ]       |
     *  [ ][ ]*      |
     *  [ ][ ]       |
     *  [ ][ ]       |
     *  [ ][ ]       |      
     *  [ ][ ]       /
     * 
     * [ ]  [ ]   \       Предфинишные клетки, разделенные между игроками
     * [ ]* [ ]*  /
     * 
     * [ ]  [ ]   >       Непосредственно финиш; попадая на него фишки уходят с поля
    */

    int[,] board;
    GameObject[,] gmBoard;

    public GameObject[] gmBoardLeft;
    public GameObject[] gmBoardRight;

    void FillGmBoard()
    {
        gmBoard = new GameObject[15, 2];

        for (int i = 0; i < 15; i++)
        {
            // Левая сторона
            gmBoard[i, 0] = gmBoardLeft[i];
            gmBoard[i, 0].GetComponent<CellScript>().index = i;
            // Правая сторона
            gmBoard[i, 1] = gmBoardRight[i];
            gmBoard[i, 1].GetComponent<CellScript>().index = i;
        }
    }

    // Передвижение фишек
    public int currentPlayer;

    GameObject currentPiece;
    GameObject currentCell;

    public Camera mainCamera;
    public LayerMask piecesMask;
    public LayerMask cellMask;
    public LayerMask diceMask;

    void Start()
    {
        // По дефолту ход начинается с нулевого, поэтому тут -1
        moveWhenDicesWasThrownLast = -1;

        //throwEnded = false;
        currentPlayer = 1;
        board = new int[15, 2];
        FillGmBoard();

        touchBlock = true;
    }

    public bool throwEnded;

    public bool touchBlock;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Input.GetMouseButtonDown(0) && !touchBlock)
        {
            if (Physics.Raycast(ray, out hitInfo, 100, diceMask))
            {
                if (currentPlayer == 1)
                DicePressedEvents(hitInfo);
            }
            else if (Physics.Raycast(ray, out hitInfo, 100, piecesMask))
            {
                if (dices.GetComponent<DicesScoreScript>().score != -1 
                    && dices.GetComponent<DicesScoreScript>().score != 0
                    && DiceWasThrownThisTurn())
                    PiecePressedEvents(hitInfo);
            }
            else if (Physics.Raycast(ray, out hitInfo, 100, cellMask))
            {
                if (hitInfo.collider.gameObject.Equals(possibleCell))
                    CellPressedEvents(hitInfo);
                else DehighlightEverything();
            }
            else
            {
                DehighlightEverything();
            }
        }

        if (GetComponent<GameControllerScript>().currentPlayer == 2)
        {
            if (!DiceWasThrownThisTurn())
            {
                GetComponent<GameControllerScript>().BotThrow();
                
            }
            else 
            {
                
                if (dices.GetComponent<DicesScoreScript>().throwEnded)
                {
                    BotMove();
                }
            }
        }


        /*
        if (possibleCell != null)
        {
            if (Physics.Raycast(ray, out hitInfo, 100, cellMask))
            {
                if (hitInfo.collider.gameObject.Equals(possibleCell) && !isPossibleCellHighlighted)
                {
                    possibleCell.GetComponent<CellScript>().ChangeMaterial();
                    isPossibleCellHighlighted = true;
                }
            }
            else if(isPossibleCellHighlighted)
            {
                possibleCell.GetComponent<CellScript>().ChangeMaterial();
                isPossibleCellHighlighted = false;
            }
        }*/
    }

    void CellPressedEvents(RaycastHit hitInfo)
    {
        //Debug.Log("Клетка нажата");

        int previousCellIndex = currentPiece.GetComponent<PieceScript>().cellIndex;

        // Если возможная клетка финишная
        if (possibleCell.GetComponent<CellScript>().index == 14)
        {
            // Находим клетку на которой фишка стояла и удаляем ссылку на фишку с неё
            gmBoard[previousCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip = null;
            // В текущей фишке ссылаемся на финишную клетку
            currentPiece.GetComponent<PieceScript>().cellIndex = possibleCell.GetComponent<CellScript>().index;

            if (currentPlayer == 1)
                playerOneScore++;
            else if (currentPlayer == 2)
                playerTwoScore++;

            // Перемещаем фишку на финише
            PutPieceOnFinish();
        }
        else
        {
            // Если возможная клетка пустует
            if (possibleCell.GetComponent<CellScript>().chip == null)
            {
                // Если фишка стояла на какой-либо клетке, удаляем ссылку на фишку с прошлой клетки
                if (previousCellIndex != -1)
                    // Находим клетку на которой она стояла и удаляем ссылку на фишку с неё
                    gmBoard[previousCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip = null;
            }
            // Если на клетке стоит вражеская клетка
            else
            {
                // Сначала удаляем вражескую фишку с клетки 
                // Удаляем ссылку на клетку с вражеской фишки
                possibleCell.GetComponent<CellScript>().chip.GetComponent<PieceScript>().cellIndex = -1;
                // Перемещаем вражескую фишку на базу
                possibleCell.GetComponent<CellScript>().chip.GetComponent<PieceScript>().GoToStartPosition();

                // Удаляем ссылку на текущую фишку с прошлой клетки
                gmBoard[previousCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip = null;
            }

            // В текущей фишке ссылаемся на новую клетку
            currentPiece.GetComponent<PieceScript>().cellIndex = possibleCell.GetComponent<CellScript>().index;
            // В новой клетке ссылаемся на текущую фишку
            possibleCell.GetComponent<CellScript>().chip = currentPiece;

            // Перемещаем фишку на новую клетку
            PutPieceOnCell();
        }
        


        // Запись в события 
        int cutPiece = 0;
        int doubleCell = 0;
        // если игрок срубил фишку - сutPiece = true;
        // ...
        // если игрок встал на фишку, дающую доп ход
        if (possibleCell.GetComponent<CellScript>().index == 3 || possibleCell.GetComponent<CellScript>().index == 7 || possibleCell.GetComponent<CellScript>().index == 13)
            doubleCell = 1;

        GetComponent<Events>().EndMove(currentPlayer, currentScore, possibleCell.GetComponent<CellScript>().index, doubleCell, cutPiece);

        DehighlightEverything();
    }

    void DicePressedEvents(RaycastHit hitInfo)
    {
        // Еслин не был сделан бросок
        if (moveWhenDicesWasThrownLast != GetComponent<Events>().move)
        {
            // то можно бросить кости
            hitInfo.collider.gameObject.GetComponent<DiceScript>().Throw();
            // но в этом ходу сделать это больше нельзя
            moveWhenDicesWasThrownLast = GetComponent<Events>().move;
        }
        
    }

    void PiecePressedEvents(RaycastHit hitInfo)
    {
        DehighlightPossibleCell();
        if (hitInfo.collider.gameObject.GetComponent<PieceScript>().playerID == currentPlayer)
            PickPressedPiece(hitInfo);
        else
            DehighlightEverything();
    }

    /*
    void CellPressedEvents(RaycastHit hitInfo)
    {
        if (currentPiece != null && dices.GetComponent<DicesScoreScript>().score != -1)
        {
            var tempPiece = currentPiece;
            HighlightCell(hitInfo);


            // Запись в события 
            int cutPiece = 0;
            int doubleCell = 0;
            // если игрок срубил фишку - сutPiece = true;
            // ...
            // если игрок встал на фишку, дающую доп ход
            if (currentCell.GetComponent<CellScript>().index == 3 || currentCell.GetComponent<CellScript>().index == 7 || currentCell.GetComponent<CellScript>().index == 13)
                doubleCell = 1;

            GetComponent<Events>().EndMove(currentPlayer, currentScore, currentCell.GetComponent<CellScript>().index, doubleCell, cutPiece);


            currentPiece = tempPiece;
            PutPieceOnCell();
            DehighlightEverything();
        }
        //HighlightCell(hitInfo);
    }
    */

    void PutPieceOnCell()
    {
        //само перемещение фишки
        currentPiece.transform.position = new Vector3(possibleCell.transform.position.x, possibleCell.transform.position.y + 0.2f, possibleCell.transform.position.z);
        // заведение информации в массив доски

    }

    void PutPieceOnFinish()
    {
        if (currentPlayer == 1)
        {
            //само перемещение фишки
            currentPiece.transform.position = new Vector3(possibleCell.transform.position.x - 1f, possibleCell.transform.position.y + 0.8f, possibleCell.transform.position.z);
            // заведение информации в массив доски
        }
        else
        {
            currentPiece.transform.position = new Vector3(possibleCell.transform.position.x - 1f, possibleCell.transform.position.y + 0.8f, possibleCell.transform.position.z);
        }

    }

    void PickPressedPiece(RaycastHit hitInfo)
    {
        //Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

        if (currentPiece == null)
        {
            currentPiece = hitInfo.collider.gameObject;

            // Проверяем, возможны ли ходы этой фишкой
            CheckPossibleMove();

            // Если невозможны, сбрасываем текущую фишку
            if (possibleCell == null)
                currentPiece = null;
            // Если возможны, перекрашиваем её
            else currentPiece.GetComponent<PieceScript>().ChangeMaterial();
        }
        else
        {
            if (hitInfo.collider.gameObject.Equals(currentPiece))
            {
                //DehighlightCurrentPiece();
                DehighlightEverything();
            }
            else
            {
                DehighlightCurrentPiece();
                currentPiece = hitInfo.collider.gameObject;

                // Проверяем, возможны ли ходы этой фишкой
                CheckPossibleMove();

                // Если невозможны, сбрасываем текущую фишку
                if (possibleCell == null)
                    currentPiece = null;
                // Если возможны, перекрашиваем её
                else currentPiece.GetComponent<PieceScript>().ChangeMaterial();
            }
        }

        //DehighlightPossibleCell();
    }

    void CheckPossibleMove()
    {
        // Рассчитываем возможную клетку
        // Если бросок сделан
        if (dices.GetComponent<DicesScoreScript>().score != -1)
        {
            int previousCellIndex = currentPiece.GetComponent<PieceScript>().cellIndex;
            int possibleCellIndex = previousCellIndex + currentScore;

            if (possibleCellIndex > 14) return;

            //Debug.Log(previousCellIndex + " " + possibleCellIndex);

            /////
            // В начале (индекс -1) и На нейтральной территории (индексы 0-3, 12-14)
            /////

            if (possibleCellIndex < 4 || possibleCellIndex > 11)
                // Если на возможной клетке не стоит ваша фишка
                if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                    // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                    possibleCell = gmBoard[possibleCellIndex, currentPlayer - 1];

            /////
            // На боевой территории (индексы 4-11)
            /////

            if (possibleCellIndex > 3 && possibleCellIndex < 12)
            {
                // Если на возможной клетке нет фишки
                if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                    // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                    possibleCell = gmBoard[possibleCellIndex, currentPlayer - 1];
                // Если на этой клетке что-то есть
                else
                    // Если на клетке чужая фишка
                    if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip.GetComponent<PieceScript>().playerID != currentPlayer)
                        // Если чужая фишка НЕ стоит на удваивающей клетке
                        if (possibleCellIndex != 7)
                            // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                            possibleCell = gmBoard[possibleCellIndex, currentPlayer - 1];
            }
        }
    }

    // Далее следует кусок кода, проверяющий есть ли возможные ходы у игрока
    public GameObject[] playerOneChips;
    public GameObject[] playerTwoChips;

    public bool MovingIsPossible(int diceScore)
    {
        if (currentPlayer == 1)
        {
            foreach (GameObject chip in playerOneChips)
            {
                int previousCellIndex = chip.GetComponent<PieceScript>().cellIndex;
                int possibleCellIndex = previousCellIndex + diceScore;

                if (possibleCellIndex > 14) continue;

                /////
                // В начале (индекс -1) и На нейтральной территории (индексы 0-3, 12-14)
                /////

                if (possibleCellIndex < 4 || possibleCellIndex > 11)
                    // Если на возможной клетке не стоит ваша фишка
                    if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                        // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                        return true;

                /////
                // На боевой территории (индексы 4-11)
                /////

                if (possibleCellIndex > 3 && possibleCellIndex < 12)
                {
                    // Если на возможной клетке нет фишки
                    if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                        // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                        return true;
                    // Если на этой клетке что-то есть
                    else
                        // Если на клетке чужая фишка
                        if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip.GetComponent<PieceScript>().playerID != currentPlayer)
                        // Если чужая фишка НЕ стоит на удваивающей клетке
                        if (possibleCellIndex != 7)
                            // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                            return true;
                }
            }
        }
        else if (currentPlayer == 2)
        {
            foreach (GameObject chip in playerTwoChips)
            {
                int previousCellIndex = chip.GetComponent<PieceScript>().cellIndex;
                int possibleCellIndex = previousCellIndex + diceScore;

                if (possibleCellIndex > 14) continue;

                /////
                // В начале (индекс -1) и На нейтральной территории (индексы 0-3, 12-14)
                /////

                if (possibleCellIndex < 4 || possibleCellIndex > 11)
                    // Если на возможной клетке не стоит ваша фишка
                    if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                        // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                        return true;

                /////
                // На боевой территории (индексы 4-11)
                /////

                if (possibleCellIndex > 3 && possibleCellIndex < 12)
                {
                    // Если на возможной клетке нет фишки
                    if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip == null)
                        // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                        return true;
                    // Если на этой клетке что-то есть
                    else
                        // Если на клетке чужая фишка
                        if (gmBoard[possibleCellIndex, currentPlayer - 1].GetComponent<CellScript>().chip.GetComponent<PieceScript>().playerID != currentPlayer)
                        // Если чужая фишка НЕ стоит на удваивающей клетке
                        if (possibleCellIndex != 7)
                            // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                            return true;
                }
            }
        }

        return false;
    }

    /*
    void HighlightCell(RaycastHit hitInfo)
    {
        //Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        Debug.Log("Cell was pressed!");

        if (currentCell == null)
        {
            currentCell = hitInfo.collider.gameObject;
            currentCell.GetComponent<CellScript>().ChangeMaterial();
        }
        else
        {
            if (hitInfo.collider.gameObject.Equals(currentCell))
            {
                DehighlightCurrentCell();
            }
            else
            {
                DehighlightCurrentCell();
                currentCell = hitInfo.collider.gameObject;
                currentCell.GetComponent<CellScript>().ChangeMaterial();
            }
        }

        //DehighlightCurrentPiece();
    }
    */

    public void DehighlightEverything()
    {
        DehighlightCurrentPiece();
        DehighlightPossibleCell();
    }

    void DehighlightCurrentPiece()
    {
        if(currentPiece != null)
        {
            currentPiece.GetComponent<PieceScript>().ChangeMaterial();
            currentPiece = null;
        }
    }

    void DehighlightPossibleCell()
    {
        if (possibleCell != null)
        {
            //possibleCell.GetComponent<CellScript>().ChangeMaterial();
            possibleCell = null;
        }
    }

    int OppositePlayer()
    {
        if (currentPlayer == 1) return 2;
        else return 1;
    }

    /////////
    // Bot //
    /////////


    public void BotThrow()
    {
        dices.GetComponent<DicesScoreScript>().Throw();
    }

    public void BotMove()
    {
        //if (!DiceWasThrownThisTurn())
            //return;

        // 1. Поиск лучшей фишки для движения
        List<GameObject> possibleChips = new List<GameObject>();

        // Проходим по массиву фишку
        foreach (GameObject chip in playerTwoChips)
        {
            // Находим фишки которые могут сходить

            int previousCellIndex = chip.GetComponent<PieceScript>().cellIndex;
            int possibleCellIndex = previousCellIndex + currentScore;

            if (possibleCellIndex > 14)
                continue;

            /////
            // В начале (индекс -1) и На нейтральной территории (индексы 0-3, 12-14)
            /////

            if (possibleCellIndex < 4 || possibleCellIndex > 11)
                // Если на возможной клетке не стоит ваша фишка
                if (gmBoard[possibleCellIndex, 1].GetComponent<CellScript>().chip == null)
                    // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                    possibleChips.Add(chip);

            /////
            // На боевой территории (индексы 4-11)
            /////

            if (possibleCellIndex > 3 && possibleCellIndex < 12)
            {
                // Если на возможной клетке нет фишки
                if (gmBoard[possibleCellIndex, 1].GetComponent<CellScript>().chip == null)
                    // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                    possibleChips.Add(chip);
                // Если на этой клетке что-то есть
                else
                    // Если на клетке чужая фишка
                    if (gmBoard[possibleCellIndex, 1].GetComponent<CellScript>().chip.GetComponent<PieceScript>().playerID != 2)
                    // Если чужая фишка НЕ стоит на удваивающей клетке
                    if (possibleCellIndex != 7)
                        // Записываем ссылку возможной клетки (после, при наведении мышки на неё, клетка должна подсвечиваться)
                        possibleChips.Add(chip);
            }
        }

        Debug.Log(possibleChips.Count);

        // Выбираем из фишек лучшую

        currentPiece = possibleChips[Random.Range(0, possibleChips.Count - 1)];

        // Процесс постановки фишки на клетку

        possibleCell = gmBoard[currentPiece.GetComponent<PieceScript>().cellIndex + currentScore, 1];

        Debug.Log(possibleCell.GetComponent<CellScript>().index);

        // Завершение хода

        if (possibleCell.GetComponent<CellScript>().index == 14)
        {
            // Находим клетку на которой фишка стояла и удаляем ссылку на фишку с неё
            gmBoard[currentPiece.GetComponent<PieceScript>().cellIndex, 1].GetComponent<CellScript>().chip = null;
            // В текущей фишке ссылаемся на финишную клетку
            currentPiece.GetComponent<PieceScript>().cellIndex = possibleCell.GetComponent<CellScript>().index;

            playerTwoScore++;

            // Перемещаем фишку на финише
            PutPieceOnFinish();
        }
        else
        {
            // Если возможная клетка пустует
            if (possibleCell.GetComponent<CellScript>().chip == null)
            {
                // Если фишка стояла на какой-либо клетке, удаляем ссылку на фишку с прошлой клетки
                if (currentPiece.GetComponent<PieceScript>().cellIndex != -1)
                    // Находим клетку на которой она стояла и удаляем ссылку на фишку с неё
                    gmBoard[currentPiece.GetComponent<PieceScript>().cellIndex, 1].GetComponent<CellScript>().chip = null;
            }
            // Если на клетке стоит вражеская клетка
            else
            {
                // Сначала удаляем вражескую фишку с клетки 
                // Удаляем ссылку на клетку с вражеской фишки
                possibleCell.GetComponent<CellScript>().chip.GetComponent<PieceScript>().cellIndex = -1;
                // Перемещаем вражескую фишку на базу
                possibleCell.GetComponent<CellScript>().chip.GetComponent<PieceScript>().GoToStartPosition();

                // Удаляем ссылку на текущую фишку с прошлой клетки
                gmBoard[currentPiece.GetComponent<PieceScript>().cellIndex, 1].GetComponent<CellScript>().chip = null;
            }

            // В текущей фишке ссылаемся на новую клетку
            currentPiece.GetComponent<PieceScript>().cellIndex = possibleCell.GetComponent<CellScript>().index;
            // В новой клетке ссылаемся на текущую фишку
            possibleCell.GetComponent<CellScript>().chip = currentPiece;

            // Перемещаем фишку на новую клетку
            PutPieceOnCell();
        }



        // Запись в события 
        int cutPiece = 0;
        int doubleCell = 0;
        // если игрок срубил фишку - сutPiece = true;
        // ...
        // если игрок встал на фишку, дающую доп ход
        if (possibleCell.GetComponent<CellScript>().index == 3 || possibleCell.GetComponent<CellScript>().index == 7 || possibleCell.GetComponent<CellScript>().index == 13)
        {
            doubleCell = 1;
        }
            

        GetComponent<Events>().EndMove(currentPlayer, currentScore, possibleCell.GetComponent<CellScript>().index, doubleCell, cutPiece);

        currentPiece = null;
        possibleCell = null;
    }
}
