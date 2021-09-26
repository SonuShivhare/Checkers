using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Range(1, 10)] private float animationSpeed = 8f;
    int i, j;

    State currentTurn = State.men2;

    List<Vector2Int> highlightLocList = new List<Vector2Int>();
    public Vector2Int playerLoc;
    private bool isAnimating = false;

    private void Awake()
    {
            if(instance == null)
        {
            instance = this;
        }
    }

    public void OnClick(Transform Segment)
    {
        if (!isAnimating)
        {
            i = Convert.ToInt32(Segment.name.Split(',')[0]);
            j = Convert.ToInt32(Segment.name.Split(',')[1]);
            HighlightDate.instance.Intialize(Board.instance.boardSegments[i, j].location);

            switch (Board.instance.boardSegments[i, j].state)
            {
                case State.empty:
                    HighlightDate.instance.DestroyHighlight();
                    return;
                case State.men1:
                    if (currentTurn == State.men1)
                    {
                        HighlightDate.instance.DestroyHighlight();
                        EnemiesData.instance.ClearEnemyInfo();
                        playerLoc = Board.instance.boardSegments[i, j].location;

                        HighlightDate.instance.InsertIteration(i, j, 0, -1, 1);
                        HighlightDate.instance.InsertIteration(i, j, 0, 1, 1);

                        if (Board.instance.boardSegments[i, j].isKing == true)
                        {
                            HighlightDate.instance.InsertIteration(i, j, 0, 1, -1);
                            HighlightDate.instance.InsertIteration(i, j, 0, -1, -1);
                        }
                        HighlightDate.instance.GenrateHighlight();
                    }
                    break;
                case State.men2:
                    if (currentTurn == State.men2)
                    {
                        HighlightDate.instance.DestroyHighlight();
                        EnemiesData.instance.ClearEnemyInfo();
                        playerLoc = Board.instance.boardSegments[i, j].location;

                        HighlightDate.instance.InsertIteration(i, j, 0, 1, -1);
                        HighlightDate.instance.InsertIteration(i, j, 0, -1, -1);

                        if (Board.instance.boardSegments[i, j].isKing == true)
                        {
                            HighlightDate.instance.InsertIteration(i, j, 0, -1, 1);
                            HighlightDate.instance.InsertIteration(i, j, 0, 1, 1);
                        }
                        HighlightDate.instance.GenrateHighlight();
                    }
                    break;
                case State.highlight:
                    MovePlayer();
                    break;
            }
        }
    }

    public void MovePlayer()
    {
        AudioManager.instance.PlayMoveSound();
        HighlightDate.instance.DestroyHighlight();
        Board.instance.boardSegments[i, j].state = Board.instance.boardSegments[playerLoc.x, playerLoc.y].state;
        Board.instance.boardSegments[i, j].isKing = Board.instance.boardSegments[playerLoc.x, playerLoc.y].isKing;
        Board.instance.boardSegments[playerLoc.x, playerLoc.y].state = State.empty;
        Board.instance.boardSegments[playerLoc.x, playerLoc.y].isKing = false;
        Board.instance.boardSegments[i, j].player = Board.instance.boardSegments[playerLoc.x, playerLoc.y].player;

        if (j - playerLoc.y == 1 || j - playerLoc.y == -1)
        {
            StartCoroutine(MyCouroutine(Board.instance.boardSegments[playerLoc.x, playerLoc.y].player.transform, Board.instance.boardSegments[i, j].segment.transform.position));
            highlightLocList.Clear();
        }
        else
        {
            highlightLocList = EnemiesData.instance.ReturnPath(playerLoc, new Vector2Int(i, j));
            highlightLocList.Reverse();
            StartCoroutine(MyCouroutine(Board.instance.boardSegments[playerLoc.x, playerLoc.y].player.transform, Board.instance.boardSegments[highlightLocList[0].x, highlightLocList[0].y].segment.transform.position));
            highlightLocList.RemoveAt(0);
        }

        Board.instance.boardSegments[playerLoc.x, playerLoc.y].player.transform.GetComponent<Canvas>().sortingOrder = 3;
        Board.instance.boardSegments[playerLoc.x, playerLoc.y].player = null;
    }

    private void CheckForKing()
    {
        if (Board.instance.boardSegments[i, j].state == State.men1)
        {
            currentTurn = State.men2;
            if (Board.instance.boardSegments[i, j].location.y == 7)
            {
                Board.instance.boardSegments[i, j].isKing = true;
                Board.instance.boardSegments[i, j].player.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            currentTurn = State.men1;
            if (Board.instance.boardSegments[i, j].location.y == 0)
            {
                Board.instance.boardSegments[i, j].isKing = true;
                Board.instance.boardSegments[i, j].player.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void DestroyEnemy()
    {
        EnemiesData.instance.FindPath(playerLoc, new Vector2Int(i, j));
        EnemiesData.instance.IsGameover();
    }

    IEnumerator MyCouroutine(Transform player, Vector2 target)
    {
        while (Vector2.Distance(player.position, target) > 0.01)
        {
            player.position = Vector2.MoveTowards(player.position, target, animationSpeed);
            yield return null;
        }

        if (highlightLocList.Count > 0)
        {
            StartCoroutine(MyCouroutine(player, Board.instance.boardSegments[highlightLocList[0].x, highlightLocList[0].y].segment.transform.position));
            highlightLocList.RemoveAt(0);
        }
        else
        {
            DestroyEnemy();
            CheckForKing();
            player.GetComponent<Canvas>().sortingOrder = 2;
            isAnimating = false;
            HighlightDate.instance.DestroyHighlight();
            if(currentTurn == State.men1) AIController.instance.playMove();
        }
    }
}