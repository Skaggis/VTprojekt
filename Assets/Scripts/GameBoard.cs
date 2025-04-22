
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;

    //kolla bara levande celler + grannar för ekonomin
    private readonly HashSet<Vector3Int> aliveCells = new();
    private readonly HashSet<Vector3Int> cellsToCheck = new();

    public int population { get; private set; }
    public int iterations { get; private set; }
    public float time { get; private set; }

    private void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Clear();

        Vector2Int center = pattern.GetCenter();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            //offsettas baserat på vart center är
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - center);
            //vect2 "castas" till vec3
            currentState.SetTile((Vector3Int)cell, aliveTile);
            aliveCells.Add(cell);
        }
        population = aliveCells.Count;

    }

    private void Clear()
    {
        aliveCells.Clear();
        cellsToCheck.Clear();

        currentState.ClearAllTiles();
        nextState.ClearAllTiles();

        population = 0;
        iterations = 0;
        time = 0f;
    }
    private void OnEnable()
    {
        StartCoroutine(Sim());
    }

    private IEnumerator Sim()
    {
        var interval = new WaitForSeconds(updateInterval);
        //waitforseconds allockerar minne? bättre att cacha?
        //därför egen cachad var ovan för waitforseconds
        //tradeoff: floaten kan inte ändras under Sim

        while (enabled)
        {
            UpdateState();
            
            population = aliveCells.Count;
            iterations++;
            time += updateInterval;

            yield return interval;
        }
        
    }
    private void UpdateState()
    {
        //rensa innan check
        cellsToCheck.Clear();

        //hitta grannarna! 3*3 matris
        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //cellsToCheck.Add(neighbor);
                    cellsToCheck.Add(cell + new Vector3Int(x, y));
                    //Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                    //cellsToCheck.Add(neighbor);
                }
            }

        }

        //transition till nästa state
        foreach (Vector3Int cell in cellsToCheck)
        {
            //TODO: räkna antal grannar som är vid liv
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 3)
            {
                //lever
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
            else if (alive && (neighbors < 2 || neighbors > 3))
            {
                //dör över/underpopulerat
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell);
            }
            else
            {
                //stannar som samma
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //måste kolla om neighbor lever, func nedan
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                
                //exkludera migsjälv, kolla enbart grannar för att avgöra mitt öde
                if (x == 0 && y == 0)
                {
                    continue;
                }
                else if (IsAlive(neighbor))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsAlive(Vector3Int cell)
    {
        //funkar bara när enbart celler som lever har en tile
        //return currentState.HasTile(cell);
        return currentState.GetTile(cell) == aliveTile;
        //om döda skulle ha tiles:
        //return currentState.HasTile(cell) == aliveTile;
    }
}
