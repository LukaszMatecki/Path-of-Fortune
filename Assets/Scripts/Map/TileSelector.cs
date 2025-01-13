using System.Collections;
using System.Collections.Generic;
using GG;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public GameObject character;
    public Animator characterAnimator;
    public float clickResetTime = 3f;

    public Dice dice;

    public TMP_Text feedbackPanelText;
    public bool isActive = true;
    private bool isProcessingMoves;
    private float lastClickTime;

    private int leftClickCount;
    public int maxSteps;

    [SerializeField] private MapStateSaveSystem saveSystem;

    private readonly List<Tile> selectedTiles = new();
    public Tile startingTile;
    public Tilemap tilemapGround;
    public Tilemap tilemapOverGround;

    public Vector3 TileUnderCharacter;

    private void Start()
    {
        PlayerInfo.Instance.hasGameStarted = true;
    }

    private void Awake()
    {
        if (PlayerInfo.Instance.hasGameStarted) saveSystem.LoadMapState();
        if (PlayerInfo.Instance.battleJustLost)
        {
            PlayerInfo.Instance.battleJustLost = false;
            character.transform.position = new Vector3(-1.5f, 0.2f, -15.5f);
            SetStartingTileFromPlayerPosition();
        }
        else
        {
            RestorePlayerPosition();
            CheckEnemyUnderPlayer();
        }


    }

    private void OnEnable()
    {
        GameLoader.OnMapStateLoaded += OnMapStateLoaded;
    }

    private void OnDisable()
    {
        GameLoader.OnMapStateLoaded -= OnMapStateLoaded;
    }

    private void OnMapStateLoaded()
    {
        Debug.Log("Map state loaded. Ustawiam starting tile.");
        SetStartingTileFromPlayerPosition();
    }

    private void CheckEnemyUnderPlayer()
    {
        var playerPosition = character.transform.position;
        LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");
        var rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, overGroundLayer))
        {
            var enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                
                if (!PlayerInfo.Instance.battleJustLost)
                {
                    saveSystem.AddEntity(enemy.transform.position, "enemy");
                    Debug.Log($"Enemy {enemy.name} found under the player.");
                    StartCoroutine(HandleEnemyDead(enemy));
                }
                
            }
        }
    }

    private IEnumerator HandleEnemyDead(Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("No enemy provided to HandleEnemyDead.");
            yield break;
        }

        enemy.EnemyDead();

        SetStartingTileFromPlayerPosition();
        yield return new WaitForSeconds(2f);
        DeleteEnemy(enemy);
    }

    private void Update()
    {
        if (!isActive) return;

        if (selectedTiles.Count > 0)
        {
            //characterAnimator.SetBool("isWalking", true);
        }
        else
        {
            characterAnimator.SetBool("isWalking", false);
        }


        if (dice.hasRolled)
        {
            if (Input.GetMouseButtonDown(0))
                //Debug.Log("LPM klikniêty - próba zaznaczenia pola.");
                TrySelectTile();
            else if (Input.GetMouseButtonDown(1))
                //Debug.Log("PPM klikniêty - próba odznaczenia pola.");
                TryDeselectTile();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("LPM klikniêty - próba zaznaczenia pola.");
                TrySelectTile();
                TrackLeftClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("PPM klikniêty - próba odznaczenia pola.");
                TryDeselectTile();
            }
        }
    }

    private void TrackLeftClick()
    {
        if (Time.time - lastClickTime > clickResetTime) leftClickCount = 0;

        leftClickCount++;
        lastClickTime = Time.time;


        if (leftClickCount > 20)
        {
            ShowFeedbackMessage("You need to roll the dice first!");
            leftClickCount = 0;
        }
    }

    private void ShowFeedbackMessage(string message)
    {
        if (feedbackPanelText != null)
        {
            feedbackPanelText.text = message;
            feedbackPanelText.gameObject.SetActive(true);
            Invoke(nameof(HideFeedbackMessage), 5f);
        }
    }

    private void HideFeedbackMessage()
    {
        if (feedbackPanelText != null) feedbackPanelText.gameObject.SetActive(false);
    }

    public void SetMaxSteps(int steps)
    {
        maxSteps = steps;
        //Debug.Log("Max Steps ustawione na: " + maxSteps);
    }

    public void HighlightPotentialMoves()
    {
        UpdateTileHighlights();
    }

    private void TrySelectTile()
    {
        if (selectedTiles.Count >= maxSteps)
            //Debug.Log("Osi¹gniêto maksymaln¹ liczbê zaznaczeñ.");
            return;

        var tile = GetTileUnderMouse();
        if (tile == null)
            //Debug.Log("Nie wykryto ¿adnego pola pod myszk¹.");
            return;

        if (selectedTiles.Contains(tile))
            //Debug.Log("To pole jest ju¿ zaznaczone.");
            return;

        var isAdjacent = selectedTiles.Count == 0
            ? tile.IsAdjacent(startingTile)
            : tile.IsAdjacent(selectedTiles[selectedTiles.Count - 1]);

        if (isAdjacent)
        {
            //Debug.Log("Pole przyleg³e: " + tile.name);

            if (!tile.HasObstacle() && tile.IsAvailableForSelection)
            {
                tile.Highlight(Color.cyan);
                selectedTiles.Add(tile);
                //Debug.Log("Pole zaznaczone: " + tile.name);
            }
            //Debug.Log("Pole ma przeszkodê lub jest niedostêpne: " + tile.name);
        }
        else
        {
            //Debug.Log("Pole nie jest przyleg³e do ostatniego zaznaczonego.");
        }

        UpdateTileHighlights();
    }

    private void TryDeselectTile()
    {
        var tile = GetTileUnderMouse();
        if (tile == null)
            //Debug.Log("Nie wykryto ¿adnego pola pod myszk¹ do odznaczenia.");
            return;

        if (!selectedTiles.Contains(tile))
            //Debug.Log("To pole nie jest zaznaczone.");
            return;

        var index = selectedTiles.IndexOf(tile);
        for (var i = selectedTiles.Count - 1; i >= index; i--)
        {
            selectedTiles[i].Highlight(Color.white);
            //Debug.Log("Pole odznaczone: " + selectedTiles[i].name);
            selectedTiles.RemoveAt(i);
        }

        UpdateTileHighlights();
    }

    private Tile GetTileUnderMouse()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var tile = hit.collider.GetComponent<Tile>();
            //Debug.Log("Pole pod myszk¹: " + (tile != null ? tile.name : "brak"));
            return tile;
        }

        //Debug.Log("Brak kolizji pod myszk¹.");
        return null;
    }

    public void OnReadyButtonClicked()
    {
        if (isProcessingMoves)
        {
            characterAnimator.SetBool("isWalking", true);
            //Debug.Log("Ruchy ju¿ trwaj¹. Przycisk 'Ready' jest zablokowany.");
            return;
        }

        characterAnimator.SetBool("isWalking", true);
        //Debug.Log("Przycisk 'Ready' klikniêty - rozpoczynanie ruchu.");
        isProcessingMoves = true;
        StartCoroutine(MoveCharacterAlongPath());
    }

    private void DeleteEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("No enemy to delete.");
            return;
        }

        Debug.Log($"Deleting enemy {enemy.name}.");
        Destroy(enemy.gameObject);
    }

    private void StartBattle(Enemy enemy)
    {
        Debug.Log("Rozpoczynanie walki z przeciwnikiem");

        if (character != null)
        {
            PlayerInfo.Instance.PlayerPosition = character.transform.position;
            //Debug.Log("Zapisano pozycjê gracza");
        }

        
        GameManager.Instance.SetCurrentEnemy(enemy);
        saveSystem.SaveCurrentState();
        SceneManager.LoadScene("Fight");
    }

    private void RestorePlayerPosition()
    {
        if (PlayerInfo.Instance != null)
        {
            if (character != null && PlayerInfo.Instance.PlayerPosition != Vector3.zero)
            {
                character.transform.position = PlayerInfo.Instance.PlayerPosition;
                //Debug.Log($"ustawiono pozycjê gracza na {PlayerInfo.Instance.PlayerPosition}");
            }
        }
        else
        {
            //Debug.Log("playerinfo jest null");
        }
    }

    private void SetStartingTileFromPlayerPosition()
    {
        if (character == null)
        {
            Debug.LogWarning("Brak modelu gracza. Nie mo¿na ustawiæ startingTile.");
            return;
        }

        if (tilemapGround == null)
        {
            Debug.LogWarning("Brak tilemapy. Nie mo¿na ustawiæ startingTile.");
            return;
        }

        var playerPosition = character.transform.position;
        Debug.Log($"playerPosition: {playerPosition}");
        var layerMask = LayerMask.GetMask("Tilemap_Ground");
        var ray = new Ray(playerPosition + Vector3.up * 10f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                startingTile = tile;
                Debug.Log($"starting tile ustawiony na: {tile.transform.position}");
            }
                
        }
    }

    private IEnumerator MoveCharacterAlongPath()
    {
        try
        {
            foreach (var tile in selectedTiles)
            {
                characterAnimator.SetBool("isWalking", true);

                var targetPosition = tile.transform.position;
                targetPosition.y = character.transform.position.y;

                var direction = targetPosition - character.transform.position;
                direction.y = 0;

                var targetRotation = Quaternion.identity;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                    targetRotation = direction.x > 0
                        ? Quaternion.Euler(0, 90, 0)
                        : Quaternion.Euler(0, 270, 0);
                else
                    targetRotation = direction.z > 0
                        ? Quaternion.Euler(0, 0, 0)
                        : Quaternion.Euler(0, 180, 0);

                while (Vector3.Distance(character.transform.position, targetPosition) > 0.1f)
                {
                    character.transform.rotation =
                        Quaternion.Slerp(character.transform.rotation, targetRotation, Time.deltaTime * 5);
                    character.transform.position = Vector3.MoveTowards(character.transform.position, targetPosition,
                        Time.deltaTime * 2);
                    yield return null;
                }

                characterAnimator.SetBool("isWalking", false);

                var tilePosition = targetPosition;
                var tileCellPosition = tilemapOverGround.WorldToCell(tilePosition);
                var worldPosition = tilemapOverGround.GetCellCenterWorld(tileCellPosition);
                var loweredPosition = new Vector3(worldPosition.x, worldPosition.y + 150f, worldPosition.z);
                LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");

                RaycastHit hit;
                if (Physics.Raycast(loweredPosition, Vector3.down, out hit, Mathf.Infinity))
                {
                    //Debug.Log($"Raycast trafi³ w obiekt: {hit.collider.name}");

                    var enemy = hit.collider.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        //Debug.Log($"Przeciwnik {enemy.name} (zdrowie = {enemy.HealthPoints} diff={enemy.DifficultyLevel} znaleziony na kafelku {tile.name}.");

                        characterAnimator.SetBool("isWalking", false);
                        StopAllCoroutines();
                        //Debug.Log("Zatrzymano dalszy ruch postaci.");

                        TileUnderCharacter = tile.transform.position;
                        //Debug.Log($"=================Pozycja przeciwnika {TileUnderCharacter}");
                        StartBattle(enemy);
                        //Debug.Log("Rozpoczêto walkê z przeciwnikiem.");

                        yield break;
                    }

                    var chest = hit.collider.GetComponent<Chest>();
                    if (chest != null)
                    {
                        //Debug.Log($"Znaleziono skrzynkê na kafelku {tile.name}.");
                        if (!chest.isOpened)
                            PlayerManager.Instance.AddCoins(chest.coins);
                        chest.OpenChest();
                        chest.CloseChest();
                        saveSystem.AddEntity(chest.transform.position, "chest");
                    }
                    //Debug.Log("Raycast trafi³ w obiekt, ale nie jest to przeciwnik.");

                    var shop = hit.collider.GetComponent<Shop>();
                    if (shop != null)
                    {
                        //Debug.Log($"Znaleziono sklep na kafelku {tile.name}.");
                        shop.OpenShop();
                    }
                }
                //Debug.Log($"Brak obiektu na kafelku {tile.name}. Przechodzê do kolejnego.");
            }
        }
        finally
        {
            //Debug.Log("Ruch zakoñczony. Czyszczenie zaznaczenia potencjalnych ruchów.");

            ClearPotentialMoveHighlights();

            if (selectedTiles.Count > 0) startingTile = selectedTiles[selectedTiles.Count - 1];

            foreach (var tile in selectedTiles) tile.Highlight(Color.white);
            selectedTiles.Clear();

            characterAnimator.SetBool("isWalking", false);
            isProcessingMoves = false;
            dice.hasRolled = false;
        }
    }


    private void ClearPotentialMoveHighlights()
    {
        foreach (var tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
            if (!selectedTiles.Contains(tile))
            {
                tile.Highlight(Color.white);
                tile.IsAvailableForSelection = false;
            }
    }

    private void UpdateTileHighlights()
    {
        foreach (var tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
            if (!selectedTiles.Contains(tile))
            {
                tile.Highlight(Color.white);
                tile.IsAvailableForSelection = false;
            }

        var referenceTile = selectedTiles.Count > 0 ? selectedTiles[selectedTiles.Count - 1] : startingTile;

        foreach (var tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            if (tile == referenceTile || selectedTiles.Contains(tile))
                continue;

            if (tile.IsAdjacent(referenceTile))
            {
                //Debug.Log("Sprawdzanie s¹siedniego pola: " + tile.name);

                var hasObstacle = tile.HasObstacle();
                var hasChest = tile.HasChest();
                var hasShop = tile.HasShop();
                var isInMaxRange = selectedTiles.Count < maxSteps;

                if (hasObstacle)
                {
                    tile.Highlight(Color.red);
                    tile.IsAvailableForSelection = false;
                }
                else if (hasChest)
                {
                    tile.Highlight(new Color(1.2f, 1f, 0f));
                    tile.IsAvailableForSelection = true;
                }
                else if (hasShop)
                {
                    tile.Highlight(new Color(0f, 0f, 1f));
                    tile.IsAvailableForSelection = true;
                }
                else if (!isInMaxRange)
                {
                    tile.Highlight(Color.red);
                    tile.IsAvailableForSelection = false;
                }
                else
                {
                    tile.Highlight(Color.green);
                    tile.IsAvailableForSelection = true;
                }
            }
        }
    }
}