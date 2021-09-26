using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CurrentSelection
{
	public Vector2Int AIPiecelocation;
	public Vector2Int highLightLoc;
	public int value;
}

public struct CreditScore
{
	public int safeArea;
	public int man2;
	public int king;
	public int toBeKing;
}

public class AIController : MonoBehaviour
{
	public static AIController instance;

	public List<Vector2Int> MoveableMenLocationList = new List<Vector2Int>();
	List<Vector2Int> highLightLocList = new List<Vector2Int>();
	List<Vector2Int> highLightLocListEnemy = new List<Vector2Int>();
	CurrentSelection bestSelection = new CurrentSelection();
	CurrentSelection currentSelection = new CurrentSelection();
	CreditScore creditScore = new CreditScore();
	int globalValue;

	Vector2Int distanceLoc;


	bool isGameover = false; // Remove me

    private void Awake()
    {
		if(instance == null)
        {
			instance = this;
        }

		currentSelection.value = -100;
		bestSelection.value = -100;
		MoveableMenLocationList.Clear();

		globalValue = 0;

		creditScore.safeArea = 1;
		creditScore.man2 = 2;
		creditScore.toBeKing = 5;
		creditScore.king = 10;
	}

	public void playMove()
    {
		ResetData();
		GetMen1Piece();
		FindManWithBestMove();
		GameManager.instance.OnClick(Board.instance.boardSegments[bestSelection.AIPiecelocation.x, bestSelection.AIPiecelocation.y].segment.transform);
		GameManager.instance.OnClick(Board.instance.boardSegments[bestSelection.highLightLoc.x, bestSelection.highLightLoc.y].segment.transform);
	}

	private void GetMen1Piece()
    {
        foreach (var segment in Board.instance.boardSegments)
        {
			if (segment.state == State.men1)
			{
				Vector2Int location = segment.location;
				MoveableMenLocationList.Add(location);
			}
        }
    }

    private void FindManWithBestMove()
    {
        foreach (var manLocation in MoveableMenLocationList)
        {
			HighlightDate.instance.DestroyHighlight();
			EnemiesData.instance.enemies.Clear();
			GameManager.instance.OnClick(Board.instance.boardSegments[manLocation.x, manLocation.y].segment.transform);

			highLightLocList = HighlightDate.instance.highlightLocList;

            foreach (var highLightLoc in highLightLocList)
            {
				globalValue = 0;
				Evaluate(highLightLoc, manLocation);
				if(globalValue > currentSelection.value)
                {
					currentSelection.AIPiecelocation = manLocation;
					currentSelection.highLightLoc = highLightLoc;
					currentSelection.value = globalValue;
                }
			}


			if (currentSelection.value > bestSelection.value)
			{
				bestSelection.AIPiecelocation = currentSelection.AIPiecelocation;
				bestSelection.highLightLoc = currentSelection.highLightLoc;
				bestSelection.value = currentSelection.value;
			}

			//int value = minimax(manLocation, 0, true);
			//if (value > currentSelection.value)
			//{
			//	currentSelection.location = manLocation;
			//	currentSelection.value = value;
			//}
		}
		Debug.Log( "piece loaction" + Board.instance.boardSegments[bestSelection.AIPiecelocation.x, bestSelection.AIPiecelocation.y].location + 
			"highLightLocation =" + Board.instance.boardSegments[bestSelection.highLightLoc.x, bestSelection.highLightLoc.y].location + " Value " + bestSelection.value);
	}

	private void Evaluate(Vector2Int highLightLoc, Vector2Int AIPieceLoc)
    {
		if (highLightLoc.y - AIPieceLoc.y == 1 || highLightLoc.y - AIPieceLoc.y == -1 && Board.instance.boardSegments[highLightLoc.x, highLightLoc.y].location.y != 7)
		{
			if (Board.instance.boardSegments[highLightLoc.x, highLightLoc.y].location.y == 7)
			{
				if (!Board.instance.boardSegments[AIPieceLoc.x, AIPieceLoc.y].isKing) globalValue += 10;
			}
			globalValue += 1;
		}
		else
        {
			if (EnemiesData.instance.enemies.Count > 0)
			{
				while (Board.instance.boardSegments[AIPieceLoc.x, AIPieceLoc.y].location != highLightLoc)
				{
					string name = Board.instance.boardSegments[highLightLoc.x, highLightLoc.y].segment.name;
					Vector2Int enemyLoc = EnemiesData.instance.enemies[name];
					if(Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state == State.men2 && Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].isKing == false)
                    {
						globalValue += 2;
                    }
					else if(Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state == State.men2 && Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].isKing == true)
                    {
						globalValue += 5;
					}

					if (Board.instance.boardSegments[highLightLoc.x, highLightLoc.y].location.y == 7)
					{
						if (!Board.instance.boardSegments[AIPieceLoc.x, AIPieceLoc.y].isKing) globalValue += 10;
					}
					distanceLoc = highLightLoc - enemyLoc;
					highLightLoc = enemyLoc - distanceLoc;
				}
			}
		}
	}

	private void ResetData()
	{
		currentSelection.value = -100;
		bestSelection.value = -100;
		MoveableMenLocationList.Clear();
	}




	//if (depth == 0 || isGameover)
	//{
	//    return -1;
	//}

	//if (isMaximizingPlayer)
	//{
	//    int bestValue = -Convert.ToInt32(Mathf.Infinity);

	//}
	//else
	//{
	//    int bestValue = +Convert.ToInt32(Mathf.Infinity);
	//}

	//function minimax(position, depth, maximizingPlayer)
	//if depth == 0 or game over in position
	//	return static evaluation of position

	//if maximizingPlayer
	//	maxEval = -infinity
	//	for each child of position
	//		eval = minimax(child, depth - 1, false)
	//		maxEval = max(maxEval, eval)
	//	return maxEval

	//else
	//	minEval = +infinity
	//	for each child of position
	//		eval = minimax(child, depth - 1, true)
	//		minEval = min(minEval, eval)
	//	return minEval
}
