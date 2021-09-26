using System.Collections.Generic;
using UnityEngine;

struct HighlightIterationList
{
    public int right;
    public int top;
    public int count;
    public int directionX;
    public int directionY;
}

public class HighlightDate : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject panel01;

    List<HighlightIterationList> iterationLists = new List<HighlightIterationList>();
    public static HighlightDate instance;
    Vector2Int playerLoc;
    private bool isGenrating = true;
    int right;
    int top;
    public int count;
    int directionX;
    int directionY;
    public List<Vector2Int> highlightLocList = new List<Vector2Int>();
    Vector2Int highlightLoc;
    Vector2Int enemyLoc = new Vector2Int(-1, -1);

    GameObject obj;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void GenrateHighlight()
    {
        while (iterationLists.Count > 0)
        {
            right = iterationLists[0].right;
            top = iterationLists[0].top;
            count = iterationLists[0].count;
            directionX = iterationLists[0].directionX;
            directionY = iterationLists[0].directionY;
            ResetEnemies();
            Move();
            RemoveIteration();
        }
    }

    private void Move()
    {
        this.right += directionX;
        this.top += directionY;
        count++;

        if (this.right < 8 && this.right > -1 && this.top < 8 && this.top > -1)
        {
            if (Board.instance.boardSegments[playerLoc.x, playerLoc.y].state == Board.instance.boardSegments[this.right, this.top].state)
            {
                return;
            }
            else if (Board.instance.boardSegments[this.right, this.top].state == State.empty)
            {
                if (count % 2 == 0 || count == 1)
                {
                    highlightLoc = new Vector2Int(this.right, this.top);
                    if (enemyLoc.x >= 0)
                    {
                        EnemiesData.instance.InsertEnemy(highlightLoc, enemyLoc);
                        enemyLoc.x = -1;
                    }
                    highlightLocList.Add(highlightLoc);
                    instantiateHighlight(highlightLoc);
                }

                if (count % 2 == 0)
                {
                    InsertIteration(highlightLoc.x, highlightLoc.y, 2, directionX, directionY);
                    InsertIteration(highlightLoc.x, highlightLoc.y, 2, -directionX, directionY);
                }
            }
            else
            {
                if (count % 2 != 0)
                {
                    enemyLoc = new Vector2Int(this.right, this.top);
                    Move();
                }
                return;
            }
        }
    }
    private void instantiateHighlight(Vector2Int location)
    {
        obj = Instantiate(highlight);
        obj.transform.SetParent(panel01.transform);
        obj.transform.position = Board.instance.boardSegments[location.x, location.y].segment.transform.position;
        Board.instance.boardSegments[location.x, location.y].highlight = obj;
        Board.instance.boardSegments[location.x, location.y].state = State.highlight;
    }

    public void DestroyHighlight()
    {
        for (int i = 0; i < highlightLocList.Count; i++)
        {
            highlightLoc = highlightLocList[i];
            if (Board.instance.boardSegments[highlightLoc.x, highlightLoc.y].state == State.highlight)
            {
                Board.instance.boardSegments[highlightLoc.x, highlightLoc.y].state = State.empty;
                Destroy(Board.instance.boardSegments[highlightLoc.x, highlightLoc.y].highlight);
            }
        }
        highlightLocList.Clear();
    }

    public void InsertIteration(int right, int top, int count, int directionX, int directionY)
    {
        HighlightIterationList iteration;
        iteration.right = right;
        iteration.top = top;
        iteration.count = count;
        iteration.directionX = directionX;
        iteration.directionY = directionY;
        iterationLists.Add(iteration);
    }

    public void Intialize(Vector2Int playerLoc)
    {
        this.playerLoc = playerLoc;
    }

    public void RemoveIteration()
    {
        iterationLists.RemoveAt(0);
    }

    public void ResetEnemies()
    {
        enemyLoc.x = -1;
    }
}
