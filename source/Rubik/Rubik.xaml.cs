using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RubikSolve
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        Rubik rubik = new Rubik();
        public MainWindow()
        {
            InitializeComponent();
            
            rubik.UpdateRubikCanvas(RubikCanvas);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rubik.RightClockwise(RubikCanvas);
        }
    }

    // 魔方类
    public class Rubik
    {
        public enum Surface { Up, Down, Left, Right, Front, Back};          // 枚举各面
        
        public enum Block { LeftTop, LeftBottom, RightTop, RightBottom};    // 枚举方块在各面的位置

        public enum Color { Yellow, White, Blue, Green, Red, Organe};       // 枚举各颜色 

        public enum Spin { F, FC, R, RC, U, UC};                            // 枚举各旋转操作

        public int[,] rubik = new int[6, 4];                                // 魔方数组，6面，每面4块
        
        // 构造函数
        public Rubik()
        {
            Initialize();
        }

        private Polygon GeneratePolygon(int surface, int block, int color)
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 2;
            switch (color)
            {
                case (int)Color.Yellow:
                    polygon.Fill = Brushes.Yellow;
                    break;
                case (int)Color.White:
                    polygon.Fill = Brushes.White;
                    break;
                case (int)Color.Blue:
                    polygon.Fill = Brushes.Blue;
                    break;
                case (int)Color.Green:
                    polygon.Fill = Brushes.Green;
                    break;
                case (int)Color.Red:
                    polygon.Fill = Brushes.Red;
                    break;
                case (int)Color.Organe:
                    polygon.Fill = Brushes.Orange;
                    break;
                default:
                    polygon.Fill = Brushes.Transparent;
                    break;
            }
            polygon.Points = GeneratePoints(surface, block);
            return polygon;
        }

        private PointCollection GeneratePoints(int surface, int block)
        {
            PointCollection points = new PointCollection();
            Point[] point = new Point[4];
            int offsetX = 300, offsetY = 270;
            switch (surface)
            {
                case (int)Surface.Down:
                    offsetY += 290;
                    break;
                case (int)Surface.Left:
                    offsetX -= 265;
                    offsetY -= 160;
                    break;
                case (int)Surface.Back:
                    offsetX += 265;
                    offsetY -= 160;
                    break;
                default:
                    break;
            }

            for (int index = 0; index < 4; index++)
            {
                point[index].X += offsetX;
                point[index].Y += offsetY;
            }

            switch (surface)
            {
                case (int)Surface.Up:
                case (int)Surface.Down:
                    switch (block)
                    {
                        case (int)Block.LeftTop:
                            point[0].Y -= 140;
                            point[1].X += 70;
                            point[1].Y -= 105;
                            point[2].Y -= 70;
                            point[3].X -= 70;
                            point[3].Y -= 105;
                            break;
                        case (int)Block.RightBottom:
                            point[0].Y -= 70;
                            point[1].X += 70;
                            point[1].Y -= 35;
                            point[3].X -= 70;
                            point[3].Y -= 35;
                            break;
                        case (int)Block.RightTop:
                            point[0].X += 70;
                            point[0].Y -= 105;
                            point[1].X += 140;
                            point[1].Y -= 70;
                            point[2].X += 70;
                            point[2].Y -= 35;
                            point[3].Y -= 70;
                            break;
                        case (int)Block.LeftBottom:
                            point[0].X -= 70;
                            point[0].Y -= 105;
                            point[1].Y -= 70;
                            point[2].X -= 70;
                            point[2].Y -= 35;
                            point[3].X -= 140;
                            point[3].Y -= 70;
                            break;
                        default:
                            break;
                    }
                    break;
                case (int)Surface.Left:
                case (int)Surface.Right:
                    switch (block)
                    {
                        case (int)Block.LeftTop:
                            point[1].X += 70;
                            point[1].Y -= 35;
                            point[2].X += 70;
                            point[2].Y += 50;
                            point[3].Y += 85;
                            break;
                        case (int)Block.LeftBottom:
                            point[0].Y += 85;
                            point[1].X += 70;
                            point[1].Y += 50;
                            point[2].X += 70;
                            point[2].Y += 135;
                            point[3].Y += 170;
                            break;
                        case (int)Block.RightTop:
                            point[0].X += 70;
                            point[0].Y -= 35;
                            point[1].X += 140;
                            point[1].Y -= 70;
                            point[2].X += 140;
                            point[2].Y += 15;
                            point[3].X += 70;
                            point[3].Y += 50;
                            break;
                        case (int)Block.RightBottom:
                            point[0].X += 70;
                            point[0].Y += 50;
                            point[1].X += 140;
                            point[1].Y += 15;
                            point[2].X += 140;
                            point[2].Y += 100;
                            point[3].X += 70;
                            point[3].Y += 135;
                            break;
                        default:
                            break;
                    }
                    break;
                case (int)Surface.Front:
                case (int)Surface.Back:
                    switch (block)
                    {
                        case (int)Block.RightTop:
                            point[1].X -= 70;
                            point[1].Y -= 35;
                            point[2].X -= 70;
                            point[2].Y += 50;
                            point[3].Y += 85;
                            break;
                        case (int)Block.RightBottom:
                            point[0].Y += 85;
                            point[1].X -= 70;
                            point[1].Y += 50;
                            point[2].X -= 70;
                            point[2].Y += 135;
                            point[3].Y += 170;
                            break;
                        case (int)Block.LeftTop:
                            point[0].X -= 70;
                            point[0].Y -= 35;
                            point[1].X -= 140;
                            point[1].Y -= 70;
                            point[2].X -= 140;
                            point[2].Y += 15;
                            point[3].X -= 70;
                            point[3].Y += 50;
                            break;
                        case (int)Block.LeftBottom:
                            point[0].X -= 70;
                            point[0].Y += 50;
                            point[1].X -= 140;
                            point[1].Y += 15;
                            point[2].X -= 140;
                            point[2].Y += 100;
                            point[3].X -= 70;
                            point[3].Y += 135;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            for (int index = 0; index < 4; index++)
            {
                points.Add(point[index]);
            }
            return points;
        }


        // 获取模仿状态Canvas图
        public void UpdateRubikCanvas(Canvas canvas)
        {
            canvas.Children.Clear();

            int[] indexProjectionSurfaces = { (int)Surface.Left, (int)Surface.Back, (int)Surface.Down };
            int[] indexNoumenonSurfaces = { (int)Surface.Front, (int)Surface.Right, (int)Surface.Up };

            foreach (int indexSurface in indexProjectionSurfaces)
            {
                for (int indexBlock = 0; indexBlock < 4; indexBlock++)
                {
                    canvas.Children.Add(GeneratePolygon(indexSurface, indexBlock, rubik[indexSurface, indexBlock]));
                }
            }
            foreach (int indexSurface in indexNoumenonSurfaces)
            {
                for (int indexBlock = 0; indexBlock < 4; indexBlock++)
                {
                    canvas.Children.Add(GeneratePolygon(indexSurface, indexBlock, rubik[indexSurface, indexBlock]));
                }
            }
        }


        // 魔方状态初始化
        public void Initialize()
        {
            for (int indexSurface = 0; indexSurface < 6; indexSurface++)
            {
                for (int indexBlock = 0; indexBlock < 4; indexBlock++)
                {
                    rubik[indexSurface, indexBlock] = indexSurface;
                }
            }
        }

        // 检查魔方状态，各面四块一致则返回真，否则返回假
        public bool CheckSurface(int[,] rubik)
        {
            try
            {
                for (int indexSurface = 0; indexSurface < 6; indexSurface++)
                {
                    for (int indexBlock = 1; indexBlock < 4; indexBlock++)  // 比较该面另外三个块是否与0号块一致
                    { 
                        if (rubik[indexSurface, indexBlock] != rubik[indexSurface, 0])
                        {
                            return false;   // 不等则直接返回假
                        }
                    }
                }
                return true;    // 循环完成说明未发现不等的情况，返回真
            }
            catch (Exception e)
            {
                MessageBox.Show($"检查出错！{e.Message}");
                return false;
            }
        }

        // 定义旋转操作 - 前顺时针
        public Spin FrontClockwise(Canvas canvas)
        {
            // 顺时针旋转前面四面
            RollBlocks(true, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(true, (int)Surface.Up, (int)Block.LeftBottom, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Left, (int)Block.LeftBottom);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(true, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom, (int)Surface.Down, (int)Block.LeftBottom, (int)Surface.Left, (int)Block.LeftTop);
            UpdateRubikCanvas(canvas);
            return Spin.F;
        }

        // 定义旋转操作 - 前逆时针
        public Spin FrontCounterClockwise(Canvas canvas)
        {
            // 逆时针旋转前面四面
            RollBlocks(false, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(false, (int)Surface.Up, (int)Block.LeftBottom, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Left, (int)Block.LeftBottom);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(false, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom, (int)Surface.Down, (int)Block.LeftBottom, (int)Surface.Left, (int)Block.LeftTop);
            UpdateRubikCanvas(canvas);
            return Spin.FC;
        }

        // 定义旋转操作 - 右顺时针
        public Spin RightClockwise(Canvas canvas)
        {
            // 顺时针旋转右面四面
            RollBlocks(true, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(true, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Down, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(true, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightBottom, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightTop);
            UpdateRubikCanvas(canvas);
            return Spin.R;
        }

        // 定义旋转操作 - 右逆时针
        public Spin RightCounterClockwise(Canvas canvas)
        {
            // 逆时针旋转右面四面
            RollBlocks(false, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(false, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Down, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(false, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightBottom, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightTop);
            UpdateRubikCanvas(canvas);
            return Spin.RC;
        }


        // 定义旋转操作 - 上顺时针
        public Spin UpClockwise(Canvas canvas)
        {
            // 顺时针旋转上面四面
            RollBlocks(true, (int)Surface.Up, (int)Block.LeftTop, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(true, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Left, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Right, (int)Block.LeftTop);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(true, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Left, (int)Block.LeftTop, (int)Surface.Back, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop);
            UpdateRubikCanvas(canvas);
            return Spin.U;
        }


        // 定义旋转操作 - 上逆时针
        public Spin UpCounterClockwise(Canvas canvas)
        {
            // 逆时针旋转上面四面
            RollBlocks(false, (int)Surface.Up, (int)Block.LeftTop, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(false, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Left, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Right, (int)Block.LeftTop);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(false, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Left, (int)Block.LeftTop, (int)Surface.Back, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop);
            UpdateRubikCanvas(canvas);
            return Spin.UC;
        }

        //滚动四个方块的颜色，顺时针方向定义四个面和方块顺序，即当clockwise为true时，1到2，2到3，3到4，4到1，clockwise为false时反向
        private void RollBlocks(bool clockwise, int surface1, int block1, int surface2, int block2, int surface3, int block3, int surface4, int block4)
        {
            int cache = rubik[surface1, block1];
            if (clockwise)  // 顺时针
            {
                rubik[surface1, block1] = rubik[surface4, block4];
                rubik[surface4, block4] = rubik[surface3, block3];
                rubik[surface3, block3] = rubik[surface2, block2];
                rubik[surface2, block2] = cache;
            }
            else            // 逆时针
            {
                rubik[surface1, block1] = rubik[surface2, block2];
                rubik[surface2, block2] = rubik[surface3, block3];
                rubik[surface3, block3] = rubik[surface4, block4];
                rubik[surface4, block4] = cache;
            }
        }

    }

}
