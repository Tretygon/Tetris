using System.Collections.Generic;
using System.Windows.Media;
using TetrisGameManager;

namespace Tetris
{
    /// <summary>
    /// provides set of brushes
    /// </summary>
    public class Pallete
    {
        public enum Scheme { Bright, Solid , Contrast, Dim, Ghost};

        public readonly Scheme CurrentScheme;
        private readonly Dictionary<Shapes, Brush> ShapeColoring;
        public readonly Brush GridLinesBrush;
        public readonly Brush EmptyCellBrush;

        public Pallete(Scheme scheme)
        {
            CurrentScheme = scheme;
            switch (scheme)
            {
                case Scheme.Bright:
                    ShapeColoring = new Dictionary<Shapes, Brush>()
                    {
                        {Shapes.Block, Brushes.Yellow},  //brown?
                        { Shapes.I, Brushes.Aqua},
                        { Shapes.L, Brushes.Magenta},
                        { Shapes.S, Brushes.Red},
                        { Shapes.T, Brushes.DarkOrange},
                        { Shapes.Z, Brushes.Lime},
                        { Shapes.Г, new SolidColorBrush(Color.FromArgb(255,00,97,255))}, //blue
                        { Shapes.Preview, Brushes.LightGray}
                    };
                    EmptyCellBrush = Brushes.Snow;
                    GridLinesBrush = Brushes.Gainsboro;
                    break;
                case Scheme.Solid:
                    ShapeColoring = new Dictionary<Shapes, Brush>()
                    {
                        {Shapes.Block, Brushes.Gold},
                        { Shapes.I, Brushes.SaddleBrown},
                        { Shapes.L, Brushes.DarkViolet},
                        { Shapes.S, new SolidColorBrush(Color.FromArgb(255,220,00,00))}, //darker red
                        { Shapes.T, Brushes.DarkOrange},
                        { Shapes.Z, new SolidColorBrush(Color.FromArgb(255, 30,195,30))}, //green
                        { Shapes.Г, Brushes.Blue},
                        { Shapes.Preview, Brushes.Silver}
                    };
                    EmptyCellBrush = Brushes.WhiteSmoke;
                    GridLinesBrush = Brushes.Gainsboro;
                    break;
                case Scheme.Contrast:
                    ShapeColoring = new Dictionary<Shapes, Brush>()
                    {
                        {Shapes.Block, Brushes.Yellow},  //brown?
                        { Shapes.I, Brushes.Aqua},
                        { Shapes.L, Brushes.Magenta},
                        { Shapes.S, Brushes.Red},
                        { Shapes.T, Brushes.DarkOrange},
                        { Shapes.Z, Brushes.Lime},
                        { Shapes.Г, new SolidColorBrush(Color.FromArgb(255,00,97,255))}, //blue
                        { Shapes.Preview, Brushes.WhiteSmoke}
                    };
                    EmptyCellBrush = Brushes.Black;
                    GridLinesBrush = Brushes.Gray;
                    break;
                case Scheme.Dim:
                    ShapeColoring = new Dictionary<Shapes, Brush>()
                    {
                        {Shapes.Block, Brushes.Gold}, 
                        { Shapes.I, Brushes.SaddleBrown},
                        { Shapes.L, Brushes.DarkViolet},
                        { Shapes.S, Brushes.DarkRed}, //darker red
                        { Shapes.T, Brushes.DarkOrange},
                        { Shapes.Z, Brushes.Green}, //green
                        { Shapes.Г, Brushes.BlueViolet},
                        { Shapes.Preview, Brushes.DarkSlateGray}
                    };
                    EmptyCellBrush = Brushes.Gainsboro;
                    GridLinesBrush = Brushes.LightGray;
                    break;
                case Scheme.Ghost:
                    ShapeColoring = new Dictionary<Shapes, Brush>()
                    {
                        {Shapes.Block, Brushes.MediumTurquoise},
                        { Shapes.I, Brushes.LightCyan},
                        { Shapes.L, Brushes.PeachPuff},
                        { Shapes.S, Brushes.PaleVioletRed},
                        { Shapes.T, Brushes.LightGreen},
                        { Shapes.Z, Brushes.Plum},
                        { Shapes.Г, Brushes.LightBlue},
                        { Shapes.Preview, Brushes.CadetBlue}
                    };
                    EmptyCellBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB0D4B0")); //vomit green;
                    GridLinesBrush = Brushes.DarkSeaGreen;
                    break;
            }
        }
        public Brush this [Shapes shape]
        {
            get => ShapeColoring[shape];
        }
    }
}

