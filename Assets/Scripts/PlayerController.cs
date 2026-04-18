using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;


    private GridManager gridManager;
    private Vector2Int currentGridPos;

    public bool IsMoving { get; private set; }

    public void Init(GridManager manager, Vector2Int startPos)
    {
        gridManager = manager;
        currentGridPos = startPos;
        transform.position = gridManager.GridToWorld(startPos);
    }

    public void MoveAlongPath(List<Vector2Int> path, Action onComplete)
    {
        if (IsMoving)
            return;

        StartCoroutine(MoveCoroutine(path, onComplete));
    }

    private IEnumerator MoveCoroutine(List<Vector2Int> path, Action onComplete)
    {
        IsMoving = true;
        int a = 1;

        foreach (Vector2Int step in path)
        {
            Vector3 targetWorldPos = gridManager.GridToWorld(step);

            if (a % 2 == 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -15); // tilt right
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 15); // tilt left
            }
            a++;

            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWorldPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetWorldPos;
            currentGridPos = step;
        }

        transform.rotation = new Quaternion(0, 0, 0, 1); // reset rotation
        IsMoving = false;
        onComplete?.Invoke();
    }

    public void Dig()
    {
        IsMoving = true;
    }

    public void DoneDigging()
    {
        IsMoving = false;
    }
}