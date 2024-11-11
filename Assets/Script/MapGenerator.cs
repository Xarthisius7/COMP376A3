using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject mapChunkPrefab; // The prefab for each map chunk.
    public List<GameObject> objectPrefabs; // List of prefabs to randomly instantiate in each chunk.
    public int initialChunks = 4; // Number of initial map chunks to generate.
    public float chunkLength = 90f; // Distance between each map chunk.
    public float generateDistance = 30f; // Distance at which we generate new chunks when player approaches.
    public float destroyDistance = 180f; // Distance at which we destroy old chunks when player is far away.
    public int gridX = 10; // Number of grids in x direction.
    public int gridZ = 10; // Number of grids in z direction.
    public float xOffset = 10f; // X offset between each grid.
    public float zOffset = 10f; // Z offset between each grid.
    public float yLevel = 0f; // Y level for each grid.

    private List<MapChunk> mapChunks = new List<MapChunk>();
    private Transform playerTransform;

    private bool FirstChunk = true;

    private int nextChunkPos = 0;


    public float TowerBaseRate;
    public float EnemyBaseRate;
    public float ObsticleBaseRate;
    public float PowerUpBaseRate;
    public float MiscBaseRate;

    public float ObsticleExtendRate; 




    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateInitialChunks();
    }

    void Update()
    {
        MonitorPlayerPosition();
    }

    private void GenerateInitialChunks()
    {
        for (int i = 0; i < initialChunks; i++)
        {
            Vector3 chunkPosition = new Vector3(0, 0, nextChunkPos * chunkLength);
            nextChunkPos++;
            GenerateChunk(chunkPosition);
        }
    }

    private void GenerateChunk(Vector3 position)
    {

        position.y = 8.1f;
        GameObject newChunk = Instantiate(mapChunkPrefab, position, Quaternion.identity);
        MapChunk mapChunk = newChunk.AddComponent<MapChunk>();
        mapChunk.position = position;
        mapChunk.hasGenerated = false;

        if (!FirstChunk)
        {
            //GenerateGrid(mapChunk);
        } else
        {
            FirstChunk = false;
        }
        mapChunks.Add(mapChunk);

    }

    private void MonitorPlayerPosition()
    {
        foreach (var chunk in mapChunks)
        {
            if (!chunk.hasGenerated && Vector3.Distance(playerTransform.position, chunk.position) <= generateDistance)
            {
                chunk.hasGenerated = true;
                GenerateGrid(chunk);
            }
            else if (chunk.hasGenerated && Vector3.Distance(playerTransform.position, chunk.position) > destroyDistance)
            {
                DestroyChunk(chunk);
                Vector3 chunkPosition = new Vector3(0, 0, nextChunkPos * chunkLength);
                nextChunkPos++;
                GenerateChunk(chunkPosition);
                break;
            }
        }
    }

    private void DestroyChunk(MapChunk chunk)
    {
        mapChunks.Remove(chunk);
        Destroy(chunk.gameObject);
    }

    private void GenerateGrid(MapChunk mapChunk)
    {
        int[,] grid = new int[gridX, gridZ];
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                grid[x, z] = -1; 
            }
        }

        Vector3 gridStart = mapChunk.position - new Vector3((gridX / 2) * xOffset, 0, -90 + (gridZ / 2) * zOffset);

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (grid[x, z] == -1 && !HasTowerNearby(grid, x, z, 3))
                {
                    if (Random.value <= TowerBaseRate)
                    {
                        grid[x, z] = 2;
                    }
                }
            }
        }

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (grid[x, z] == 2)
                {
                    SpawnEnemiesAroundTower(grid, x, z);
                }
            }
        }

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (grid[x, z] == -1)
                {
                    if (Random.value <= ObsticleBaseRate * DifficultyManager.instance.obstacleDensity)
                    {
                        grid[x, z] = 3;
                        SpreadObstacle(grid, x, z);
                    }
                }
            }
        }

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (grid[x, z] == -1)
                {
                    if (Random.value <= PowerUpBaseRate)
                    {
                        grid[x, z] = Random.Range(4, 6);
                    }
                }
            }
        }

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (grid[x, z] == -1)
                {
                    if (Random.value <= MiscBaseRate)
                    {
                        grid[x, z] = Random.Range(0, 2);
                    }
                }
            }
        }

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if(grid[x, z] == 9)
                {
                    grid[x, z] = 3;
                }
                if (grid[x, z] != -1)
                {
                    Vector3 gridPosition = gridStart + new Vector3(x * xOffset, yLevel, z * zOffset);
                    GameObject obj = Instantiate(objectPrefabs[grid[x, z]], gridPosition, Quaternion.identity);
                    Enemy em = obj.GetComponent<Enemy>();
                    if(em != null)
                    {
                        em.enemyStrengh = DifficultyManager.instance.enemyStrength;
                    }
                    obj.transform.SetParent(mapChunk.transform);
                }
            }
        }
    }

    private bool HasTowerNearby(int[,] grid, int x, int z, int radius)
    {
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int nx = x + i;
                int nz = z + j;
                if (nx >= 0 && nx < gridX && nz >= 0 && nz < gridZ)
                {
                    if (grid[nx, nz] == 2) return true;
                }
            }
        }
        return false;
    }

    private void SpawnEnemiesAroundTower(int[,] grid, int x, int z)
    {
        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                int nx = x + i;
                int nz = z + j;
                if (nx >= 0 && nx < gridX && nz >= 0 && nz < gridZ && grid[nx, nz] == -1)
                {
                    if (Random.value <= EnemyBaseRate * DifficultyManager.instance.enemySpawnRate)
                    {
                        grid[nx, nz] = Random.Range(0, 2); 
                    }
                }
            }
        }
    }

    private void SpreadObstacle(int[,] grid, int x, int z)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nx = x + i;
                int nz = z + j;
                if (nx >= 0 && nx < gridX && nz >= 0 && nz < gridZ && grid[nx, nz] == -1)
                {
                    if (Random.value <= ObsticleExtendRate)
                    {
                        grid[nx, nz] = 9; 
                    }
                }
            }
        }
    }

}

public class MapChunk : MonoBehaviour
{
    public Vector3 position;
    public bool hasGenerated = false;
}
