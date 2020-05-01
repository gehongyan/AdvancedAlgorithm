using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Sorting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public short sortingMethod = 0;     // 算法方式代码，0为插入排序，1为希尔排序，2为快速排序
        public short arrayScale = 10;        // 随机生成数组规模
        public MainWindow()
        {
            InitializeComponent();
            
        }

        // 执行
        private void Button_Roll_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TextBox_RunCount.Text, out int runCount))
            {
                if (runCount > 0)
                {
                    long totalRunCount = 0;
                    for (int indexRunCount = 0; indexRunCount < runCount; indexRunCount++)
                    {
                        totalRunCount += RunOnce();
                    }
                    Label_AveragRunCount.Content = totalRunCount / runCount;
                }
            }
            else
            {
                MessageBox.Show("执行次数输入错误！");
            }
        }

        private int RunOnce()
        {
            int[] arr = GenerateRandom(1, arrayScale * 10, arrayScale);  // 待排序数组

            List<int> listOriginal = new List<int>(arr);
            DataList_Original.ItemsSource = listOriginal;

            int compareCount = InsertionSort(arr);    // 调用插入排序函数

            List<int> listOrdered = new List<int>(arr);
            DataList_Ordered.ItemsSource = listOrdered;

            return compareCount;
        }

        // 插入排序 - 减治法
        private static int InsertionSort(int[] arr)
        {
            int compareCount = 0;
            // arr[0]个体为有序子数列
            for (int index = 1; index < arr.Length; index++)
            {
                int insertVal = arr[index];  // 记录待插入数值
                int insertIndex = index - 1; // 有序子数列末尾索引

                // 满足条件则未找到合适的插入位置
                while (insertIndex >= 0 && arr[insertIndex] > insertVal)
                {
                    arr[insertIndex + 1] = arr[insertIndex];    // 将比待插入数值大的数向右移动
                    insertIndex--;                              // 继续向前比较
                    compareCount++;                             // 比较次数自增
                }
                // 不满足条件则找到了合适的插入位置
                arr[insertIndex + 1] = insertVal;
            }
            return compareCount;
        }

        // 生成随机种子
        public static int GenerateRandomSeed()
        {
            return (int)DateTime.Now.Ticks;
        }

        // 产生随机数数组
        // 输入随机数下限 minValue，上限 maxValue，生成数量 randNum
        public int[] GenerateRandom(int minValue, int maxValue, int randNum)
        {
            Random ran = new Random(GenerateRandomSeed());
            int[] arr = new int[randNum];

            for (int i = 0; i < randNum; i++)
            {
                arr[i] = ran.Next(minValue, maxValue);
            }
            return arr;
        }


        // 插入排序
        private void RadioButton_Method_Insertion_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 0;
        }

        // 希尔排序
        private void RadioButton_Method_Shell_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 1;
        }
       
        // 快速排序
        private void RadioButton_Method_Quick_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 2;
        }

        // 规模为10
        private void RadioButton_Scale_10_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 10;
        }

        // 规模为100
        private void RadioButton_Scale_100_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 100;
        }

        // 规模为1000
        private void RadioButton_Scale_1000_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 1000;
        }

        // 规模为10000
        private void RadioButton_Scale_10000_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 10000;
        }

        //输入校验
        private void TextBox_RunCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex(@"[^0-9]+");
            e.Handled = re.IsMatch(e.Text.Trim());
        }
    }
}
