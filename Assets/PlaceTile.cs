using UnityEngine;

public class TileDragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (gridManager.CanPlaceTile(transform.position))
        {
            transform.position = gridManager.SnapToGrid(transform.position);
        }
        else
        {
            Debug.Log("Nie mo¿na umieœciæ kafelka w tej pozycji.");
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 mousePosition = hit.point;
                mousePosition.y = 0;
                transform.position = mousePosition;
            }
        }
    }


}
