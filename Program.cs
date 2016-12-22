using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Element
    {
        public int x;
        public int y;
        public Element()
        {
            ;
        }
        public Element(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void Swap(int a = 0)
        {
            this.x += this.y;
            this.y = this.x - this.y;
            this.x -= this.y;
        }
        public Element Swap()
        {
            this.Swap(0);
            return this;
        }
    }
    public class Figure
    {
        public byte Size;
        public byte Type;
        public List<Element> Dots;

        public Figure(byte type)
        {
            this.Type = type;
            CreateFigure();
        }

        public void CreateFigure(byte type)
        {
            this.Type = type;
            this.CreateFigure();
        }
        public void CreateFigure()
        {
            this.Dots = new List<Element>();
            switch (this.Type)
            {
                case 1:
                    this.Dots.Add(new Element(0, 0));
                    this.Dots.Add(new Element(0, 1));
                    this.Dots.Add(new Element(1, 1));
                    this.Dots.Add(new Element(2, 1));
                    this.Size = 3;
                    break;
                case 2:
                    this.Dots.Add(new Element(0, 0));
                    this.Dots.Add(new Element(0, 1));
                    this.Dots.Add(new Element(1, 0));
                    this.Dots.Add(new Element(1, 1));
                    this.Size = 2;
                    break;
                case 3:
                    this.Dots.Add(new Element(0, 1));
                    this.Dots.Add(new Element(1, 0));
                    this.Dots.Add(new Element(1, 1));
                    this.Dots.Add(new Element(2, 1));
                    this.Size = 3;
                    break;
                case 4:
                    this.Dots.Add(new Element(0, 0));
                    this.Dots.Add(new Element(0, 1));
                    this.Dots.Add(new Element(1, 1));
                    this.Dots.Add(new Element(1, 2));
                    this.Size = 3;
                    break;
                case 5:
                    this.Dots.Add(new Element(0, 0));
                    this.Dots.Add(new Element(1, 0));
                    this.Dots.Add(new Element(2, 0));
                    this.Dots.Add(new Element(3, 0));
                    this.Size = 4;
                    break;
                default:
                    this.Dots.Add(new Element(0, 0));
                    this.Size = 1;
                    break;
            }
        }

        public void Rotate()
        {
            bool[,] RotateMass = new bool[this.Size, this.Size];
            bool[,] ResultMass = new bool[this.Size, this.Size];
            for (int i = 0; i < this.Size; ++i)
            {
                for (int j = 0; j < this.Size; ++j)
                {
                    RotateMass[i, j] = this.Dots.Where(z => z.x == j && z.y == i).Count() != 0;
                }
            }

            for (int i = 0; i < this.Size; ++i)
            {
                for (int j = 0; j < this.Size; ++j)
                {
                    ResultMass[i, this.Size - j - 1] = RotateMass[j, i];
                }
            }

            this.Dots.Clear();
            for (int i = 0; i < this.Size; ++i)
            {
                for (int j = 0; j < this.Size; ++j)
                {
                    if (ResultMass[i, j])
                    {
                        this.Dots.Add(new Element(j, i));
                    }
                }
            }
            //this.Dots = this.Dots.Select(z => z.Rotate()).ToList();
        }
    }
    public class GameField
    {
        public int TopOffset;
        public int LeftOffset;
        public bool CanPaint;

        public int DotOffsetFromLeft_;
        public int DotOffsetFromLeft
        {
            get
            {
                return this.DotOffsetFromLeft_;
            }
            set
            {
                if ((value < this.Rules.FieldWidth && (value) >= 0) && (this.Elements.Where(z => z.x == value && z.y == this.DotOffsetFromTop).Count() == 0))
                {
                    this.DotOffsetFromLeft_ = value;
                }
                else
                {
                    if (value < this.Rules.FieldWidth && (value) >= 0)
                    {
                        if (this.Elements.Where(z => z.y == this.DotOffsetFromTop && z.x == this.DotOffsetFromLeft).Count() == 0)
                        {
                            this.Elements.Add(new Element(this.DotOffsetFromLeft, this.DotOffsetFromTop));
                        }
                        DeleteCompletedLines();
                        this.NewElementCreate();
                    }
                }
            }
        }

        public int DotOffsetFromTop_;
        public int DotOffsetFromTop
        {
            get
            {
                return this.DotOffsetFromTop_;
            }
            set
            {
                try
                {
                    if ((this.CreatedFigure.Dots.Select(z => (z.y + value) < this.Rules.FieldHeight).Where(z => !z).Count() == 0) && this.Elements.Where(z => (z.x == this.DotOffsetFromLeft) && (z.y == (this.DotOffsetFromTop + 1))).Count() == 0)
                    {
                        this.DotOffsetFromTop_ = value;
                    }
                    else
                    {
                        foreach (var Dot in this.CreatedFigure.Dots)
                        {
                            if (this.Elements.Where(z => z.x == Dot.x + this.DotOffsetFromLeft && z.y == Dot.y + this.DotOffsetFromTop).Count() == 0)
                            {
                                this.Elements.Add(new Element(Dot.x + this.DotOffsetFromLeft, Dot.y + this.DotOffsetFromTop));
                                DeleteCompletedLines();
                            }
                        }
                        this.NewElementCreate();
                    }
                }
                catch
                {
                    ;
                }
            }
        }
        public List<Element> Elements;
        private string FieldBorders_;
        public string FieldBorders
        {
            get
            {
                return FieldBorders_;
            }
            set
            {
                UpdateBorders(value);
                FieldBorders_ = value;
            }
        }
        private string TopPartOfField;
        private string BottomPartOfField;

        public Figure CreatedFigure;

        public GameRules Rules;
        public GameField(int Top, int Left, string Borders, GameRules Rules) // /-\||\-/ *
        {
            this.CreatedFigure = new Figure(1);
            this.CanPaint = true;
            this.Elements = new List<Element>();
            this.TopOffset = Top;
            this.LeftOffset = Left;
            this.Rules = Rules;
            this.NewElementCreate();
            this.Rules.GlobalTimer.Elapsed += this.NeedRepaint;
            this.FieldBorders = Borders;
            this.OutGameField();
        }
        public void DeleteCompletedLines()
        {
            for (var i = this.Rules.FieldHeight; i >= 0; --i)
            {
                if (this.Elements.Where(z => z.y == i).Distinct().Count() == this.Rules.FieldWidth)
                {
                    this.Elements = this.Elements.Where(z => z.y != i).ToList();
                    var BufferElements = this.Elements.Where(z => z.y > i).ToList();
                    this.Elements = this.Elements.Where(z => z.y < i).Select(z => new Element(z.x, z.y + 1)).ToList();
                    this.Elements.AddRange(BufferElements);
                }
            }
        }
        private void UpdateBorders(string Borders)
        {
            this.TopPartOfField = Borders[0] + "";
            for (int i = 0; i < this.Rules.FieldWidth; ++i)
            {
                this.TopPartOfField += Borders[1];
            }
            this.TopPartOfField += Borders[2];

            this.BottomPartOfField = Borders[5] + "";
            for (int i = 0; i < this.Rules.FieldWidth; ++i)
            {
                this.BottomPartOfField += Borders[6];
            }
            this.BottomPartOfField += Borders[7];
        }
        public void NewElementCreate()
        {
            this.DotOffsetFromLeft_ = (this.Rules.FieldWidth / 2);
            this.DotOffsetFromTop_ = 0;
        }
        public bool CheckElements()
        {
            bool Control;
            Control = this.Elements.Where(z => (z.x == this.DotOffsetFromLeft) && (z.y == (this.DotOffsetFromTop + 1))).Count() == 0;
            Control = Control && (this.DotOffsetFromTop + 1) < this.Rules.FieldHeight;
            //Control = Control && ((this.DotOffsetFromLeft) < this.Rules.FieldWidth && (this.DotOffsetFromLeft) >= 0);
            return Control;
        }
        public void NeedRepaint(object sender, System.EventArgs e)
        {
            this.CanPaint = false;
            ++DotOffsetFromTop;
            this.OutGameField();
        }
        public void OutGameField()
        {
            this.CanPaint = false;

            Console.CursorTop = this.TopOffset;
            //Console.SetCursorPosition(this.LeftOffset, this.TopOffset);

            string LeftSpace;

            LeftSpace = "";
            for (int Offs = 0; Offs < this.LeftOffset; ++Offs)
            {
                LeftSpace += ' ';
            }
            Console.WriteLine(LeftSpace + this.TopPartOfField);

            string GameFieldPart;

            for (int y = 0; y < this.Rules.FieldHeight; ++y)
            {
                GameFieldPart = "" + this.FieldBorders[3];
                for (int x = 0; x < this.Rules.FieldWidth; ++x)
                {
                    if (CreatedFigure.Dots.Where(z => z.x + this.DotOffsetFromLeft == x && z.y + this.DotOffsetFromTop == y).Count() != 0)
                    {
                        GameFieldPart += this.FieldBorders[8];
                    }
                    else
                    {
                        if (this.Elements.Where(z => z.x == x && z.y == y).Count() != 0)
                        {
                            GameFieldPart += this.FieldBorders[8];
                        }
                        else
                        {
                            GameFieldPart += this.FieldBorders[9];
                        }
                    }
                }
                GameFieldPart += this.FieldBorders[4];

                Console.WriteLine(LeftSpace + GameFieldPart);
            }

            //Console.CursorLeft = this.LeftOffset;

            Console.WriteLine(LeftSpace + this.BottomPartOfField);
            Console.WriteLine("x - {0}   y - {1}   e - {2}", DotOffsetFromLeft, DotOffsetFromTop, this.Elements.Count());

            this.CanPaint = true;
        }
    }
    public class GameRules
    {
        public int FieldWidth;
        public int FieldHeight;
        public System.Timers.Timer GlobalTimer;
        public GameRules(int TickTime, int Width, int Height)
        {
            this.FieldWidth = Width;
            this.FieldHeight = Height;
            this.GlobalTimer = new System.Timers.Timer(TickTime);
            //this.GlobalTimer.Elapsed += this.GlobalTimerElapse;
            this.GlobalTimer.Start();
        }
        public void GlobalTimerElapse(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Elapse");
        }
    }
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press any key for start");
            ConsoleKeyInfo ConsoleKey = Console.ReadKey();

            Console.Clear();
            byte o = 1;

            var a = new GameField(10, 10, @"┌─┐││└─┘▒░", new GameRules(400, 10, 10));

            for (;;)
            {
                if (a.CanPaint)
                {
                    ConsoleKey = Console.ReadKey(true);
                    switch (ConsoleKey.Key)
                    {
                        case System.ConsoleKey.R:
                            Console.Clear();
                            a.CreatedFigure.CreateFigure(o);
                            ++o;
                            break;
                        case System.ConsoleKey.A:
                        case System.ConsoleKey.LeftArrow:
                            --a.DotOffsetFromLeft;
                            break;
                        case System.ConsoleKey.D:
                        case System.ConsoleKey.RightArrow:
                            ++a.DotOffsetFromLeft;
                            break;
                        case System.ConsoleKey.W:
                        case System.ConsoleKey.UpArrow:
                            a.CreatedFigure.Rotate();
                            break;
                        case System.ConsoleKey.H:
                            a.Rules.GlobalTimer.Stop();
                            HelpDialog();
                            a.Rules.GlobalTimer.Start();
                            break;
                        case System.ConsoleKey.N:
                            a.Rules.GlobalTimer.Stop();
                            a = NewGame();
                            Console.Clear();
                            break;
                        case System.ConsoleKey.P:
                            a.Rules.GlobalTimer.Stop();
                            StopDialog();
                            a.Rules.GlobalTimer.Start();
                            break;
                        default:
                            ++a.DotOffsetFromTop;
                            a.Rules.GlobalTimer.Stop();
                            a.Rules.GlobalTimer.Start();
                            break;
                    }
                    if (a.CanPaint)
                    {
                        a.OutGameField();
                    }
                }
            }
        }
        static GameField NewGame()
        {
            Console.Clear();

            Console.WriteLine("Input offset: \ny: ");
            int xo;
            int yo;
            int.TryParse(Console.ReadLine(), out xo);
            Console.WriteLine("x: ");
            int.TryParse(Console.ReadLine(), out yo);

            Console.WriteLine("Input TickTime(ms), width, height");
            int tt;
            int w;
            int h;
            int.TryParse(Console.ReadLine(), out tt);
            int.TryParse(Console.ReadLine(), out w);
            int.TryParse(Console.ReadLine(), out h);

            Console.WriteLine("Input draw pars");
            return new GameField(xo, yo, Console.ReadLine(), new GameRules(tt, w, h));
        }
        static void HelpDialog()
        {
            Console.Clear();
            Console.WriteLine("HelpDialog");
            Console.ReadKey();
        }
        static void StopDialog()
        {
            Console.WriteLine("Game Paused");
            Console.ReadKey();
        }
    }
}
