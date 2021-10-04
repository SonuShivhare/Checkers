using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemiesData : MonoBehaviour
{
    public Text men1ScoreText;
    public Text men2ScoreText;
    public Text men1CapturedText;
    public Text men2CapturedText;
    public static EnemiesData instance;
    public int globalValue = 0;

    public Dictionary<string, Vector2Int> enemies;
    public List<Vector2Int> highlightLocList = new List<Vector2Int>();
    int men1Score = 0;
    int men2Score = 0;

    private Vector2Int distanceLoc;

    private void Awake()
    {
        if (instance == null) instance = this;
        enemies = new Dictionary<string, Vector2Int>();

        men1CapturedText.text = "Captured: " + 0;
        men2CapturedText.text = "Captured: " + 0;
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
        return -1;
    }


    public void FindPath(Vector2Int playerLoc, Vector2Int hightLightLoc)
    {
        if (enemies.Count > 0)
        {
            while (Board.instance.boardSegments[playerLoc.x, playerLoc.y].location != hightLightLoc)
            {
                highlightLocList.Add(hightLightLoc);
                string name = Board.instance.boardSegments[hightLightLoc.x, hightLightLoc.y].segment.name;
                if (!enemies.ContainsKey(name))
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
        int score = 0;
        Vector2Int location = new Vector2Int();
        State state = new State();
        for (int i = highlightLocList.Count - 1; i >= 0; i--)
        {
            if (highlightLocList[i].x < 8 && highlightLocList[i].x > -1 && highlightLocList[i].y < 8 && highlightLocList[i].y > -1)
            {
                string name = Board.instance.boardSegments[highlightLocList[i].x, highlightLocList[i].y].segment.name;

                if (enemies.ContainsKey(name))
                {
                    Vector2Int enemyLoc = enemies[name];

                    if (Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state == State.men1)
                    {
                        if(Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].isKing)
                        {
                            score += 10;
                            location = new Vector2Int(highlightLocList[i].x, highlightLocList[i].y);
                        }
                        else
                        {
                            score += 2;
                            location = new Vector2Int(highlightLocList[i].x, highlightLocList[i].y);
                        }
                        state = State.men2;
                    }
                    else
                    {
                        if (Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].isKing)
                        {
                            score += 10;
                            location = new Vector2Int(highlightLocList[i].x, highlightLocList[i].y);
                        }
                        else
                        {
                            score += 2;
                            location = new Vector2Int(highlightLocList[i].x, highlightLocList[i].y);
                        }
                        state = State.men1;
                    }

                    Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].state = State.empty;
                    Destroy(Board.instance.boardSegments[enemyLoc.x, enemyLoc.y].player);
                    AudioManager.instance.PlayCaptureSound();
                }
            }
        }
        if(state == State.men1)
        {
            StartCoroutine(GameManager.instance.PlayScoreAnimation(score, new Vector2Int(location.x, location.y), State.men1));
            SetMen1Score(score);
        }
        else
        {
            StartCoroutine(GameManager.instance.PlayScoreAnimation(score, new Vector2Int(location.x, location.y), State.men2));
            SetMen2Score(score);
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
        List<Vector2Int> men1SegementLocations = new List<Vector2Int>();
        List<Vector2Int> men2SegementLocations = new List<Vector2Int>();

        bool men1CanTakeTurn = false;
        bool men2CanTakeTurn = false;

        foreach (var segment in Board.instance.boardSegments)
        {
            if (segment.state == State.men1)
            {
                Vector2Int location = segment.location;
                men1SegementLocations.Add(location);
            }

            if (segment.state == State.men2)
            {
                Vector2Int location = segment.location;
                men2SegementLocations.Add(location);
            }
        }

        men1CapturedText.text = "Captured: " + (12 - men2SegementLocations.Count);
        men2CapturedText.text = "Captured: " + (12 - men1SegementLocations.Count);

        if (men1SegementLocations.Count == 0)
        {
            PlayerPrefs.SetString("Winner", "RED WON!");
            SceneManager.LoadScene("Gameover");
            return;
        }
        if (men2SegementLocations.Count == 0)
        {
            PlayerPrefs.SetString("Winner", "BLUE WON!");
            SceneManager.LoadScene("Gameover");
            return;
        }


        Debug.Log("Men1Count: " + men1SegementLocations.Count);
        foreach (var location in men1SegementLocations)
        {
            HighlightDate.instance.DestroyHighlight();
            EnemiesData.instance.enemies.Clear();
            GameManager.instance.MoveAI(Board.instance.boardSegments[location.x, location.y].segment.transform);
            List<Vector2Int> highLightLocList = HighlightDate.instance.highlightLocList;
            Debug.Log("Men1HighCount: " + highLightLocList.Count);
            if (highLightLocList.Count > 0)
            {
                men1CanTakeTurn = true;
                break;
            }
        }

        Debug.Log("Men2Count: " + men2SegementLocations.Count);
        foreach (var location in men2SegementLocations)
        {
            HighlightDate.instance.DestroyHighlight();
            EnemiesData.instance.enemies.Clear();
            GameManager.instance.OnClick(Board.instance.boardSegments[location.x, location.y].segment.transform);
            List<Vector2Int> highLightLocList = HighlightDate.instance.highlightLocList;
            Debug.Log("Men2HighCount: " + highLightLocList.Count);
            foreach (var item in highLightLocList)
            {
                Debug.Log(item);
            }
            if (highLightLocList.Count > 0)
            {
                men2CanTakeTurn = true;
                break;
            }
        }

        Debug.Log(men1CanTakeTurn + " - " + men2CanTakeTurn);

        if(!men1CanTakeTurn || !men2CanTakeTurn)
        {
            PlayerPrefs.SetString("Winner", "DRAW!");
            SceneManager.LoadScene("Gameover");
        }
    }

    public void SetMen1Score(int num)
    {
        men1Score += num;
        men1ScoreText.text = "Score: " + men1Score;
    }

    public void SetMen2Score(int num)
    {
        men2Score += num;
        men2ScoreText.text = "Score: " + men2Score;
    }
}
