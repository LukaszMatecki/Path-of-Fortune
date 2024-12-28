using System.Collections.Generic;
using GG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tile startingTile;
    public int maxSteps;
    public GameObject character;
    public Animator characterAnimator;
    public Tilemap tilemapGround;
    public Tilemap tilemapOverGround;
    public bool isActive = true;
    public GameObject playerCharacter;
    private List<Tile> selectedTiles = new List<Tile>();
    private bool isProcessingMoves = false;

    void Start()
    {

    }

    void Update()
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

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("LPM klikniêty - próba zaznaczenia pola.");
            TrySelectTile();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("PPM klikniêty - próba odznaczenia pola.");
            TryDeselectTile();
        }
    }

    public void SetMaxSteps(int steps)
    {
        maxSteps = steps;
        Debug.Log("Max Steps ustawione na: " + maxSteps);
    }
    public void HighlightPotentialMoves()
    {
        UpdateTileHighlights();
    }

    private void TrySelectTile()
    {
        if (selectedTiles.Count >= maxSteps)
        {
            Debug.Log("Osi¹gniêto maksymaln¹ liczbê zaznaczeñ.");
            return;
        }

        Tile tile = GetTileUnderMouse();
        if (tile == null)
        {
            Debug.Log("Nie wykryto ¿adnego pola pod myszk¹.");
            return;
        }

        if (selectedTiles.Contains(tile))
        {
            Debug.Log("To pole jest ju¿ zaznaczone.");
            return;
        }

        bool isAdjacent = selectedTiles.Count == 0
            ? tile.IsAdjacent(startingTile)
            : tile.IsAdjacent(selectedTiles[selectedTiles.Count - 1]);

        if (isAdjacent)
        {
            Debug.Log("Pole przyleg³e: " + tile.name);

            if (!tile.HasObstacle() && tile.IsAvailableForSelection)
            {
                tile.Highlight(Color.cyan);
                selectedTiles.Add(tile);
                Debug.Log("Pole zaznaczone: " + tile.name);
            }
            else
            {
                Debug.Log("Pole ma przeszkodê lub jest niedostêpne: " + tile.name);
            }
        }
        else
        {
            Debug.Log("Pole nie jest przyleg³e do ostatniego zaznaczonego.");
        }

        UpdateTileHighlights();
    }

    private void TryDeselectTile()
    {
        Tile tile = GetTileUnderMouse();
        if (tile == null)
        {
            Debug.Log("Nie wykryto ¿adnego pola pod myszk¹ do odznaczenia.");
            return;
        }

        if (!selectedTiles.Contains(tile))
        {
            Debug.Log("To pole nie jest zaznaczone.");
            return;
        }

        int index = selectedTiles.IndexOf(tile);
        for (int i = selectedTiles.Count - 1; i >= index; i--)
        {
            selectedTiles[i].Highlight(Color.white);
            Debug.Log("Pole odznaczone: " + selectedTiles[i].name);
            selectedTiles.RemoveAt(i);
        }

        UpdateTileHighlights();
    }

    private Tile GetTileUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            Debug.Log("Pole pod myszk¹: " + (tile != null ? tile.name : "brak"));
            return tile;
        }
        Debug.Log("Brak kolizji pod myszk¹.");
        return null;
    }

    public void OnReadyButtonClicked()
    {
        if (isProcessingMoves)
        {
            characterAnimator.SetBool("isWalking", true);
            Debug.Log("Ruchy ju¿ trwaj¹. Przycisk 'Ready' jest zablokowany.");
            return;
        }

        characterAnimator.SetBool("isWalking", true);
        Debug.Log("Przycisk 'Ready' klikniêty - rozpoczynanie ruchu.");
        isProcessingMoves = true; // Ustawiamy flagê na true
        StartCoroutine(MoveCharacterAlongPath());
    }

    private void StartBattle(Enemy enemy)
    {
        Debug.Log("weszliœmy to funkcji startbattle");

        StopAllCoroutines();

        BattleManager.Instance.SetupBattle(enemy);
        BattleManager.Instance.SetPlayerCharacter(playerCharacter);
        Debug.Log("Talia przeciwnika: " + enemy.DeckSO.name);

        SceneManager.LoadScene("Fight");
    }

    private System.Collections.IEnumerator MoveCharacterAlongPath()
    {
        try
        {
            foreach (Tile tile in selectedTiles)
            {
                characterAnimator.SetBool("isWalking", true);

                Vector3 targetPosition = tile.transform.position;
                targetPosition.y = character.transform.position.y;

                Vector3 direction = targetPosition - character.transform.position;
                direction.y = 0;

                Quaternion targetRotation = Quaternion.identity;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    targetRotation = direction.x > 0
                        ? Quaternion.Euler(0, 90, 0)
                        : Quaternion.Euler(0, 270, 0);
                }
                else
                {
                    targetRotation = direction.z > 0
                        ? Quaternion.Euler(0, 0, 0)
                        : Quaternion.Euler(0, 180, 0);
                }

                while (Vector3.Distance(character.transform.position, targetPosition) > 0.1f)
                {
                    character.transform.rotation = Quaternion.Slerp(character.transform.rotation, targetRotation, Time.deltaTime * 5);
                    character.transform.position = Vector3.MoveTowards(character.transform.position, targetPosition, Time.deltaTime * 2);
                    yield return null;
                }

                //characterAnimator.SetBool("isWalking", false);

                Vector3 tilePosition = targetPosition;
                Vector3Int tileCellPosition = tilemapOverGround.WorldToCell(tilePosition);
                Vector3 worldPosition = tilemapOverGround.GetCellCenterWorld(tileCellPosition);
                Vector3 loweredPosition = new Vector3(worldPosition.x, worldPosition.y + 150f, worldPosition.z);
                LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");
                
                RaycastHit hit;
                if (Physics.Raycast(loweredPosition, Vector3.down, out hit, Mathf.Infinity))
                {
                    Debug.Log($"Raycast trafi³ w obiekt: {hit.collider.name}");

                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        Debug.Log($"Przeciwnik {enemy.name} znaleziony na kafelku {tile.name}.");

                        // Przypisanie przeciwnika do BattleManager
                        BattleManager.Instance.CurrentEnemy = enemy;

                        // Zatrzymanie ruchu postaci
                        characterAnimator.SetBool("isWalking", false);
                        StopAllCoroutines();
                        Debug.Log("Zatrzymano dalszy ruch postaci.");

                        // Rozpoczêcie walki
                        StartBattle(enemy);
                        Debug.Log("Rozpoczêto walkê z przeciwnikiem.");

                        yield break;
                    }
                    else
                    {
                        Debug.Log("Raycast trafi³ w obiekt, ale nie jest to przeciwnik.");
                    }
                }
                else
                {
                    Debug.Log($"Brak obiektu na kafelku {tile.name}. Przechodzê do kolejnego.");
                }

            }
        }
        finally
        {
            Debug.Log("Ruch zakoñczony. Czyszczenie zaznaczenia potencjalnych ruchów.");

            ClearPotentialMoveHighlights();

            if (selectedTiles.Count > 0)
            {
                startingTile = selectedTiles[selectedTiles.Count - 1];
            }

            foreach (Tile tile in selectedTiles)
            {
                tile.Highlight(Color.white);
            }
            selectedTiles.Clear();

            characterAnimator.SetBool("isWalking", false);
            isProcessingMoves = false;
        }
    }



    private void ClearPotentialMoveHighlights()
    {
        foreach (Tile tile in UnityEngine.Object.FindObjectsByType<Tile>(UnityEngine.FindObjectsSortMode.None))
        {
            if (!selectedTiles.Contains(tile))
            {
                tile.Highlight(Color.white);
                tile.IsAvailableForSelection = false;
            }
        }
    }

    private void UpdateTileHighlights()
    {
        // Resetujemy wszystkie kafelki na bia³e
        foreach (Tile tile in UnityEngine.Object.FindObjectsByType<Tile>(UnityEngine.FindObjectsSortMode.None))
        {
            if (!selectedTiles.Contains(tile))
            {
                tile.Highlight(Color.white);
                tile.IsAvailableForSelection = false; // Domyœlnie pole nie jest dostêpne
            }
        }

        // ZnajdŸ przyleg³e do ostatniego zaznaczonego lub startowego
        Tile referenceTile = selectedTiles.Count > 0 ? selectedTiles[selectedTiles.Count - 1] : startingTile;

        // Iterujemy po wszystkich kafelkach
        foreach (Tile tile in UnityEngine.Object.FindObjectsByType<Tile>(UnityEngine.FindObjectsSortMode.None))
        {
            if (tile == referenceTile || selectedTiles.Contains(tile))
                continue;  // Pomijamy ju¿ zaznaczone kafelki

            // Sprawdzamy, czy pole jest s¹siednie do ostatniego zaznaczonego
            if (tile.IsAdjacent(referenceTile))
            {
                Debug.Log("Sprawdzanie s¹siedniego pola: " + tile.name);

                // Sprawdzamy, czy na polu nie ma przeszkód oraz czy jest dostêpne do zaznaczenia
                bool hasObstacle = tile.HasObstacle();
                bool isInMaxRange = selectedTiles.Count < maxSteps;

                if (hasObstacle)
                {
                    Debug.Log("Pole ma przeszkodê: " + tile.name);
                    tile.Highlight(Color.red);
                    tile.IsAvailableForSelection = false;
                }
                else if (!isInMaxRange)
                {
                    Debug.Log("Pole poza dozwolonym zasiêgiem: " + tile.name);
                    tile.Highlight(Color.red);
                    tile.IsAvailableForSelection = false;
                }
                else
                {
                    tile.Highlight(Color.green);
                    tile.IsAvailableForSelection = true;
                    Debug.Log("Pole dostêpne do zaznaczenia: " + tile.name);
                }
            }
        }
    }



}
