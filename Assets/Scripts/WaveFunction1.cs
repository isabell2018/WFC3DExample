using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction1 : MonoBehaviour
{
    public int dimensions;//map dimensions
    public Tile[] tileObjects;//our tile prefabs
    public List<Cell> gridComponents;//all the area (,,)listed
    public Cell cellObj;//individual cell that will populate inside

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int z = 0; z < dimensions; z++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                for (int x = 0; x < dimensions; x++)
                {
                    Cell newCell = Instantiate(cellObj, new Vector3(x+7, y, z-7), Quaternion.identity);
                    newCell.CreateCell(false, tileObjects);
                    gridComponents.Add(newCell);
                }
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()//sorting out the tile w the least options
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)//first placed cell
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        Tile selectedTile = cellToCollapse.tileOptions
            [UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new Tile[] { selectedTile };

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);//tile instantiated

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);
        for (int z = 0; z < dimensions; z++)
        { 
            for (int y = 0; y < dimensions; y++) 
            {
                for (int x = 0; x < dimensions; x++) 
                { 
                    //var index = x + y * dimensions; 
                    Debug.Log($"x: {x}, y: {y}, z: {z}");
                    var index = x + y * dimensions + z * dimensions * dimensions;
                    // Add a check for out-of-bounds index
                    if (index < 0 || index >= gridComponents.Count)
                    {
                        Debug.LogError($"Index out of bounds: index = {index}, " +
                                       $"dimensions = {dimensions}, gridComponents.Count = {gridComponents.Count}");
                        continue;
                    }

                    if (gridComponents[index].collapsed) 
                    { 
                        Debug.Log("called"); 
                        newGenerationCell[index] = gridComponents[index]; 
                    }
                    else 
                    { 
                        List<Tile> options = new List<Tile>(); 
                        foreach (Tile t in tileObjects) 
                        {
                            options.Add(t); 
                        } 
                        
                        //update front (new)
                        if (z < dimensions - 1) 
                        { 
                            Cell front = gridComponents[x + y * dimensions + (z + 1) * dimensions * dimensions]; 
                            List<Tile> validOptions = new List<Tile>();
                            
                            foreach (Tile possibleOptions in front.tileOptions) 
                            { 
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions); 
                                var valid = tileObjects[valOption].frontNeighbours;
                                validOptions = validOptions.Concat(valid).ToList(); 
                            }
                            CheckValidity(options, validOptions); 
                        } 
                        
                        //update up
                        if (y > 0) 
                        { 
                            Cell up = gridComponents[x + (y - 1) * dimensions + z * dimensions * dimensions]; 
                            List<Tile> validOptions = new List<Tile>();
                            
                            foreach (Tile possibleOptions in up.tileOptions) 
                            { 
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions); 
                                var valid = tileObjects[valOption].upNeighbours;
                                validOptions = validOptions.Concat(valid).ToList(); 
                            }
                            CheckValidity(options, validOptions); 
                        } 
                        
                        //update right
                        if (x < dimensions - 1) 
                        { 
                            Cell right = gridComponents[x + 1 + y * dimensions + z * dimensions * dimensions]; 
                            List<Tile> validOptions = new List<Tile>();
                            
                            foreach (Tile possibleOptions in right.tileOptions) 
                            { 
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions); 
                                var valid = tileObjects[valOption].leftNeighbours;
                                validOptions = validOptions.Concat(valid).ToList(); 
                            }
                            CheckValidity(options, validOptions); 
                        } 
                        
                        //look back (new)
                        if (z > 0) 
                        {
                            Cell back = gridComponents[x + y * dimensions + (z - 1) * dimensions * dimensions];
                            List<Tile> validOptions = new List<Tile>();
                            
                            foreach (Tile possibleOptions in back.tileOptions) 
                            {
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].backNeighbours;
                                validOptions = validOptions.Concat(valid).ToList(); 
                            }
                            
                            CheckValidity(options, validOptions); 
                        }
                        
                        //look down
                        if (y < dimensions - 1) 
                        {
                            Cell down = gridComponents[x + (y + 1) * dimensions + z * dimensions * dimensions];
                            List<Tile> validOptions = new List<Tile>();
                            
                            foreach (Tile possibleOptions in down.tileOptions) 
                            {
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].downNeighbours;
                                validOptions = validOptions.Concat(valid).ToList(); 
                            }
                            
                            CheckValidity(options, validOptions); 
                        }

                        //look left
                        if (x > 0)
                        {
                            Cell left = gridComponents[x - 1 + y * dimensions + z * dimensions * dimensions];
                            List<Tile> validOptions = new List<Tile>();

                            foreach (Tile possibleOptions in left.tileOptions)
                            {
                                var valOption = Array.FindIndex
                                    (tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].rightNeighbours;

                                validOptions = validOptions.Concat(valid).ToList();
                            }

                            CheckValidity(options, validOptions);
                        }

                        Tile[] newTileList = new Tile[options.Count];

                        for (int i = 0; i < options.Count; i++)
                        {
                            newTileList[i] = options[i];
                        }

                        newGenerationCell[index].RecreateCell(newTileList);
                    }
                }
            }
        }

        gridComponents = newGenerationCell;
            iterations++;

        if(iterations < dimensions * dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
