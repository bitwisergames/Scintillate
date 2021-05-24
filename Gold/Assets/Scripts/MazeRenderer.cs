using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour {
    public static uint width = 25;

    public static uint height = 25;

    public static float size = 8f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform playerPrefab = null;

    [SerializeField]
    private Transform coinPrefab = null;

    [SerializeField]
    private Transform entryPrefab = null;

    [SerializeField]
    UnityEngine.AI.NavMeshSurface surface = null;

    // Start is called before the first frame update
    void Start() {
        WallState[,] maze = MazeGenerator.Generate(width, height);

        SpawnPlayer();
        Draw(maze);
    }

    private void Draw(WallState[,] maze) {
        Transform floor = Instantiate(floorPrefab, transform);
        floor.position = new Vector3(-0.5f * size, 0, -0.5f * size);
        floor.localScale = new Vector3((width / 10f) * size, 1 * size, (height / 10f) * size);
        floor.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(width * size, height * size);

        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                WallState cell = maze[x, y];
                Vector3 position = new Vector3((-width / 2f + x) * size, 0, (-height / 2f + y) * size);
                if (x != 0 || y != 0) {
                    if (cell.HasFlag(WallState.COIN)) {
                        Transform coin = Instantiate(coinPrefab, transform);
                        coin.position = position + new Vector3(0, 2, 0);
                    }

                    if (cell.HasFlag(WallState.UP)) {
                        Transform topWall = Instantiate(wallPrefab, transform);
                        topWall.position = position + new Vector3(0, 0, size / 2f);
                        topWall.localScale = new Vector3(topWall.localScale.x * size, topWall.localScale.y * size, topWall.localScale.z * size);
                    }

                    if (cell.HasFlag(WallState.LEFT)) {
                        Transform leftWall = Instantiate(wallPrefab, transform) as Transform;
                        leftWall.position = position + new Vector3(-size / 2f, 0, 0);
                        leftWall.localScale = new Vector3(leftWall.localScale.x * size, leftWall.localScale.y * size, leftWall.localScale.z * size);
                        leftWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }
                else {
                    Transform entry = Instantiate(entryPrefab, transform);
                    entry.position = position + new Vector3(-size / 2f, 0, 0); ;
                    entry.localScale = new Vector3(entry.localScale.x * size, entry.localScale.y * size, entry.localScale.z * size);
                }

                if (x == width - 1) {
                        if (cell.HasFlag(WallState.RIGHT)) {
                            Transform rightWall = Instantiate(wallPrefab, transform) as Transform;
                            rightWall.position = position + new Vector3(size / 2f, 0, 0);
                            rightWall.localScale = new Vector3(rightWall.localScale.x * size, rightWall.localScale.y * size, rightWall.localScale.z * size);
                            rightWall.eulerAngles = new Vector3(0, 90, 0);
                        }
                    }

                    if (y == 0) {
                        if (cell.HasFlag(WallState.DOWN)) {
                            Transform bottomWall = Instantiate(wallPrefab, transform) as Transform;
                            bottomWall.position = position + new Vector3(0, 0, -size / 2f);
                            bottomWall.localScale = new Vector3(bottomWall.localScale.x * size, bottomWall.localScale.y * size, bottomWall.localScale.z * size);
                        }
                    }
            }
        }
        

        surface.BuildNavMesh();
    }

    private void SpawnPlayer() {
        Transform player = Instantiate(playerPrefab, new Vector3((-0.5f * width * size) - (2 * size), 0.2f, -0.5f * height * size), transform.rotation);
    }
}
