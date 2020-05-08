using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuickHullSolve
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public QuickHull quickHull = new QuickHull();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Slider_PointNum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                TextBox_PointNum.Text = $@"{(int)Math.Pow(Slider_PointNum.Value, 2.0)}";
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void Button_GeneratePoints_Click(object sender, RoutedEventArgs e)
        {
            quickHull.Generate(int.Parse(TextBox_PointNum.Text), (int)Canvas_QuickHull.ActualWidth, (int)Canvas_QuickHull.ActualHeight);
            quickHull.UpdateQuickHullCanvas(Canvas_QuickHull, false, -1);
            Button_Solve.IsEnabled = true;
        }

        private void Button_Solve_Click(object sender, RoutedEventArgs e)
        {
            Label_CommandCount.Content = quickHull.Solve();
            quickHull.UpdateQuickHullCanvas(Canvas_QuickHull, true, -1);
            Button_Solve.IsEnabled = false;
            Slider_History.Maximum = quickHull.historyLines.Count;
            Slider_History.Value = Slider_History.Maximum;
            DoubleCollection ticks = new DoubleCollection();
            for (int index = 1; index < Slider_History.Maximum; index++)
            {
                ticks.Add(index);
            }
            Slider_History.Ticks = ticks;
            Slider_History.IsSnapToTickEnabled = true;
        }


        // 保存canvas
        public static void SaveCanvas(Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);
            //canvas.Arrange(new Rect(size));

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth, //width
                (int)canvas.ActualHeight, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);

            SaveRTBAsPNG(rtb, filename);
        }

        private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            PngBitmapEncoder enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (FileStream stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }


        private void Slider_History_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            quickHull.UpdateQuickHullCanvas(Canvas_QuickHull, true, (int)Slider_History.Value - 1);
        }

        private void Button_SaveImg_Click(object sender, RoutedEventArgs e)
        {
            string stringTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
            int num = 0;
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string filePath = m_Dialog.SelectedPath.Trim();

            Directory.CreateDirectory($@"{filePath}\{stringTime}");

            for (int index = 0; index < quickHull.historyLines.Count; index++)
            {
                quickHull.UpdateQuickHullCanvas(Canvas_QuickHull, true, index);
                SaveCanvas(Canvas_QuickHull, 96, $@"{filePath}\{stringTime}\QuickHull_{num++}.png");
            }
            quickHull.UpdateQuickHullCanvas(Canvas_QuickHull, true, (int)Slider_History.Value - 1);
        }

        private void Label_PointNum_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Space);
        }

        private void Label_PointNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex(@"[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void Button_MultiQuickHull_Click(object sender, RoutedEventArgs e)
        {
            long totalCommandCount = 0;
            int pointNum = int.Parse(TextBox_PointNum.Text);
            int width = (int)Canvas_QuickHull.ActualWidth;
            int height = (int)Canvas_QuickHull.ActualHeight;
            int runTime = int.Parse(TextBox_RunTime.Text);
            for (int indexCount = 0; indexCount < runTime; indexCount++)
            {
                quickHull.Generate(pointNum, width, height);
                totalCommandCount += quickHull.Solve();
            }
            Label_CommandCount.Content = totalCommandCount / runTime;
        }
    }

    public class QuickHull
    {
        public PointCollection pointsAll = new PointCollection();       // 存储全点集
        public PointCollection pointsVertex = new PointCollection();    // 存储已分类的顶点集
        public PointCollection pointsInternal = new PointCollection();  // 存储已分类的内部点集
        public PointCollection pointsUnknown = new PointCollection();   // 存储未分类点集

        public List<List<Line>> historyLines = new List<List<Line>>();

        public QuickHull()
        {

        }

        // 执行快包算法
        public int Solve()
        {
            int totalCommandCount = 0;
            try
            {
                Point pointMostLeft = pointsUnknown[0];
                Point pointMostRight = pointsUnknown[0];
                totalCommandCount += 2;
                for (int index = 1; index < pointsUnknown.Count; index++)
                {
                    totalCommandCount += 2;
                    if (pointsUnknown[index].X < pointMostLeft.X)    // 待查点X < 已知极左点X
                    {
                        pointMostLeft = pointsUnknown[index];
                        totalCommandCount++;
                    }
                    else if (pointsUnknown[index].X == pointMostLeft.X && pointsUnknown[index].Y < pointMostLeft.Y)     // 待查点X = 已知极左点X，待查点Y < 已知极左点Y
                    {
                        pointMostLeft = pointsUnknown[index];
                        totalCommandCount++;
                    }
                    else if (pointsUnknown[index].X > pointMostRight.X) // 待查点X > 已知极右点X
                    {
                        pointMostRight = pointsUnknown[index];
                        totalCommandCount++;
                    }
                    else if (pointsUnknown[index].X == pointMostRight.X && pointsUnknown[index].Y > pointMostRight.Y)   // 待查点X = 已知极右点X，待查点Y > 已知极左点Y
                    {
                        pointMostRight = pointsUnknown[index];
                        totalCommandCount++;
                    }
                }
                totalCommandCount++;

                // 移动极左极右点到顶点
                ChangeToVertex(pointMostLeft, 0);
                ChangeToVertex(pointMostRight, 1);
                totalCommandCount += 4;

                historyLines.Add(GetLines(pointsVertex, out int commandCount));
                totalCommandCount += commandCount;

                PointCollection subPointsUp = new PointCollection(); // 上点集
                PointCollection subPointsDown = new PointCollection(); // 下点集

                // 分割点集
                totalCommandCount += DividePoints(pointMostLeft, pointMostRight, subPointsUp, subPointsDown);

                // 递归
                totalCommandCount += FindSubHull(subPointsUp, pointMostLeft, pointMostRight);
                totalCommandCount += FindSubHull(subPointsDown, pointMostRight, pointMostLeft);
                return totalCommandCount;

            }
            catch (Exception e)
            {
                //System.Windows.MessageBox.Show(e.Message);
                return 0;
            }
        }

        // 对输入的两点及其上点集执行递归算法，找到最远点，根据围成的三角形分类点，生成两条边，对每条边极其上方的点集再次执行本递归算法
        private int FindSubHull(PointCollection points, Point pointLeft, Point pointRight)
        {
            if (points.Count == 0)
            {
                return 1;
            }
            int totalCommandCount = 0;
            Point pointFarthest = FindFarthestPoint(points, pointLeft, pointRight, out int commandCount);
            totalCommandCount += commandCount;
            ChangeToVertex(pointFarthest, pointsVertex.IndexOf(pointLeft) + 1);
            totalCommandCount += 3;
            historyLines.Add(GetLines(pointsVertex, out commandCount));
            totalCommandCount += commandCount;
            points.Remove(pointFarthest);
            totalCommandCount++;

            PointCollection points1 = new PointCollection();
            PointCollection points2 = new PointCollection();
            foreach (Point point in points)
            {
                int deter1 = CalDeter(pointLeft, pointFarthest, point);
                int deter2 = CalDeter(pointFarthest, pointRight, point);
                totalCommandCount += 2;
                if (deter1 > 0)
                {
                    points1.Add(point);
                    totalCommandCount++;
                }
                else if (deter2 > 0)
                {
                    points2.Add(point);
                    totalCommandCount++;
                }
                else    // 三角形内
                {
                    ChangeToInternal(point);
                    totalCommandCount += 2; ;
                }
            }
            totalCommandCount += FindSubHull(points1, pointLeft, pointFarthest);
            totalCommandCount += FindSubHull(points2, pointFarthest, pointRight);
            return totalCommandCount;

        }

        // 查找最远点
        private Point FindFarthestPoint(PointCollection points, Point pointLeft, Point pointRight, out int commandCount)
        {
            commandCount = 0;
            Point pointFarthest = new Point();
            int areaMax = -1;
            foreach (Point point in points)
            {
                int area = CalDeter(pointLeft, pointRight, point);
                if (area > areaMax)
                {
                    pointFarthest = point;
                    areaMax = area;
                    commandCount += 2;
                }
            }
            return pointFarthest;
        }

        // 根据输入的左右两点和点集将点分割为两个部分
        private int DividePoints(Point pointLeft, Point pointRight, PointCollection pointsUp, PointCollection pointsDown)
        {
            int commandCount = 0;
            foreach (Point point in pointsUnknown)
            {
                int deter = CalDeter(pointLeft, pointRight, point);
                commandCount++;
                if (deter > 0)  // 上点集
                {
                    pointsUp.Add(point);
                    commandCount++;
                }
                else if (deter == 0)    // 共线
                {
                    ChangeToInternal(point);
                    commandCount += 2;
                }
                else            // 下点集
                {
                    pointsDown.Add(point);
                    commandCount++;
                }
            }
            return commandCount;
        }

        // 计算行列式
        private int CalDeter(Point point1, Point point2, Point point3)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            int x3 = (int)point3.X;
            int y3 = (int)point3.Y;
            return x1 * y2 + x3 * y1 + x2 * y3 - x3 * y2 - x2 * y1 - x1 * y3;
        }

        // 设定点为内部点
        private void ChangeToInternal(Point point)
        {
            pointsUnknown.Remove(point);
            pointsInternal.Add(point);
        }

        // 设定点为顶点
        private void ChangeToVertex(Point point, int index)
        {

            pointsUnknown.Remove(point);
            pointsVertex.Insert(index, point);
        }

        // 生成随机点组num个
        public void Generate(int num, int maxX, int maxY)
        {
            pointsAll.Clear();    // 先清空
            pointsVertex.Clear();
            pointsInternal.Clear();
            pointsUnknown.Clear();
            historyLines.Clear();

            Random ran = new Random(GenerateRandomSeed());
            for (int index = 0; index < num; index++)
            {
                Point point = new Point(ran.Next(5, maxX - 5), ran.Next(5, maxY - 5));
                pointsAll.Add(point);
                pointsUnknown.Add(point);
            }

        }

        // 生成随机种子
        private static int GenerateRandomSeed()
        {
            return (int)DateTime.Now.Ticks;
        }

        // 更新Canvas
        public void UpdateQuickHullCanvas(Canvas canvas, bool drawLines, int step)
        {
            canvas.Children.Clear();
            foreach (Point point in pointsUnknown)
            {
                canvas.Children.Add(GeneratePoint(point, 0));   // 未知状态点
            }
            foreach (Point point in pointsInternal)
            {
                canvas.Children.Add(GeneratePoint(point, 2));   // 已分类的内部点
            }
            // 画线
            if (drawLines)
            {
                if (step == -1)
                {
                    DrawHullLines(canvas, GetLines(pointsVertex, out int _));
                }
                else
                {
                    try
                    {
                        DrawHullLines(canvas, historyLines[step]);
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }
            // 画点
            foreach (Point point in pointsVertex)
            {
                canvas.Children.Add(GeneratePoint(point, 1));   // 已分类的顶点
            }
            canvas.UpdateLayout();
        }

        // 以椭圆实现点
        private Ellipse GeneratePoint(Point point, int type)
        {
            Ellipse ellipse = new Ellipse
            {
                StrokeThickness = 8,
                Height = 8,
                Width = 8,
                Margin = new Thickness(-4, -4, 0, 0)
            };
            switch (type)
            {
                case 0:     // 未知状态
                    ellipse.Stroke = Brushes.Black;
                    ellipse.Fill = Brushes.Black;
                    break;
                case 1:     // 已分类的顶点
                    ellipse.Stroke = Brushes.Red;
                    ellipse.Fill = Brushes.Red;
                    break;
                case 2:     // 内部点
                    ellipse.Stroke = Brushes.Gray;
                    ellipse.Fill = Brushes.Gray;
                    break;
                default:
                    break;
            }
            Canvas.SetLeft(ellipse, point.X);
            Canvas.SetTop(ellipse, 600 - point.Y);
            return ellipse;
        }

        public void DrawHullLines(Canvas canvas, List<Line> lines)
        {
            foreach (Line line in lines) //GetLines(pointsVertex))
            {
                canvas.Children.Add(line);
            }
            canvas.UpdateLayout();
        }

        private List<Line> GetLines(PointCollection points, out int commandCount)
        {
            commandCount = 0;
            List<Line> lines = new List<Line>();
            for (int index = 0; index < points.Count - 1; index++)
            {
                lines.Add(GenerateLine(points[index], points[index + 1]));
                commandCount += 2;
            }
            lines.Add(GenerateLine(points[points.Count - 1], points[0]));
            commandCount += 2;
            return lines;
        }

        private Line GenerateLine(Point point1, Point point2)
        {
            Line line = new Line
            {
                X1 = point1.X,
                Y1 = 600 - point1.Y,
                X2 = point2.X,
                Y2 = 600 - point2.Y,
                Stroke = Brushes.Orange,
                StrokeThickness = 4
            };
            return line;
        }

    }
}
