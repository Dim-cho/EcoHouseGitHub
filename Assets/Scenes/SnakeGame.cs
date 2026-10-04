using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SnakeGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;


    [Header("Settings")]
    public int gridSize = 20;          // 20 x 20 playfield
    public float moveInterval = 0.12f; // seconds per step (lower = faster)
    public string nextSceneName = "MiddleRoom";


    [Header("Leaving the game (Esc)")]
    [Tooltip("The exact name of the scene to go back to when Esc is pressed.")]
    public string returnSceneName = "Computer";
    [Tooltip("The text shown on the black screen.")]
    public string quittingMessage = "Quitting...";
    [Tooltip("Drag your TextMeshPro Font Asset here (the one made with Window > TextMeshPro > Font Asset Creator). " +
             "Leave empty for the default font.")]
    public TMP_FontAsset quittingFont;
    public float quittingFontSize = 72f;
    public Color quittingTextColor = Color.white;
    [Tooltip("Seconds to fade to black.")]
    public float fadeDuration = 0.6f;
    [Tooltip("Seconds to stay on the black screen before the scene changes.")]
    public float holdTime = 0.6f;


    [Header("Starting the game")]
    [Tooltip("Shown on the black screen when the game opens. Font, size and color are the same as the quitting text above.")]
    public string startingMessage = "Starting...";
    [Tooltip("Seconds to stay on the black screen before it fades away.")]
    public float startHoldTime = 0.6f;
    [Tooltip("Seconds to fade from black into the game.")]
    public float startFadeDuration = 0.6f;


    [Header("Reward")]
    [Tooltip("Coins per point, paid once when the run ends.")]
    public int coinsPerPoint = 5;


    // Colors
    public Color headColor = new Color(0.2f, 1f, 0.2f);
    public Color bodyColor = new Color(0.1f, 0.7f, 0.1f);
    public Color foodColor = Color.red;
    public Color wallColor = new Color(0.3f, 0.3f, 0.35f);


    Sprite squareSprite;
    Transform snakeParent;


    List<Vector2Int> snakePositions = new List<Vector2Int>();
    List<GameObject> snakeObjects = new List<GameObject>();


    GameObject foodObject;
    Vector2Int foodPosition;


    Vector2Int direction = Vector2Int.right;
    Vector2Int nextDirection = Vector2Int.right;


    float timer;
    int score;
    int paidCoins;
    bool isGameOver;
    bool isQuitting;
    bool isIntro;


    void Start()
    {
        CreateSquareSprite();
        CreateBorder();
        snakeParent = new GameObject("Snake").transform;
        StartGame();


        isIntro = true;
        StartCoroutine(IntroRoutine());
    }


    // Makes a 1x1 white sprite in code so no art is needed
    void CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        squareSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }


    GameObject CreateBlock(string name, Vector2 pos, Color color, int sortingOrder, Transform parent = null)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.9f, 0.9f, 1f); // small gap between cells
        if (parent != null) obj.transform.SetParent(parent);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = color;
        sr.sortingOrder = sortingOrder;
        return obj;
    }


    // Draws a ring of wall blocks around the play area
    void CreateBorder()
    {
        Transform borderParent = new GameObject("Border").transform;
        int half = gridSize / 2;
        for (int x = -half - 1; x <= half; x++)
        {
            for (int y = -half - 1; y <= half; y++)
            {
                bool isEdge = x == -half - 1 || x == half || y == -half - 1 || y == half;
                if (isEdge)
                {
                    GameObject wall = CreateBlock("Wall", GridToWorld(new Vector2Int(x, y)), wallColor, 0, borderParent);
                    wall.transform.localScale = Vector3.one;
                }
            }
        }
    }


    Vector2 GridToWorld(Vector2Int g)
    {
        return new Vector2(g.x + 0.5f, g.y + 0.5f);
    }


    void StartGame()
    {
        // Clear old snake
        foreach (GameObject obj in snakeObjects) Destroy(obj);
        snakeObjects.Clear();
        snakePositions.Clear();
        if (foodObject != null) Destroy(foodObject);


        score = 0;
        paidCoins = 0;
        isGameOver = false;
        timer = 0f;
        direction = Vector2Int.right;
        nextDirection = Vector2Int.right;


        // Start with 3 segments in the middle
        for (int i = 0; i < 3; i++)
        {
            snakePositions.Add(new Vector2Int(-i, 0));
        }
        RebuildSnakeVisuals();


        SpawnFood();
        UpdateUI();
    }


    void Update()
    {
        if (isQuitting || isIntro) return;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(QuitRoutine());
            return;
        }


        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(nextSceneName);
            if (Input.GetKeyDown(KeyCode.R)) StartGame();
            return;
        }


        HandleInput();


        timer += Time.deltaTime;
        if (timer >= moveInterval)
        {
            timer -= moveInterval;
            Step();
        }
    }


    // Fades the screen to black with the "Quitting..." text, then goes back to the computer
    IEnumerator QuitRoutine()
    {
        if (string.IsNullOrEmpty(returnSceneName) || !Application.CanStreamedLevelBeLoaded(returnSceneName))
        {
            Debug.LogError("SnakeGame: scene '" + returnSceneName + "' can't be loaded. Check the spelling, and make sure it is in " +
                           "File > Build Profiles (Build Settings) > Scene List.", this);
            yield break;
        }


        isQuitting = true;
        Time.timeScale = 1f;


        CanvasGroup overlay = CreateOverlay(quittingMessage, 0f);


        float t = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            yield return null;
        }
        overlay.alpha = 1f;


        if (holdTime > 0f) yield return new WaitForSecondsRealtime(holdTime);


        SceneManager.LoadScene(returnSceneName);
    }


    // Starts on a black screen saying "Starting...", then fades into the game
    IEnumerator IntroRoutine()
    {
        CanvasGroup overlay = CreateOverlay(startingMessage, 1f);


        if (startHoldTime > 0f) yield return new WaitForSecondsRealtime(startHoldTime);


        float t = 0f;
        float duration = Mathf.Max(0.01f, startFadeDuration);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(t / duration));
            yield return null;
        }


        Destroy(overlay.gameObject);
        isIntro = false;
    }


    // Builds a full-screen black panel with centered text, on top of everything.
    // startAlpha 0 = invisible (for fading in), 1 = fully black (for fading out).
    CanvasGroup CreateOverlay(string message, float startAlpha)
    {
        GameObject canvasObject = new GameObject("ScreenOverlay", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;


        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;


        CanvasGroup group = canvasObject.GetComponent<CanvasGroup>();
        group.alpha = startAlpha;
        group.blocksRaycasts = true;


        GameObject black = new GameObject("Black", typeof(RectTransform), typeof(Image));
        black.transform.SetParent(canvasObject.transform, false);
        StretchToParent(black.GetComponent<RectTransform>());
        black.GetComponent<Image>().color = Color.black;


        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);
        StretchToParent(textObject.GetComponent<RectTransform>());


        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        if (quittingFont != null) label.font = quittingFont;
        label.text = message;
        label.fontSize = quittingFontSize;
        label.color = quittingTextColor;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;


        return group;
    }


    static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }


    void HandleInput()
    {
        // Can't reverse directly into yourself
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && direction != Vector2Int.down)
            nextDirection = Vector2Int.up;
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && direction != Vector2Int.up)
            nextDirection = Vector2Int.down;
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && direction != Vector2Int.right)
            nextDirection = Vector2Int.left;
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && direction != Vector2Int.left)
            nextDirection = Vector2Int.right;
    }


    void Step()
    {
        direction = nextDirection;
        Vector2Int newHead = snakePositions[0] + direction;
        int half = gridSize / 2;


        // Wall collision
        if (newHead.x < -half || newHead.x >= half || newHead.y < -half || newHead.y >= half)
        {
            GameOver();
            return;
        }


        bool ateFood = newHead == foodPosition;


        // Self collision (the tail will move away, so ignore it unless we're growing)
        int checkCount = ateFood ? snakePositions.Count : snakePositions.Count - 1;
        for (int i = 0; i < checkCount; i++)
        {
            if (snakePositions[i] == newHead)
            {
                GameOver();
                return;
            }
        }


        // Move: add new head
        snakePositions.Insert(0, newHead);


        if (ateFood)
        {
            score += 1;
            UpdateUI();
            SpawnFood();
        }
        else
        {
            // Remove tail
            snakePositions.RemoveAt(snakePositions.Count - 1);
        }


        RebuildSnakeVisuals();
    }


    void RebuildSnakeVisuals()
    {
        foreach (GameObject obj in snakeObjects) Destroy(obj);
        snakeObjects.Clear();


        for (int i = 0; i < snakePositions.Count; i++)
        {
            Color c = (i == 0) ? headColor : bodyColor;
            GameObject seg = CreateBlock("Segment", GridToWorld(snakePositions[i]), c, 2, snakeParent);
            snakeObjects.Add(seg);
        }
    }


    void SpawnFood()
    {
        int half = gridSize / 2;
        List<Vector2Int> freeCells = new List<Vector2Int>();


        for (int x = -half; x < half; x++)
            for (int y = -half; y < half; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!snakePositions.Contains(cell)) freeCells.Add(cell);
            }


        if (freeCells.Count == 0) { GameOver(); return; } // You won!


        foodPosition = freeCells[Random.Range(0, freeCells.Count)];


        if (foodObject != null) Destroy(foodObject);
        foodObject = CreateBlock("Food", GridToWorld(foodPosition), foodColor, 1);
    }


    void GameOver()
    {
        isGameOver = true;


        // Paid once, here — awarding per point as you eat would let a player
        // farm coins without ever finishing a run.
        paidCoins = MinigameReward.Pay(score * coinsPerPoint);


        UpdateUI();
    }


    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;


        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(isGameOver);


            if (isGameOver)
            {
                gameOverText.text =
                    $"Game over\nScore {score} · {MinigameReward.Describe(paidCoins)}";
            }
        }
    }
}