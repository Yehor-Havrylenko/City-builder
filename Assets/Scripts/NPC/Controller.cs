using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace NPC
{
    public class Controller : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Ydir = Animator.StringToHash("Ydir");
        [SerializeField] private float speed = 2f;
        [SerializeField] private int maxRecentPositions = 5;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Tilemap _roadTilemap;
        private Vector3Int _currentTile;
        private Vector3Int _targetTile;
        private Queue<Vector3Int> _recentPositions = new();
        private bool _isMoving;
        private bool _isPaused;
        private Coroutine _pauseCoroutine;
        
        public void Initialize(Tilemap roadTilemap, Vector3 position)
        {
            _roadTilemap = roadTilemap;
            transform.position = position;
            _currentTile = _roadTilemap.WorldToCell(transform.position);
            ChooseNextTarget();
            _pauseCoroutine = StartCoroutine(RandomPause());
        }
        
        private void OnDestroy()
        {
            if (_pauseCoroutine != null) StopCoroutine(_pauseCoroutine);
        }

        private void Update()
        {
            
            if (_isMoving && _isPaused == false)
            {
                MoveToTarget();
            }
            else if (_isMoving == false && _isPaused == false)
            {
                ChooseNextTarget();
            }
        }
        
        private IEnumerator RandomPause()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
                _isPaused = true;
                animator.SetFloat(Speed, 0);
                float pauseDuration = Random.Range(2f, 3f);

                string idleAnimation = Random.Range(0, 2) == 0 ? "Idle1" : "Idle2";
                animator.SetTrigger(idleAnimation);

                yield return new WaitForSeconds(pauseDuration);
                _isPaused = false;
            }
        }

        private void ChooseNextTarget()
        {
            List<Vector3Int> validTiles = GetValidAdjacentTiles(_currentTile);

            validTiles.RemoveAll(tile => _recentPositions.Contains(tile));

            if (validTiles.Count > 0)
            {
                _targetTile = validTiles[Random.Range(0, validTiles.Count)];
                AddPositionToRecent(_currentTile);
                _isMoving = true;

                Vector3 direction =
                    (_roadTilemap.GetCellCenterWorld(_targetTile) - _roadTilemap.GetCellCenterWorld(_currentTile))
                    .normalized;
                UpdateAnimation(direction);
            }
            else
            {
                if (_recentPositions.Count > 0)
                {
                    _recentPositions.Dequeue();
                }

                ChooseNextTarget();
            }
        }

        private void MoveToTarget()
        {
            Vector3 targetPosition = _roadTilemap.GetCellCenterWorld(_targetTile);
            targetPosition.z = targetPosition.y;
            var newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = newPosition;

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                _isMoving = false;
                _currentTile = _targetTile;
            }
        }

        private List<Vector3Int> GetValidAdjacentTiles(Vector3Int currentTile)
        {
            List<Vector3Int> adjacentTiles = new List<Vector3Int>
            {
                currentTile + new Vector3Int(1, 0, 0),
                currentTile + new Vector3Int(-1, 0, 0),
                currentTile + new Vector3Int(0, 1, 0),
                currentTile + new Vector3Int(0, -1, 0)
            };

            adjacentTiles.RemoveAll(tile => !IsRoadTile(tile));
            return adjacentTiles;
        }

        private bool IsRoadTile(Vector3Int tilePosition)
        {
            TileBase tile = _roadTilemap.GetTile(tilePosition);
            return tile != null && tile is RuleTile;
        }

        private void AddPositionToRecent(Vector3Int position)
        {
            if (_recentPositions.Count >= maxRecentPositions)
            {
                _recentPositions.Dequeue();
            }

            _recentPositions.Enqueue(position);
        }

        private void UpdateAnimation(Vector3 direction)
        {
            animator.SetFloat(Speed, speed);

            animator.SetFloat(Ydir, direction.y);

            if (direction.x != 0 && direction.y < 0)
            {
                spriteRenderer.flipX = direction.x > 0;
            }

            else if (direction.x != 0 && direction.y > 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
    }
}