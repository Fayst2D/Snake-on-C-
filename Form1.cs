using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private int[,] field;
        private int snakeSize = 40;
        private Graphics graphic;
        private bool IsFirstStart;
        private Point[] elements;
        private Point direction;
        private Random rand;


        enum MyEnum
        {
            Empty,
            Snake,
            Food
        }

        public Form1()
        {
            InitializeComponent();
            field = new int[Width / snakeSize, Height / snakeSize];

            pictureBox1.Image = new Bitmap(Width, Height);
            graphic = Graphics.FromImage(pictureBox1.Image);
            rand = new Random();
        }

        private void Start()
        {
            for (int x = 0; x < Width / snakeSize; x++)
            {
                for (int y = 0; y < Height / snakeSize; y++)
                {
                    field[x, y] = (int) MyEnum.Empty;
                }
            }

            elements = new Point[4];
            elements[0] = new Point(Width / snakeSize / 2, Height / snakeSize / 2);
            field[elements[0].X, elements[0].Y] = (int) MyEnum.Snake;


            for (int i = 1; i < elements.Length; i++)
            {
                elements[i] = new Point(elements[0].X - (i*direction.X), elements[0].Y - (i * direction.Y));
                field[elements[0].X - (i*direction.X), elements[0].Y - (i*direction.Y)] = (int)MyEnum.Snake;
            }

            bool k = true;
            while (k)
            {
                int x = rand.Next(Width / snakeSize - 1);
                int y = rand.Next(Height / snakeSize - 1);
                if (field[x,y] == (int) MyEnum.Empty)
                {
                    k = false;
                    field[x, y] = (int)MyEnum.Food;

                }
            }
            

            for (int x = 0; x < Width / snakeSize; x++)
            {
                for (int y = 0; y < Height / snakeSize; y++)
                {
                    if (field[x, y] == (int) MyEnum.Empty)
                    {
                        graphic.FillRectangle(Brushes.Black, x * snakeSize, y * snakeSize, snakeSize, snakeSize);
                    }

                    if (field[x, y] == (int) MyEnum.Snake)
                    {
                        graphic.FillRectangle(Brushes.Crimson, x * snakeSize, y * snakeSize, snakeSize, snakeSize);
                    }

                    if (field[x, y] == (int) MyEnum.Food)
                    {
                        graphic.FillRectangle(Brushes.GreenYellow, x * snakeSize, y * snakeSize, snakeSize, snakeSize);
                    }
                }
            }

            pictureBox1.Refresh();
            IsFirstStart = false;
        }

        private void Stop()
        {
            IsFirstStart = false;
            timer1.Stop();
        }
        private void IsOutFromField()
        {
            for (int i = 0; i < elements.Length - 1; i++)
            {
                if (elements[i].X >= Width / snakeSize || elements[i].X < 0)
                {
                    if (elements[i].X >= Width / snakeSize)
                    {
                        elements[i].X = Width / snakeSize - elements[i].X;
                    }
                    else
                    {
                        elements[i].X = Width / snakeSize - elements[i].X-2;
                    }
                }

                if (elements[i].Y >= Height / snakeSize || elements[i].Y < 0)
                {
                    if (elements[i].Y >= Height / snakeSize)
                    {
                        elements[i].Y = Height / snakeSize - elements[i].Y;
                    }
                    else
                    {
                        elements[i].Y = Height / snakeSize - elements[i].Y - 2;
                    }
                }
            }
        }
        private void Step()
        {
            graphic.FillRectangle(Brushes.Black, elements[elements.Length - 1].X * snakeSize,
                elements[elements.Length - 1].Y * snakeSize, snakeSize, snakeSize);
            field[elements[elements.Length - 1].X, elements[elements.Length - 1].Y] = (int)MyEnum.Empty;


            Point[] newElements = new Point[elements.Length];

            newElements[0] = new Point(elements[0].X + direction.X, elements[0].Y + direction.Y);

            for (int i = 1; i < elements.Length; i++)
            {
                newElements[i] = elements[i-1];
            }

            elements = newElements;
            IsOutFromField();

            if (field[newElements[0].X, newElements[0].Y] == (int)MyEnum.Snake)
            {
                Stop();
            }

            if (field[newElements[0].X, newElements[0].Y] == (int)MyEnum.Food)
            {
                Eat();
            }

            for (int i = 0; i < newElements.Length - 1; i++)
            {
                field[newElements[i].X, newElements[i].Y] = (int)MyEnum.Snake;
            }

            graphic.FillRectangle(Brushes.Crimson, elements[0].X * snakeSize, elements[0].Y * snakeSize, snakeSize,
                snakeSize);
            
            pictureBox1.Refresh();
        }

        private void Eat()
        {
            Point[] newElements = new Point[elements.Length+1];
            newElements[newElements.Length - 1] = elements[elements.Length - 1];
            for (int i = 0; i < newElements.Length-1; i++)
            {
                newElements[i] = elements[i];
            }
            elements = newElements;

            int tx = 0, ty = 0;
            bool k = true;
            while (k)
            {
                int x = rand.Next(Width / snakeSize - 1);
                int y = rand.Next(Height / snakeSize - 1);
                if (field[x, y] == (int)MyEnum.Empty)
                {
                    k = false;
                    field[x, y] = (int)MyEnum.Food;
                    tx = x;
                    ty = y;
                }
            }
            graphic.FillRectangle(Brushes.GreenYellow, tx * snakeSize, ty * snakeSize, snakeSize, snakeSize);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsFirstStart)
            {
                Start();
            }
            else
            {
                Step();
            }
        }


        private bool IsReverseKey(Point old,Point dir)
        {
            return old.X * -1 == dir.X || old.Y * -1 == dir.Y;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Point olddirection = new Point(10,10);


            if (!timer1.Enabled)
            {
                timer1.Start();
                IsFirstStart = true;
            }
            else
            {
                olddirection = direction;
            }

            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left: direction.X = -1; direction.Y = 0;break;
                case Keys.W:
                case Keys.Up: direction.X = 0; direction.Y = -1; break;
                case Keys.S:
                case Keys.Down: direction.X = 0; direction.Y = 1; break;
                case Keys.D:
                case Keys.Right: direction.X = 1; direction.Y = 0; break;
            }

            if (IsReverseKey(olddirection, direction))
            {
                direction = olddirection;
            }
        }
    }
}
