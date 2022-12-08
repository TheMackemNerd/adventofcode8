try
{
    string path = "C:\\adventofcode\\day8\\data.txt";
    StreamReader sr = new(path);

    int rows = 0;
    int cols = 0;

    //Quick loop of the file to calculate the max row and col bounds for our 2D array
    while (!sr.EndOfStream)
    {
        string? line = sr.ReadLine();
        if (line != null)
        {
            if (line.Length > cols) { cols = line.Length; };
            rows++;
        }
    }
    sr.Close();

    //Read the file again to parse the file into a 2D array
    sr = new(path);
    int[,] matrix = new int[rows, cols];
    int row = 0;
    while (!sr.EndOfStream)
    {
        string? line = sr.ReadLine();
        if (line != null)
        {     
            //Iterate through each character of the row
            for (int col = 0; col < cols; col++)
            {
                //Populate the matrix
                matrix[row, col] = int.Parse(line.Substring(col, 1)); 
            }
            row++;
        }
    }

    //Trees around the perimeter of the range are all visible 
    int visibeOnEdge = (cols * 2)+((rows * 2)-4);
    int visibleInside = 0;
    int maxscenicScore = 0;

    //Loop through the rows, ignoring the first and last (on the perimeter)
    for (int r = 1; r < rows-1; r++)
    {
        //Loop through the cols, ignoring the first and last (on the perimeter)
        for (int c = 1; c < cols-1; c++)
        {
            //Source tree height
            int height = matrix[r, c];

            //Extract a 1-dimensional array of trees fanning out in each direction from the source tree
            int[] toLeft = extractArray(Direction.TO_LEFT, r, c, matrix);
            int[] toRight = extractArray(Direction.TO_RIGHT, r, c, matrix);
            int[] toTop = extractArray(Direction.TO_TOP, r, c, matrix);
            int[] toBottom = extractArray(Direction.TO_BOTTOM, r, c, matrix);

            // if ANY of the 1D arrays include any tree which is higher than the source tree height, the tree is visible
            if ((toLeft.Max() < height) || (toRight.Max() < height) || (toTop.Max() < height) || (toBottom.Max() < height))
            {
                visibleInside++;
            }

            //Calculate the scenic score of the tree for each direction
            int scoreLeft = CountSmallerTrees(toLeft, height);
            int scoreRight = CountSmallerTrees(toRight, height);
            int scoreTop = CountSmallerTrees(toTop, height);
            int scoreBottom = CountSmallerTrees(toBottom, height);

            //Calculate the total scenic score for the tree
            int scenicScore = scoreTop * scoreLeft * scoreBottom * scoreRight;

            //Is the tree's scenic score the highest so far?
            if (scenicScore > maxscenicScore)
            {
                maxscenicScore = scenicScore;
            }
        }
    }
    Console.WriteLine("Visible Trees: " + (visibleInside + visibeOnEdge));
    Console.WriteLine("Max Scenic Score: " + maxscenicScore);

}
catch (Exception ex)
{
    Console.WriteLine("ERROR: " + ex.Message);
}

int CountSmallerTrees(int[] trees, int height)
{
    /* Count the number of smaller trees between the source tree and the end of the sequence.
    * When we get to a larger (or equal size tree) we include this tree, but then stop.
    * 
    * height = 5, trees = [1, 3, 4, 5, 2, 3, 6, 7, 8]
    * count = 4
    */

    try
    {
        int count = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            count++;
            if (trees[i] >= height)
            {
                goto AfterLoop;
            }
        }

    AfterLoop:
        return count;

    }
    catch (Exception ex)
    {
        throw new Exception("Failure in Tree Counter: " + ex.Message);
    }
}


int[] extractArray(Direction dir, int row, int col, int[,] matrix)
{
    /* A function used to turn a sequence of items in a 2D array into a 1D array
     * 
     * The direction determines which cells are included:
     * 
     * TO LEFT: Row is fixed, but we take cells from the source column to left edge (col 0)
     * TO RIGHT: Row is fixed, but we take cells from the source column to the right edge (cols)
     * TO TOP: col is fixed, but we take cells from the source row to the top edge (row 0)
     * TO BOTTOM: col is fixed, but we take cells from the source row to the bottom edge (rows)
     * 
     * 3  4  5  6  7  8
     * 1  2  3  4  5  6
     * 5  6 [7] 8  0  2
     * 2  3  1  0  2  3
     * 1  3  8  3  5  7
     * 
     * TO_LEFT: [6, 5]
     * TO_RIGHT: [8, 0, 2]
     * TO_TOP: [3, 5]
     * TO_BOTTOM: [1, 8]
     * 
     * The first element in the array is always the cell adjacent to the source cell
     */

    try
    {
        int cols = matrix.GetUpperBound(1) + 1;
        int rows = matrix.GetUpperBound(0) + 1;

        int[] newArray;
        switch (dir)
        {
            case Direction.TO_LEFT:
                newArray = new int[col];
                for (int i=0; i < col; i++)
                {
                    newArray[i] = matrix[row, i];
                }
                // We need to reverse the array so the direction is from the source cell to the left-edge
                return Reverse(newArray);
            case Direction.TO_RIGHT:
                newArray = new int[cols - col-1];
                for (int i = col+1; i < cols; i++)
                {
                    newArray[i-col-1] = matrix[row, i];
                }
                return newArray;
            case Direction.TO_TOP:
                newArray = new int[row];
                for (int i = 0; i < row; i++)
                {
                    newArray[i] = matrix[i, col];
                }
                // We need to reverse the array so the direction is from the source cell to the top-edge
                return Reverse(newArray);
            default:
                newArray = new int[rows - row-1];
                for (int i = row+1; i < rows; i++)
                {
                    newArray[i-row-1] = matrix[i, col];
                }
                return newArray;
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Error in Extract Array: " + ex.Message);
    }
}

int[] Reverse(int[] array)
{
    /* Flip an array see the sequence is reversed, i.e. [1, 2, 3] becomes [3, 2, 1]
    */

    try
    {
        for (int i = 0; i < array.Length / 2; i++)
        {
            (array[array.Length - i - 1], array[i]) = (array[i], array[array.Length - i - 1]);
        }

        return array;
    }
    catch (Exception ex)
    {
        throw new Exception("Error in Array Reverse: " + ex.Message);
    }
}

public enum Direction
{
    TO_LEFT,
    TO_RIGHT,
    TO_TOP,
    TO_BOTTOM,
};