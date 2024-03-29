﻿using System;
using System.Linq;

namespace Skyscrapers
{
    /// <summary>
    /// The board class.
    /// </summary>
    class Board
    {
        // Constants variables
        public const int HintsAmount = 3;
        public const int BoardSize = 4;
        // Example Board
        private int[,] ExampleBoard;

        // Class attributes
        private int[,] SolvedBoard { get; }
        private int[,] Edges { get; }
        private int[,] ResetBoard { get; }
        private int[,] SolvingBoard { get; set; }
        private int CountHints { get; set; }

        public Board(int size)
        { 
            ExampleBoard = new int[,] { { 1, 4, 3, 2 }, 
                                        { 2, 1, 4, 3 }, 
                                        { 4, 3, 2, 1 }, 
                                        { 3, 2, 1, 4 } };
            SolvedBoard = GenerateBoard(size);
            Edges = CreateEdges(size);
            ResetBoard = new int[size,size];
            SolvingBoard = new int[size, size];
            CountHints = HintsAmount;
        }

        static int[,] GenerateBoard(int size)
        {
            Random rnd = new Random();
            int[,] board = new int[size, size];
            int[] digits = new int[size];
            for (int i = 1; i <= size; i++)
            {
                digits[i - 1] = i;
            }
            int[] shuffeld_row = new int[size];
            for (int i = 0; i < size; i++)
            {
                while (!CheckRow(board, shuffeld_row))
                {   // while row is in board, shuffle again!
                    shuffeld_row = digits.OrderBy(item => rnd.Next()).ToArray();
                }
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = shuffeld_row[j];
                }
            }
            return board;
        }

        static bool CheckRow(int[,] built_board, int[] seq)
        { 
            for (int j = 0; j < built_board.GetLength(0); j++)
            {
                for (int i = 0; i < seq.Length; i++)
                {
                    if (built_board[j,i] == seq[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int[,] CreateEdges(int size)
        { //Create edges using the board inside.
            int[,] edges = new int[4, size];

            for (int side = 0; side < 4; side++)
            {
                for (int i = 0; i < size; i++)
                {
                    edges[side, i] = CountBuildings(side,i,SolvedBoard);
                }
            }

            return edges;
        }

        private int CountBuildings(int side, int i, int[,] board)
        { // Check how many buildings are in each row/colmn
            int height = 0;
            int cur_highest = 0;
            switch (side)
            {
                case 0:
                    for (int j = 0; j < board.GetLength(0); j++)
                    {
                        if (board[i, j] > cur_highest)
                        {
                            height++;
                            cur_highest = board[i, j];
                        }
                        if (cur_highest == board.GetLength(0))
                        {
                            break;
                        }
                    }
                    return height;
                case 1:
                    for (int j = board.GetLength(0) - 1; j >= 0; j--)
                    {
                        if (board[i, j] > cur_highest)
                        {
                            height++;
                            cur_highest = board[i, j];
                        }
                        if (cur_highest == board.GetLength(0))
                        {
                            break;
                        }
                    }
                    return height;
                case 2:
                    for (int j = 0; j < board.GetLength(0); j++)
                    {
                        if (board[j, i] > cur_highest)
                        {
                            height++;
                            cur_highest = board[j, i];
                        }
                        if (cur_highest == board.GetLength(0))
                        {
                            break;
                        }
                    }
                    return height;
                case 3:
                    for (int j = board.GetLength(0) - 1; j >= 0; j--)
                    {
                        if (board[j, i] > cur_highest)
                        {
                            height++;
                            cur_highest = board[j, i];
                        }
                        if (cur_highest == board.GetLength(0))
                        {
                            break;
                        }
                    }
                    return height;
                default:
                    return height;
            }
        }


        //Gets methods:
        public int[,] GetResetBoard()
        {
            return SolvingBoard;
        }

        public int[,] GetEdges()
        {
            return Edges;
        }

        public int[,] GetSolvingdBoard()
        {
            return SolvingBoard;
        }
        public int[,] GetSolveddBoard()
        {
            return SolvedBoard;
        }

        public int GetCountHints()
        {
            return CountHints;
        }

        public (int,int) GetHint()
        { //function that gives a hint.
            // Creates a Random object
            Random rnd = new Random();
            // Generates the first random row and coloumn of the cell
            int row = rnd.Next(0, SolvingBoard.GetLength(0));
            int col = rnd.Next(0, SolvingBoard.GetLength(1));
            // checks if needs to grill again the row and the coloumn and does it:
            while (SolvingBoard[row, col] == SolvedBoard[row, col])
            {
                row = rnd.Next(0, SolvingBoard.GetLength(0));
                col = rnd.Next(0, SolvingBoard.GetLength(1));
            }
            // takes the value of the cell from the solved board 
            // and copy it to the currently solving board
            SolvingBoard[row, col] = SolvedBoard[row, col];
            ResetBoard[row, col] = SolvingBoard[row, col];
            CountHints -= 1;
            return (row, col);
        }

        public string UpdateHintButtonText()
        {
            return string.Format("Hint ({0})", GetCountHints().ToString());
        }

        public void UpdateSolvingBoard(string cell_name, string cell_text)
        { // update in the logic what the screen shows.
            int row = cell_name[4]-48;
            int col = cell_name[6]-48;
            if (cell_text == "")
            {
                SolvingBoard[row, col] = 0;
            }
            else
            {
                int val = cell_text[0] - 48;
                SolvingBoard[row, col] = val;
            }
        }

        public void BackToResetBoard()
        { //Returning to reset board
            for (int row = 0; row < ResetBoard.GetLength(0); row++)
            {
                for (int col = 0; col < ResetBoard.GetLength(1); col++)
                {
                    SolvingBoard[row, col] = ResetBoard[row, col];
                }
            }

        }

        private bool AllCellsFilled()
        { // Check if all the cells are filled.
            for (int row = 0; row < ResetBoard.GetLength(0); row++)
            {
                for (int col = 0; col < ResetBoard.GetLength(1); col++)
                {
                    if (SolvingBoard[row, col] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool EdgesSatisfied()
        { //check if the solving fits the edges.
            for (int side = 0; side < 4; side++)
            {
                for (int i = 0; i < SolvingBoard.GetLength(0); i++)
                {
                    if (Edges[side, i] != CountBuildings(side, i, SolvingBoard))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool DigitNoRepeat()
        { // Check if theere are no repeats in each row and colmn
            for (int cur_row = 0; cur_row < ResetBoard.GetLength(0); cur_row++)
            {
                for (int row = 0; row < ResetBoard.GetLength(0); row++)
                {
                    if (row == cur_row)
                        break;
                    else
                    {
                        for (int col = 0; col < ResetBoard.GetLength(1); col++)
                        {
                            if (SolvingBoard[cur_row, col] == SolvingBoard[row, col])
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool CheckSolution()
        { //"IsSaffe : check if all the functions are true
            if (!AllCellsFilled())
                return false;

            if (!EdgesSatisfied())
                return false;

            if (!DigitNoRepeat())
                return false;

            return true;
        }

        public bool SolveSkyScrapers(int index)
        {
            if (index == SolvingBoard.Length)
            {
                return true;
            }
            int row = index / SolvingBoard.GetLength(0);
            int col = index % SolvingBoard.GetLength(1);
            if (ResetBoard[row,col] != 0)
            {
                SolvingBoard[row, col] = ResetBoard[row, col];
                return SolveSkyScrapers(index + 1);
            }
            for (int value = 1; value <= SolvingBoard.GetLength(0); value++)
            {
                if (ValidValue(value, row, col))
                {
                    SolvingBoard[row, col] = value;
                    if (SolveSkyScrapers(index + 1))
                    {
                        return true;
                    }
                }
                SolvingBoard[row, col] = 0;
            }
            return false;
        }

        private bool ValidValue(int value, int row, int col)
        {
            if (!IsPossible(value, row, col))
                return false;
            int[] seq = new int[SolvingBoard.GetLength(0)];

            if (!CheckAvailableSeqRow(seq, 0, row, 0))
                return false;

            seq = new int[SolvingBoard.GetLength(1)];
            if (!CheckAvailableSeqCol(seq, 0, 0, col))
                return false;

            return true;
        }

        private bool CheckAvailableSeqRow(int[] seq, int specific_index, int row, int col)
        {
            if (specific_index == SolvingBoard.GetLength(0))
            {
                if (CheckEdges(0, seq, row))
                    return true;
                return false;
            }

            if (SolvingBoard[row, col] != 0)
            {
                seq[specific_index] = SolvingBoard[row, col];
                if (CheckAvailableSeqRow(seq, specific_index + 1, row, col +1))
                    return true;
                seq[specific_index] = 0;
            }
            for (int value = 1; value <= seq.Length; value++)
            {
                if (!IsPossible(value,row,col) || !NoRepeatInseq(value,seq,specific_index))
                    continue;
                seq[specific_index] = value;
                if (CheckAvailableSeqRow(seq, specific_index + 1, row, col + 1))
                    return true;
                seq[specific_index] = 0;
            }
            return false;
        }

        private bool IsPossible(int value, int row, int col)
        {
            for (int i = 0; i < SolvingBoard.GetLength(0); i++)
            {
                if (i != col)
                    if (SolvingBoard[row, i] == value)
                        return false;
                if (i != row)
                    if (SolvingBoard[i, col] == value)
                        return false;
            }
            return true;
        }

        private bool NoRepeatInseq(int value, int[] seq, int specific_index)
        {
            for (int i = 0; i < SolvingBoard.GetLength(0); i++)
            {
                if (i != specific_index)
                    if (seq[i] == value)
                        return false;
            }
            return true;
        }

        private bool CheckAvailableSeqCol(int[] seq, int specific_index, int row, int col)
        {
            if (specific_index == SolvingBoard.GetLength(0))
            {
                if (CheckEdges(2, seq, col))
                    return true;
                return false;
            }

            if (SolvingBoard[row, col] != 0)
            {
                seq[specific_index] = SolvingBoard[row, col];
                if (CheckAvailableSeqCol(seq, specific_index + 1, row + 1, col))
                    return true;
                seq[specific_index] = 0;
            }
            for (int value = 1; value <= seq.Length; value++)
            {
                if (!IsPossible(value, row, col) || !NoRepeatInseq(value, seq, specific_index))
                    continue;
                seq[specific_index] = value;
                if (CheckAvailableSeqCol(seq, specific_index + 1, row+ 1, col))
                    return true;
                seq[specific_index] = 0;
            }
            return false;
        }

        private bool CheckEdges(int direction, int[] seq, int seq_number)
        {
            int count_left = 1;
            int count_right = 1;
            int max_left = seq[0];
            int max_right = seq[seq.Length - 1];
            for (int i=1; i<seq.Length; i++)
            {
                if (seq[i] > max_left)
                {
                    max_left = seq[i];
                    count_left++;
                }
            }
            for (int i = seq.Length-2; i >= 0; i--)
            {
                if (seq[i] > max_right)
                {
                    max_right = seq[i];
                    count_right++;
                }
            }
            if (count_left == Edges[direction, seq_number] && count_right == Edges[direction + 1, seq_number])
                return true;
            return false;




        }
    }
}


