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
                    if ((value < this.Rules.FieldHeight) && this.Elements.Where(z => (z.x == this.DotOffsetFromLeft) && (z.y == (this.DotOffsetFromTop + 1))).Count() == 0)
                    {
                        this.DotOffsetFromTop_ = value;
                    }
                    else
                    {
                        if (this.Elements.Where(z => z.x == this.DotOffsetFromLeft && z.y == this.DotOffsetFromTop).Count() == 0)
                        {
                            this.Elements.Add(new Element(this.DotOffsetFromLeft, this.DotOffsetFromTop));
                            DeleteCompletedLines();
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

        public GameRules Rules;
        public GameField(int Top, int Left, string Borders, GameRules Rules) // /-\||\-/
        {
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
                    if ((DotOffsetFromTop == y && DotOffsetFromLeft == x) || this.Elements.Where(z => z.x == x && z.y == y).Count() != 0)
                    {
                        GameFieldPart += this.FieldBorders[8];
                    }
                    else
                    {
                        GameFieldPart += this.FieldBorders[9];
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
                            //a.OutGameField();
                            break;
                        case System.ConsoleKey.A:
                        case System.ConsoleKey.LeftArrow:
                            --a.DotOffsetFromLeft;
                            break;
                        case System.ConsoleKey.D:
                        case System.ConsoleKey.RightArrow:
                            ++a.DotOffsetFromLeft;
                            break;
                        case System.ConsoleKey.H:
                            a.Rules.GlobalTimer.Stop();
                            HelpDialog();
                            a.Rules.GlobalTimer.Start();
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
