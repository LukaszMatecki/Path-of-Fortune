using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tile startingTile;
    public int maxSteps;
    public GameObject character;
    public Animator characterAnimator;
    public Tilemap tilemapGround;  // Tilemap_Ground
    public Tilemap tilemapOverGround; // Tilemap_OverGround (przeszkody)
    public bool isActive = true;

    private List<Tile> selectedTiles = new List<Tile>();

    void Start()
    {

    }

    void Update()
    {
        if (!isActive) return;
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
        Debug.Log("Przycisk 'Ready' klikniêty - rozpoczynanie ruchu.");
        StartCoroutine(MoveCharacterAlongPath());
    }

    private System.Collections.IEnumerator MoveCharacterAlongPath()
    {
        try
        {
            foreach (Tile tile in selectedTiles)
            {
                characterAnimator.SetBool("isWalking", true);
                Debug.Log("Animacja chodzenia ustawiona na true");

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

                Debug.Log("Rotacja ustawiona na: " + targetRotation.eulerAngles);

                while (Vector3.Distance(character.transform.position, targetPosition) > 0.1f)
                {
                    character.transform.rotation = Quaternion.Slerp(character.transform.rotation, targetRotation, Time.deltaTime * 5);
                    character.transform.position = Vector3.MoveTowards(character.transform.position, targetPosition, Time.deltaTime * 2);
                    yield return null;
                }

                characterAnimator.SetBool("isWalking", false);
                Debug.Log("Animacja chodzenia ustawiona na false");

                Debug.Log("Postaæ przesz³a na pole: " + tile.name);

                if (tile.HasEvent())
                {
                    Debug.Log("Wykryto zdarzenie na polu " + tile.name + ". Zatrzymanie ruchu.");
                    yield break;
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
