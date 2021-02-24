using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polygons
{

    public partial class Form1 : Form
    {
        bool go_1 = false;
        bool go_2 = false;
        static Random rand = new Random();
        int random_number;
        int figure;
        bool Draw;
        List<My_Shape> shapes = new List<My_Shape>();
        Color color_f;

        public Form1()
        {
            InitializeComponent();
            Draw = false;
            DoubleBuffered = true;
            color_f = Color.HotPink;
        }

        private bool CheckMoving(int x_m, int y_m, List<My_Shape> shapes)
        {
            if (shapes.Count() > 2)
            {
                double delta_test;
                bool Up_test = false;
                bool Down_test = false;
                List<My_Shape> Test_polygon = new List<My_Shape>();
                My_Shape Test_point = new Circle(x_m, y_m);
                shapes.Add(Test_point);
                Test_polygon.Clear();
                for (int i = 0; i < shapes.Count() - 1; i++)
                {
                    for (int j = i + 1; j < shapes.Count(); j++)
                    {
                        for (int k = 0; k < shapes.Count(); k++)
                        {
                            if (k != j && k != i)
                            {
                                delta_test = ((shapes[k].SetY - shapes[j].SetY) * (shapes[i].SetX - shapes[j].SetX)) -
                                         ((shapes[i].SetY - shapes[j].SetY) * (shapes[k].SetX - shapes[j].SetX));

                                if (delta_test >= 0) Up_test = true;
                                else Down_test = true;
                            }
                        }
                        if ((Up_test && Down_test) == false)
                        {
                            Test_polygon.Add(shapes[j]);
                            Test_polygon.Add(shapes[i]);
                        }
                        Up_test = false;
                        Down_test = false;
                    }
                }
                if (Test_polygon.Contains(Test_point) == false)
                {
                    shapes.Remove(Test_point);
                    return true;
                }
                shapes.Remove(Test_point);
            }
            return false;
        }

        

        private void CircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 1;
        }

        private void SquareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 2;
        }

        private void TriangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 3;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool Up = false;
            bool Down = false;
            double delta;
            if (Draw)
            {
                Graphics g = e.Graphics;
                foreach (My_Shape polyg in shapes)
                {
                    polyg.Inside = false;
                    polyg.Draw(g);
                    My_Shape.colo = color_f;
                }
                Pen brush = new Pen(Color.Black);
                if (shapes.Count() > 2)
                {
                    for (int i = 0; i < shapes.Count() - 1; i++)
                    {
                        for (int j = i + 1; j < shapes.Count(); j++)
                        {
                            for (int k = 0; k < shapes.Count(); k++)
                            {
                                if (k != i && k != j)
                                {
                                    delta = ((shapes[k].SetY - shapes[j].SetY) * (shapes[i].SetX - shapes[j].SetX)) -
                                        ((shapes[i].SetY - shapes[j].SetY) * (shapes[k].SetX - shapes[j].SetX));

                                    if (delta >= 0) Up = true;
                                    else Down = true;
                                }
                            }
                            if ((Up && Down) == false)
                            {
                                shapes[i].Inside = true;
                                shapes[j].Inside = true;
                                e.Graphics.DrawLine(brush, shapes[i].SetX, shapes[i].SetY, shapes[j].SetX, shapes[j].SetY);
                            }

                            Up = false;
                            Down = false;
                        }
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool check = false;

            if (shapes.Any())
            {
                foreach (My_Shape polyg in shapes)
                {
                    if (Draw && polyg.Check(e.X, e.Y))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            polyg.SetDelX = e.X - polyg.SetX;
                            polyg.SetDelY = e.Y - polyg.SetY;
                            polyg.SetChange = true;
                            check = true;
                        }
                        if (e.Button == MouseButtons.Right)
                        {
                            shapes.Remove(polyg);
                            Refresh();
                            check = true;
                            break;
                        }
                    }
                }
                if (check == false)
                {
                    if (CheckMoving(e.X, e.Y, shapes))
                    {
                        Draw = true;
                        for (int k = 0; k < shapes.Count; k++)
                        {
                            shapes[k].SetDelX = e.X - shapes[k].SetX;
                            shapes[k].SetDelY = e.Y - shapes[k].SetY;
                            shapes[k].SetChange = true;
                        }
                    }
                    else
                    {
                        Draw = true;
                        switch (figure)
                        {
                            case 1: shapes.Add(new Circle(e.X, e.Y)); break;
                            case 2: shapes.Add(new Square(e.X, e.Y)); break;
                            case 3: shapes.Add(new Triangle(e.X, e.Y)); break;
                            default: shapes.Add(new Circle(e.X, e.Y)); break;
                        }
                        Refresh();
                    }
                }

            }
            else
            {
                Draw = true;
                switch (figure)
                {
                    case 1: shapes.Add(new Circle(e.X, e.Y)); break;
                    case 2: shapes.Add(new Square(e.X, e.Y)); break;
                    case 3: shapes.Add(new Triangle(e.X, e.Y)); break;
                    default: shapes.Add(new Circle(e.X, e.Y)); break;
                }
                this.Refresh();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (My_Shape polyg in shapes)
            {
                if (polyg.SetChange)
                {
                    polyg.SetX = e.X - polyg.SetDelX;
                    polyg.SetY = e.Y - polyg.SetDelY;
                    this.Invalidate();
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (My_Shape polyg in shapes)
            {
                if (polyg.SetChange)
                {
                    polyg.SetChange = false;
                    if (polyg.Inside == false)
                    {
                        shapes.Remove(polyg);
                        Refresh();
                        break;
                    }
                    Refresh();
                }
            }
        }

        private void выбратьЦветФигурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowHelp = true;
            cd.Color = color_f;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                color_f = cd.Color;
                Invalidate();
            }
        }
        
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
            go_1 = true;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            random_number = rand.Next(0, 5);
            if (go_1)
            {
                foreach (My_Shape polyg in shapes)
                {
                    polyg.SetX += random_number;
                    polyg.SetY += random_number;
                    go_1 = false;
                    go_2 = true;
                }
            }
            else if (go_2)
            {
                foreach (My_Shape polyg in shapes)
                {
                    polyg.SetX -= random_number;
                    polyg.SetY -= random_number;
                    go_2 = false;
                    go_1 = true;
                }
            }
            Refresh();
        }
    }
    abstract class My_Shape
    {
        protected int x_0, y_0;

        public int SetX { set { x_0 = value; } get { return x_0; } }

        public int SetY { set { y_0 = value; } get { return y_0; } }

        public int SetDelX { set; get; }

        public int SetDelY { set; get; }

        public bool Inside { set; get; }

        public bool SetChange { set; get; }

        public static int r { get; set; }

        public static Color colo { get; set; }

        public My_Shape(int x, int y)
        {
            x_0 = x;
            y_0 = y;           
            r = 70;
            colo = Color.HotPink;
        }
        abstract public void Draw(Graphics g);
        abstract public bool Check(int x, int y);
    }
    class Circle : My_Shape
    {
        public Circle(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(colo);
            g.FillEllipse(brush, x_0 - r / 2, y_0 - r / 2, r, r);
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }
    class Square : My_Shape
    {
        public Square(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(colo);
            g.FillRectangle(brush, x_0 - r / 2, y_0 - r / 2, r, r);
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }
    class Triangle : My_Shape
    {
        public Triangle(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {
            Point vertex1 = new Point(x_0, y_0 - r / 2);
            Point vertex2 = new Point(x_0 - r / 2, y_0 + r / 2);
            Point vertex3 = new Point(x_0 + r / 2, y_0 + r / 2);
            Point[] tops = { vertex1, vertex2, vertex3 };
            SolidBrush brush = new SolidBrush(colo);
            g.FillPolygon(brush, tops);
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }
}
