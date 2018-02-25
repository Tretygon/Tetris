using System;
using System.Collections.Generic;

namespace TetrisGameManager
{
    static internal class TetrominoGenerator
    {
        static Random rng = new Random();

        /// <summary>
        /// Generates a brand new Tetromino
        /// </summary>
        /// <returns></returns>
        internal static List<Cell> Generate()
        {
            switch ((Shapes)rng.Next(0, 7))
            {
                case Shapes.Block:
                    return new List<Cell>(){
                        new Cell(-1, -1, Shapes.Block),
                        new Cell(0, -1, Shapes.Block),
                        new Cell(-1, 0, Shapes.Block),
                        new Cell(0, 0, Shapes.Block)
                    };
                case Shapes.Г:
                    return new List<Cell>(){
                        new Cell(-1, 0, Shapes.Г),
                        new Cell(0, 0, Shapes.Г),
                        new Cell(1, 0, Shapes.Г),
                        new Cell(-1, -1, Shapes.Г)
                    };
                case Shapes.L:
                    return new List<Cell>(){
                        new Cell(-1, 0, Shapes.L),
                        new Cell(0, 0, Shapes.L),
                        new Cell(1, 0, Shapes.L),
                        new Cell(1, -1, Shapes.L)
                    };
                case Shapes.Z:
                    return new List<Cell>(){
                        new Cell(-1, 0, Shapes.Z),
                        new Cell(0, 0, Shapes.Z),
                        new Cell(0, -1, Shapes.Z),
                        new Cell(1, -1, Shapes.Z)
                    };
                case Shapes.S:
                    return new List<Cell>(){
                        new Cell(-1, -1, Shapes.S),
                        new Cell(0, -1, Shapes.S),
                        new Cell(0, 0, Shapes.S),
                        new Cell(1, 0, Shapes.S)
                    };
                case Shapes.T:
                    return new List<Cell>(){
                        new Cell(-1, 0, Shapes.T),
                        new Cell(0, 0, Shapes.T),
                        new Cell(1, 0, Shapes.T),
                        new Cell(0, -1, Shapes.T)
                    };
                case Shapes.I:
                    return new List<Cell>(){
                        new Cell(-1, 0, Shapes.I),
                        new Cell(0, 0, Shapes.I),
                        new Cell(1, 0, Shapes.I),
                        new Cell(2, 0, Shapes.I)
                    };
                default:
                    throw new Exception("(autistic screeching)");
            }
        }
    }
}
