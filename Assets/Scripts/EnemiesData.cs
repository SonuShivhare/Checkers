using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesData : MonoBehaviour
{
    public static EnemiesData instance;
    public int globalValue = 0;

    public Dictionary<string, Vector2Int> enemies;
    public List<Vector2Int> highlightLocList = new List<Vector2Int>();
    int men1Count = 0, men2Count = 0;

    private Vector2Int distanceLoc;

    private void Awake()
    {
        if (instance == null) instance = this;
        enemies = new Dictionary<string, Vector2Int>();
    }

    public void InsertEnemy(Vector2Int location, Vector2Int enemyLoc)
    {
        string name = Board.instance.boardSegments[location.x, location.y].segment.name;
        enemies.Add(name, enemyLoc);
    }

    public List<Vector2Int> ReturnPath(Vector2Int playerLoc, Vector2Int hightLightLoc)
    {
        globalValue = 0;
        if (enemies.Count > 0)
        {
            highlightLocList.Clear();
            while (Board.instance.boardSegments[playerLoc.x, playerLoc.y].location != hightLightLoc)
            {
                highlightLocList.Add(hightLightLoc);
                string name = Board.instance.boardSegments[hightLightLoc.x, hightLightLoc.y].segment.name;
                Vector2Int enemyLoc = enemies[name];
                distanceLoc = hightLightLoc - enemyLoc;
                hightLightLoc = enemyLoc - distanceLoc;
            }
        }
        return highlightLocList;
    }
    
    public int AIReturnPath(Vector2Int playerLoc, Vector2Int hightLightLoc)
    {
        
        return  -1;
    }


    public void FindPath(Vector2Int playerLoc, Vector2Int hightLightLoc)
    {
        if (enemies.Count > 0)
        {
            while (Board.instance.boardSegments[playerLoc.x, playerLoc.y].location != hightLightLoc)
            {
                highlightLocList.Add(hightLightLoc);
                string name = Board.instance.boardSegments[hightLightLoc.x, hightLightLoc.y].segment.name;
                if(!enemies.ContainsKey(name))
                {
                    RemoveEnemy();
                    return;
                }
                Vector2Int enemyLoc = enemies[name];
                distanceLoc = hightLightLoc - enemyLoc;
                hightLightLoc = enemyLoc - distanceLoc;
            }
            RemoveEnemy();
        }
    }

    public void RemoveEnemy()
    {
        for (int i = highlightLocList.Count - 1; i >= 0; i--)
        {
            if (highlightLocList[i].x < 8 && highlightLocList[i].x > -1 && highlightLocList[i].y < 8 && highlightLocList[i].y > -1)
            {
                string name = Board.instance.boardSegments[highlightLocList[i].x, highlightLocList[i].y].segment.name;

                if (enemies.ContainsKey(name))
                {
                    Vector2Int enemyLoc = enemies[name];

                    if (Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state == State.men1) men1Count++;
                    else men2Count++;
                    Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state = State.empty;
                    Destroy(Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].player);
                    AudioManager.instance.PlayCaptureSound();
                }
            }
        }
        ClearEnemyInfo();
        ClearHighlightList();
    }

    public void ClearEnemyInfo()
    {
        enemies.Clear();
    }
    public void ClearHighlightList()
    {

        highlightLocList.Clear();
    }

    public void IsGameover()
    {
        if (men1Count == 12)
        {
            PlayerPrefs.SetString("Winner", "RED WON!");
            SceneManager.LoadScene("Gameover");
        }
        if (men2Count == 12)
        {
            PlayerPrefs.SetString("Winner", "BLUE WON!");
            SceneManager.LoadScene("Gameover");
        }
    }
}
