using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RubikSolve
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Rubik rubik = new Rubik();
        public MainWindow()
        {
            InitializeComponent();

            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_UC_Click(object sender, RoutedEventArgs e)
        {
            rubik.UpCounterClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_U_Click(object sender, RoutedEventArgs e)
        {
            rubik.UpClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_R_Click(object sender, RoutedEventArgs e)
        {
            rubik.RightClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_RC_Click(object sender, RoutedEventArgs e)
        {
            rubik.RightCounterClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_FC_Click(object sender, RoutedEventArgs e)
        {
            rubik.FrontCounterClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_F_Click(object sender, RoutedEventArgs e)
        {
            rubik.FrontClockwise(rubik.rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Slider_ShuffleStep_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Label_ShuffleStep.Content = Slider_ShuffleStep.Value;
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void Button_Shuffle_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_ShuffleRecord.Text = rubik.Shuffle((int)Slider_ShuffleStep.Value);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }

        private void Button_SearchSolution_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_SolutionTitle.Text = rubik.Solve(CehckBox_SaveImg.IsChecked ?? false, Canvas_Rubik);
            rubik.UpdateRubikCanvas(Canvas_Rubik);
        }
    }

    // 魔方类
    public class Rubik
    {
        public enum Surface { Up, Down, Left, Right, Front, Back };          // 枚举各面

        public enum Block { LeftTop, LeftBottom, RightTop, RightBottom };    // 枚举方块在各面的位置

        public enum Color { Yellow, White, Blue, Green, Red, Organe };       // 枚举各颜色 

        public enum Spin { F, FC, R, RC, U, UC };                            // 枚举各旋转操作

        public string[] stringSpin = { "F ", "F' ", "R ", "R' ", "U ", "U' " };

        public int[,] rubik = new int[6, 4];                                // 魔方数组，6面，每面4块

        // 构造函数
        public Rubik()
        {
            Initialize();
        }

        // 生成多边形贴图
        private Polygon GeneratePolygon(int surface, int block, int color)
        {
            Polygon polygon = new Polygon
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
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

        // 生成多边形顶点点集
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
            canvas.UpdateLayout();
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

        // 打乱魔方
        public string Shuffle(int totalStep)
        {
            string stringStep = "";
            if (totalStep < 0)
            {
                System.Windows.MessageBox.Show("打乱步数不可为负！");
            }
            else
            {
                int[] rollCollection = GenerateRandom(1, 6, totalStep);
                for (int indexStep = 0; indexStep < totalStep; indexStep++)
                {
                    stringStep += OneStep(rubik, rollCollection[indexStep]);
                }
            }
            return stringStep;
        }

        // 根据输入的操作码执行一步操作
        private string OneStep(int[,] rubik, int step)
        {
            string stringStep = "";
            switch (step)
            {
                case (int)Spin.F:
                    stringStep += stringSpin[(int)FrontClockwise(rubik)];
                    break;
                case (int)Spin.FC:
                    stringStep += stringSpin[(int)FrontCounterClockwise(rubik)];
                    break;
                case (int)Spin.U:
                    stringStep += stringSpin[(int)UpClockwise(rubik)];
                    break;
                case (int)Spin.UC:
                    stringStep += stringSpin[(int)UpCounterClockwise(rubik)];
                    break;
                case (int)Spin.R:
                    stringStep += stringSpin[(int)RightClockwise(rubik)];
                    break;
                case (int)Spin.RC:
                    stringStep += stringSpin[(int)RightCounterClockwise(rubik)];
                    break;
                default:
                    break;
            }
            return stringStep;
        }

        // 生成随机种子
        private static int GenerateRandomSeed()
        {
            return (int)DateTime.Now.Ticks;
        }

        // 产生随机数组
        // 输入随机数下限 minValue，上限 maxValue，生成数量 randNum
        private static int[] GenerateRandom(int minValue, int maxValue, int randNum)
        {
            Random ran = new Random(GenerateRandomSeed());
            int[] arr = new int[randNum];

            for (int i = 0; i < randNum; i++)
            {
                arr[i] = ran.Next(minValue, maxValue);
            }

            return arr;
        }

        // 拷贝模仿状态
        private int[,] CopyRubik(int[,] rubikOrigin)
        {
            int[,] rubikNew = new int[6, 4];
            for (int indexSurface = 0; indexSurface < 6; indexSurface++)
            {
                for (int indexBlock = 0; indexBlock < 4; indexBlock++)
                {
                    rubikNew[indexSurface, indexBlock] = rubikOrigin[indexSurface, indexBlock];
                }
            }
            return rubikNew;
        }


        // 搜索魔方解
        public string Solve(bool boolExportStepImage, Canvas canvasRubik)
        {
            string stringPath = "";
            int num = 0;
            string filePath = "";
            string stringTime = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
            if (boolExportStepImage)
            {
                FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
                DialogResult result = m_Dialog.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    return "";
                }
                filePath = m_Dialog.SelectedPath.Trim();

                Directory.CreateDirectory($@"{filePath}\{stringTime}");
            }



            SolveNode finalNode;    // 最终节点
            // 初始化队列
            Queue<SolveNode> checkQueue = new Queue<SolveNode>();
            SolveNode solveNodeRoot = new SolveNode
            {
                spin = -1,
                currentRubik = CopyRubik(rubik),
                path = new List<int>()
            };

            checkQueue.Enqueue(solveNodeRoot);

            while (true)
            {
                if (CheckSurface(checkQueue.Peek().currentRubik))   // 检查队列头的魔方状态
                {
                    // 检查通过
                    finalNode = checkQueue.Peek();
                    break;
                }
                else
                {
                    // 检查不通过
                    foreach (SolveNode node in GetSolveNodeSon(checkQueue.Dequeue()))
                    {
                        checkQueue.Enqueue(node);
                    }
                }
            }

            if (boolExportStepImage)
            {
                SaveCanvas(canvasRubik, 96, $@"{filePath}\{stringTime}\Rubik - {num++} - Origin.png");
            }

            foreach (int step in finalNode.path)
            {
                string currentStep = OneStep(rubik, step);
                stringPath += currentStep;
                if (boolExportStepImage)
                {
                    UpdateRubikCanvas(canvasRubik);
                    SaveCanvas(canvasRubik, 96, $@"{filePath}\{stringTime}\Rubik - {num++} - {currentStep}.png");
                }
            }

            return stringPath;
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

        // 获取可能的下一步旋转
        private List<SolveNode> GetSolveNodeSon(SolveNode node)
        {
            List<SolveNode> nodeSon = new List<SolveNode>();
            int oppositeSpin;   // 获取该操作的相反旋转
            if (node.spin == -1)        // 初始状态
            {
                oppositeSpin = -1;
            }
            else if (node.spin % 2 == 1)
            {
                oppositeSpin = node.spin - 1;
            }
            else
            {
                oppositeSpin = node.spin + 1;
            }
            // 本循环会成功执行五次，失败的一次为与传入操作的相反操作
            for (int currentSpin = 0; currentSpin < 6; currentSpin++)
            {
                if (currentSpin != oppositeSpin)
                {
                    SolveNode newNode = new SolveNode
                    {
                        path = new List<int>()
                    };
                    node.path.ForEach(i => newNode.path.Add(i));    // 拷贝步骤
                    newNode.path.Add(currentSpin);                  // 添加本步骤
                    newNode.spin = currentSpin;                     // 标记本步骤
                    int[,] currentRubik = CopyRubik(node.currentRubik);
                    OneStep(currentRubik, currentSpin);
                    newNode.currentRubik = currentRubik;
                    nodeSon.Add(newNode);                           // 加入
                }
            }
            return nodeSon;
        }

        private struct SolveNode
        {
            public int spin;            // 旋转操作编号
            public List<int> path;      // 到达该节点的路径
            public int[,] currentRubik; // 当前节点模仿状态
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
                System.Windows.MessageBox.Show($"检查出错！{e.Message}");
                return false;
            }
        }

        // 定义旋转操作 - 前顺时针
        public Spin FrontClockwise(int[,] rubik)
        {
            // 顺时针旋转前面四面
            RollBlocks(rubik, true, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Up, (int)Block.LeftBottom, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Left, (int)Block.LeftBottom);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom, (int)Surface.Down, (int)Block.LeftBottom, (int)Surface.Left, (int)Block.LeftTop);
            return Spin.F;
        }

        // 定义旋转操作 - 前逆时针
        public Spin FrontCounterClockwise(int[,] rubik)
        {
            // 逆时针旋转前面四面
            RollBlocks(rubik, false, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom, (int)Surface.Front, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Up, (int)Block.LeftBottom, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Left, (int)Block.LeftBottom);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom, (int)Surface.Down, (int)Block.LeftBottom, (int)Surface.Left, (int)Block.LeftTop);
            return Spin.FC;
        }

        // 定义旋转操作 - 右顺时针
        public Spin RightClockwise(int[,] rubik)
        {
            // 顺时针旋转右面四面
            RollBlocks(rubik, true, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Down, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightBottom, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightTop);
            return Spin.R;
        }

        // 定义旋转操作 - 右逆时针
        public Spin RightCounterClockwise(int[,] rubik)
        {
            // 逆时针旋转右面四面
            RollBlocks(rubik, false, (int)Surface.Right, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop, (int)Surface.Right, (int)Block.RightBottom, (int)Surface.Right, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Down, (int)Block.RightTop, (int)Surface.Front, (int)Block.RightBottom);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightBottom, (int)Surface.Down, (int)Block.RightBottom, (int)Surface.Front, (int)Block.RightTop);
            return Spin.RC;
        }

        // 定义旋转操作 - 上顺时针
        public Spin UpClockwise(int[,] rubik)
        {
            // 顺时针旋转上面四面
            RollBlocks(rubik, true, (int)Surface.Up, (int)Block.LeftTop, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.LeftBottom);
            // 顺时针旋转靠左的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Left, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Right, (int)Block.LeftTop);
            // 顺时针旋转靠右的邻接侧面
            RollBlocks(rubik, true, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Left, (int)Block.LeftTop, (int)Surface.Back, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop);
            return Spin.U;
        }

        // 定义旋转操作 - 上逆时针
        public Spin UpCounterClockwise(int[,] rubik)
        {
            // 逆时针旋转上面四面
            RollBlocks(rubik, false, (int)Surface.Up, (int)Block.LeftTop, (int)Surface.Up, (int)Block.RightTop, (int)Surface.Up, (int)Block.RightBottom, (int)Surface.Up, (int)Block.LeftBottom);
            // 逆时针旋转靠左的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Front, (int)Block.LeftTop, (int)Surface.Left, (int)Block.RightTop, (int)Surface.Back, (int)Block.RightTop, (int)Surface.Right, (int)Block.LeftTop);
            // 逆时针旋转靠右的邻接侧面
            RollBlocks(rubik, false, (int)Surface.Front, (int)Block.RightTop, (int)Surface.Left, (int)Block.LeftTop, (int)Surface.Back, (int)Block.LeftTop, (int)Surface.Right, (int)Block.RightTop);
            return Spin.UC;
        }

        //滚动四个方块的颜色，顺时针方向定义四个面和方块顺序，即当clockwise为true时，1到2，2到3，3到4，4到1，clockwise为false时反向
        private void RollBlocks(int[,] rubik, bool clockwise, int surface1, int block1, int surface2, int block2, int surface3, int block3, int surface4, int block4)
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
