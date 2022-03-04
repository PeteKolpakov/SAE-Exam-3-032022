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

    private int columnCount;
    private int rowCount;

    private List<int> topwall;
    private List<int> bottomwall;
    private List<int> leftwall;
    private List<int> rightwall;

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
        columnCount = cellCount_X;
        rowCount = cellCount_Y;

        for (int i = 0; i < grid.CellCount; i++)
        {
            int rndVal = Random.Range(0, prefabs.Length);
            grid.SetElement(i, new Element(prefabs[rndVal], rndVal));

            GameObject.Instantiate(grid.GetElement(i).Visuals, grid.GetCellCenter(i), Quaternion.identity);
            Debug.Log(i + " " + grid.GetElement(i).ElementType);
        }

        PopulateWallLists();

    }

    private void PopulateWallLists()
    {
        int index = 0;
        topwall = new List<int>();
        for (int i = 0; i < columnCount; i++)
        {
            if (index > grid.CellCount) break;

            topwall.Add(index);
            index += rowCount;
        }

        index = columnCount * (rowCount - 1);
        bottomwall = new List<int>();
        for (int i = 0; i < columnCount; i++)
        {
            if (index > grid.CellCount) break;

            bottomwall.Add(index);
            index += rowCount;
        }

        index = 0;
        leftwall = new List<int>();
        for (int i = 0; i < rowCount; i++)
        {
            if (index > grid.CellCount) break;

            leftwall.Add(index);
            index += columnCount;
        }

        index = columnCount - 1;
        rightwall = new List<int>();
        for (int i = 0; i < rowCount; i++)
        {
            if (index > grid.CellCount) break;

            rightwall.Add(index);
            index += columnCount;
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
        // comment the out the following line
        return -99;
    }

    /// <summary>
    /// This Function implements the funcionality for MouseClick events. It is called when a mouseclick was detected to make a move (remove adjacent Elements if there are more then two adjacent elements of the same type beginning at the worldPosition).
    /// <BR><B>Attention:</B> The given worldPosition is not necessarily inside of the grid bounds.
    /// </summary>
    /// <returns> The Number of the selected/removed cells. </returns>
    /// <param name="worldPosition">Point in worldcoordinates (this position is not necessarily inside of the grid bounds).</param>
    public int SelectCells(Vector3 worldPosition)
    {
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

        if (grid.GetElement(cellIndex) == null)
        {
            return null;
        }

        int[] cells = new int[5];

        cells[0] = cellIndex;

        // up    = -x
        cells[1] = cellIndex - columnCount;
        if (topwall.Contains(cellIndex))
        {
            cells[1] = -100;
        }
        // down  = +x
        cells[2] = cellIndex + columnCount;
        if (bottomwall.Contains(cellIndex))
        {
            cells[2] = -100;
        }
        // left  = -1
        cells[3] = cellIndex - 1;
        if (leftwall.Contains(cellIndex))
        {
            cells[3] = -100;
        }
        // right = +1
        cells[4] = cellIndex + 1;
        if (rightwall.Contains(cellIndex))
        {
            cells[4] = -100;
        }

        return cells;
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