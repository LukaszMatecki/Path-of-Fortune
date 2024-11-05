using UnityEngine;
using System.Collections;

public class MoveCharacter : MonoBehaviour
{
    public Transform character;
    public float moveSpeed = 2f;
    public float jumpHeight = 0.5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Vector3Int upDirection = new Vector3Int(0, 0, 1);
    private Vector3Int downDirection = new Vector3Int(0, 0, -1);
    private Vector3Int leftDirection = new Vector3Int(-1, 0, 0);
    private Vector3Int rightDirection = new Vector3Int(1, 0, 0);

    private void Start()
    {
        if (character == null)
        {
            Debug.LogError("Character Transform is not assigned!");
        }
        else
        {
            Debug.Log("Character Transform is assigned correctly: " + character.name);
        }
    }

    private void Update()
    {
        Debug.Log("Current Position: " + character.position);

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveInDirection(upDirection);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveInDirection(downDirection);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveInDirection(leftDirection);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveInDirection(rightDirection);
            }
        }
    }

    private void MoveInDirection(Vector3Int direction)
    {
        if (!isMoving)
        {
            targetPosition = character.position + new Vector3(direction.x, 0, direction.z);
            Debug.Log("Target Position: " + targetPosition);
            StartCoroutine(MoveToPositionWithJump());
        }
    }

    private IEnumerator MoveToPositionWithJump()
    {
        isMoving = true;

        Vector3 startPosition = character.position;
        Vector3 endPosition = targetPosition;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += moveSpeed * Time.deltaTime;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            character.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(progress)) + new Vector3(0, yOffset, 0);

            yield return null;
        }

        character.position = endPosition;
        isMoving = false;
    }
}
