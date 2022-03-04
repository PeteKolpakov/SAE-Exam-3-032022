using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respresents a game level with a playground of cellCount_X * cellCount_Y elements.
/// </summary>
public class Level
{
    /// <summary>
    /// Possible States of the currentLevel. The GameManager decides when to end the level (Win/Loose) upon the current state of the level.
    /// </summary>
    public enum LevelState
    {
        /// <summary>
        /// The Level is "empty". All <see cref="Element"/>s of the ElementGrid were removed by the player through Click-Events.
        /// </summary>
        NoElementsLeft,
        /// <summary>
        /// There are still <see cref="Element"/>s in the level but the player can't perform moves. Means that there are no <see cref="Element"/>s with Neighbours of the same type (<see cref="Element.ElementType"/>) left.
        /// </summary>
        NoMoreMovesPossible,
        /// <summary>
        /// This is the DEFAULT state. This State remains as long as the player has the possibility to remove further <see cref="Element"/>s from the level.
        /// </summary>
        FurtherMovesPossible
    };

    //---Member---
    /// <summary>
    /// Returns the Playerscore (points the player has made).
    /// </summary>
    int points;
    /// <summary>
    /// Returns or set the Playerscore (points the player has made).
    /// </summary>
    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    /// <summary>
    /// The playground.
    /// </summary>
    ElementGrid grid;

    //------STUDENTS IMPLEMENT FUNCTIONALITY BELOW---------------------------------------------------------------------
    //---Constructor---
    /// <summary>
    /// Creates a new Level of the SameGame. Initializes the ElementGrid, fills the ElementgGrid with new random Elements (determined by the prefabs and the randomGeneratorSeed) and creates instances of the element visuals of the prefabs in the scene.
    /// </summary>
    /// <param name="origin">The Origin of the level grid.</param>
    /// <param name="cellSize">Size of each cell of the level grid. </param>
    /// <param name="cellCount_X">Number of cells in x-direction.</param>
    /// <param name="cellCount_Y">Number of cells in y-direction.</param>
    /// <param name="seedForrandomNumberGenerator">Seed for the random number generator to reproduce random levels.</param>
    /// <param name="prefabs">The Prefabs for the Element visuals in the Unity scene. The number of different element types corresponds to the number of prefabs.</param>
    /// <param name="parent">The Parent for all Element visuals in the scene.</param>
    public Level(Vector3 origin, Vector2 cellSize, int cellCount_X, int cellCount_Y, int seedForRandomNumberGenerator, GameObject[] prefabs, Transform parent)
    {
        // Don't delete the following lines
        grid = new ElementGrid(origin, cellSize, cellCount_X, cellCount_Y);
        points = 0;
        Random.InitState(seedForRandomNumberGenerator);
        // ***** Students Start here ******

        for (int i = 0; i < grid.CellCount; i++)
        {
            int rndVal = Random.Range(0, prefabs.Length);
            var sphere = GameObject.Instantiate(prefabs[rndVal], grid.GetCellCenter(i), Quaternion.identity);
            grid.SetElement(i, new Element(sphere, rndVal));            
        }

    }

    //---Methods---
    /// <summary>
    /// This Function implements the funcionality for MouseHover events. It is called each frame if no MouseClick is detected.
    /// <BR><B>Attention:</B> The given worldPosition is not necessarily inside of the grid bounds.
    /// </summary>
    /// <returns> The Number of the hovered cells. </returns>
    /// <param name="worldPosition">Point in worldcoordinates (this position is not necessarily inside of the grid bounds).</param>
    public int HoverCells(Vector3 worldPosition)
    {

        // calculate index of cell from mouse position (in world coordinates)
        int index = grid.PointToIndex(worldPosition);

        if (index == -1)
        {
            return -1;
        }
        // fetch adjacent elements of the same type
        List<int> allConnectedSpheres = GetValidNeighbours(index);


        // rotate similar elements around y axis at 45* per second (accounting for the method beind called once a frame)
        GameObject sphere = null;
        foreach (var item in allConnectedSpheres)
        {
            sphere = grid.GetElement(item).Visuals;
            sphere.transform.RotateAround(sphere.transform.position, new Vector3(0, 1, 0), 45 * Time.deltaTime);
        }

        // comment the out the following line
        return index;
    }

    private List<int> GetValidNeighbours(int index)
    {
        int[] goodNeighbours = GetAdjacentCellsOfSameType(index);
        List<int> allConnectedSpheres = new List<int>();
        foreach (var item in goodNeighbours)
        {
            GetNeighborsOfNeighbors(allConnectedSpheres, item);
        }

        return allConnectedSpheres;
    }

    private void GetNeighborsOfNeighbors(List<int> allConnectedSpheres, int target)
    {
        int[] spheres = GetAdjacentCellsOfSameType(target);
        foreach (var goodSphere in spheres)
        {
            if (!allConnectedSpheres.Contains(goodSphere))
            {
                allConnectedSpheres.Add(goodSphere);
            }
        }
    }

    /// <summary>
    /// This Function implements the funcionality for MouseClick events. It is called when a mouseclick was detected to make a move (remove adjacent Elements if there are more then two adjacent elements of the same type beginning at the worldPosition).
    /// <BR><B>Attention:</B> The given worldPosition is not necessarily inside of the grid bounds.
    /// </summary>
    /// <returns> The Number of the selected/removed cells. </returns>
    /// <param name="worldPosition">Point in worldcoordinates (this position is not necessarily inside of the grid bounds).</param>
    public int SelectCells(Vector3 worldPosition)
    {
        int index = grid.PointToIndex(worldPosition);

        if (index == -1)
        {
            return -1;
        }

        List<int> allConnectedSpheres = GetValidNeighbours(index);

        if (allConnectedSpheres.Count > 1)
        {
            points += CalculatePoints(allConnectedSpheres.Count);
            grid.RemoveElements(allConnectedSpheres.ToArray());
        }


        // comment the out the following line
        return -99;
    }

    /// <summary>
    /// Returns the indices of adjacent cells with the same type (the type of the Element of the cell at cellIndex) as an array of integer indices.
    /// </summary>
    /// <returns>An array of integer indices of adjacent cells with the same type (including the selected cell) (<see cref="Element.ElementType"/>) or null (if the selected cell is empty).</returns>
    /// <param name="cellIndex">Index of the original cell.</param>
    public int[] GetAdjacentCellsOfSameType(int cellIndex)
    {
        // comment the out the following line
        //return null;

        int[] neighbours = grid.GetNeighbours(cellIndex);
        Element target = grid.GetElement(cellIndex);
        List<int> result = new List<int>();

        result.Add(cellIndex);

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (grid.GetElement(neighbours[i]).ElementType == grid.GetElement(cellIndex).ElementType)
            {
                result.Add(neighbours[i]);
            }
        }   

        return result.ToArray();
    }

    /// <summary>
    /// Checks the state of the current Level. 
    /// <returns><c>NoElementsLeft</c>: if there are no more (not empty) Elements left; <BR><c>NoMoreMovesPossible</c>: if there are no more moves possible (not empty Elements left, but only single Elements without neighbours of the same type); <BR><c>FurtherMovesPossible</c>: otherwise (see <see cref="Level.LevelState"/>).</returns>
    /// </summary>
    public LevelState CheckLevelState()
    {
        // comment the out the following line
        //return LevelState.NoElementsLeft;



        return LevelState.FurtherMovesPossible;
    }

    /// <summary>
    /// Calculates the points for each move (by the number of involved Elements of the same type) or if the player looses (by the number of the remaining Elements in the level).
    /// </summary>
    /// <returns>The points to add or substract to the players score.</returns>
    /// <param name="numElements">Number of elements.</param>
    public int CalculatePoints(int numElements)
    {
        // comment the out the following line
        //return -99;

        //Points = (Amount of Stones - 2)²

        return (int)Mathf.Pow(numElements - 2, 2);
    }

    /// <summary>
    /// Destroys the level. Cleanup the Grid.
    /// This Function is calle by the GameManager.
    /// </summary>
    public void DestroyLevel()
    {
        // Don't delete the following lines!
        if (grid != null)
        {
            grid.DestroyGrid();
            grid = null;
        }
    }
}