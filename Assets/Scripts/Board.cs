using UnityEngine;

public enum State
{
    empty,
    men1,
    men2,
    highlight
}

public struct BoardSegments
{
    public GameObject segment;
    public Vector2Int location;
    public State state;
    public GameObject player;
    public bool isKing;
    public GameObject highlight;
}

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject segment1;
    [SerializeField] private GameObject segment2;
    [SerializeField] private GameObject men1;
    [SerializeField] private GameObject men2;
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;

    private Vector2 startingPoint;
    private Vector2 canvasCenter;
    private Vector2 segmentSize;

    private GameObject temp;
    private GameObject first;
    private GameObject second;

    private float top;
    private float right;

    public static Board instance;
    [HideInInspector] public BoardSegments[,] boardSegments;

    private void Awake()
    {
        if (instance == null) instance = this;

        boardSegments = new BoardSegments[8, 8];
        ResetSize();
        canvasCenter = GetComponent<Canvas>().GetComponent<RectTransform>().position;
        segmentSize = segment1.GetComponent<RectTransform>().sizeDelta;
        startingPoint.x = canvasCenter.x - (4 * segmentSize.x) + segmentSize.x / 2;
        startingPoint.y = canvasCenter.y + (4 * segmentSize.y) - segmentSize.y / 2;

        GenrateBoard();
        GenrateMen();
    }

    private void GenrateBoard()
    {
        top = startingPoint.y;
        for (int j = 0; j < 8; j++)
        {
            top++;
            if (j % 2 == 0)
            {
                first = segment1;
                second = segment2;
            }
            else
            {
                first = segment2;
                second = segment1;
            }
            right = startingPoint.x;
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0) temp = Instantiate(first);
                else temp = Instantiate(second);
                temp.transform.position = new Vector2(right, top);
                temp.transform.SetParent(panel1.transform);
                temp.transform.name = i + "," + j;
                BoardSegments piece = new BoardSegments();
                piece.segment = temp;
                piece.location = new Vector2Int(i, j);
                piece.state = State.empty;
                piece.isKing = false;
                boardSegments[i, j] = piece;
                right += segmentSize.x;
            }
            top -= segmentSize.y;
        }
    }

    private void GenrateMen()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                if (boardSegments[i, j].segment.CompareTag("Segment01"))
                {
                    if (boardSegments[i, j].segment.transform.position.y > canvasCenter.y + segmentSize.y)
                    {
                        boardSegments[i, j].player = Instantiate(men1);
                        boardSegments[i, j].player.transform.SetParent(panel2.transform);
                        boardSegments[i, j].player.transform.position = boardSegments[i, j].segment.transform.position;
                        boardSegments[i, j].state = State.men1;
                    }
                    else if (boardSegments[i, j].segment.transform.position.y < canvasCenter.y - segmentSize.y)
                    {
                        boardSegments[i, j].player = Instantiate(men2);
                        boardSegments[i, j].player.transform.SetParent(panel2.transform);
                        boardSegments[i, j].player.transform.position = boardSegments[i, j].segment.transform.position;
                        boardSegments[i, j].state = State.men2;
                    }
                }
            }
        }
    }

    public void ResetSize()
    {
        float width = Screen.width / 9;

        Debug.Log("Width: " + Screen.width + " " + width);

        segment1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);
        segment2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);
        men1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);
        men2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);
    }
}
