using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Sorting
{
    /// <summary>
    /// Sorting.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public short sortingMethod = 0;     // 算法方式代码，0为插入排序，1为希尔排序，2为快速排序
        public short arrayScale = 10;        // 随机生成数组规模
        public bool flagShowArrayContent = false;
        public bool flagDecimal = false;
        public int maxCompareCount = 200000000;
        public MainWindow()
        {
            InitializeComponent();
            InitializeValue();
        }

        // 选项初始化
        private void InitializeValue()
        {
            RadioButton_Method_Insertion.IsChecked = true;
            RadioButton_Scale_10.IsChecked = true;
        }

        // 执行
        private void Button_Roll_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TextBox_RunCount.Text, out int runCount))
            {
                if (runCount > 0)
                {
                    if (CountEstimated(sortingMethod, arrayScale, runCount) > maxCompareCount)
                    {
                        MessageBox.Show("执行次数设置过大！");
                    }
                    else
                    {
                        long totalCompareCount = 0;
                        long totalExchangeCount = 0;
                        long totalRuntimeCount = 0;
                        for (int indexRunCount = 0; indexRunCount < runCount; indexRunCount++)
                        {
                            RunOnce(out int compareCount, out int exchangeCount, out int runtimeCount);
                            totalCompareCount += compareCount;
                            totalExchangeCount += exchangeCount;
                            totalRuntimeCount += runtimeCount;
                        }
                        Label_AverageCompareCount.Content = $"{totalCompareCount / runCount}";
                        Label_AverageExchangeCount.Content = $"{totalExchangeCount / runCount}";
                        Label_AverageRuntimeCount.Content = $"{totalRuntimeCount / runCount}";
                    }
                }
            }
            else
            {
                MessageBox.Show("执行次数输入错误！");
            }
        }

        // 估算执行消耗
        private static int CountEstimated(short sortingMethod, short arrayScale, int runCount)
        {
            if (sortingMethod == 0)
            {
                return arrayScale * arrayScale * runCount;
            }
            else // 1 或 2
            {
                return arrayScale * (int)Math.Log(arrayScale) * runCount;
            }
        }

        // 估算限制消耗的情况下的最大运行次数
        private int RuntimeEstimated(short sortingMethod, short arrayScale, int maxCompareTime)
        {
            if (CheckBox_ShowArrayContent.IsChecked == true)
            {
                return 1;
            }
            else
            {
                if (sortingMethod == 0)
                {
                    return maxCompareTime / arrayScale / arrayScale;
                }
                else
                {
                    return maxCompareTime / arrayScale / (int)Math.Log(arrayScale);
                }
            }
        }

        // 执行一次排序
        private void RunOnce(out int compareCount, out int exchangeCount, out int runtimeCount)
        {
            // 待排序数组
            if (flagDecimal)    // 小数
            {
                double[] arr = GenerateRandom(0.0, arrayScale * 10.0, arrayScale);
                if (flagShowArrayContent)
                {
                    // 显示数组内容
                    List<double> listOriginal = new List<double>(arr);
                    DataList_Original.ItemsSource = listOriginal;
                }
                if (sortingMethod == 0)
                {
                    // 调用插入排序函数
                    InsertionSort(arr, out compareCount, out exchangeCount, out runtimeCount);
                }
                else if (sortingMethod == 1)
                {
                    // 调用希尔排序
                    ShellSort(arr, out compareCount, out exchangeCount, out runtimeCount);
                }
                else  // sortingMethod == 2
                {
                    // 快速排序
                    QuickSort(arr, 0, arr.Length - 1, 0, 0, 0, out compareCount, out exchangeCount, out runtimeCount);
                }

                if (flagShowArrayContent)
                {
                    // 显示数组内容
                    List<double> listOrdered = new List<double>(arr);
                    DataList_Ordered.ItemsSource = listOrdered;
                }
            }
            else        // 整数
            {
                int[] arr = GenerateRandom(1, arrayScale * 10, arrayScale);
                if (flagShowArrayContent)
                {
                    // 显示数组内容
                    List<int> listOriginal = new List<int>(arr);
                    DataList_Original.ItemsSource = listOriginal;
                }
                if (sortingMethod == 0)
                {
                    // 调用插入排序函数
                    InsertionSort(arr, out compareCount, out exchangeCount, out runtimeCount);
                }
                else if (sortingMethod == 1)
                {
                    // 调用希尔排序
                    ShellSort(arr, out compareCount, out exchangeCount, out runtimeCount);
                }
                else  // sortingMethod == 2
                {
                    // 快速排序
                    QuickSort(arr, 0, arr.Length - 1, 0, 0, 0, out compareCount, out exchangeCount, out runtimeCount);
                }

                if (flagShowArrayContent)
                {
                    // 显示数组内容
                    List<int> listOrdered = new List<int>(arr);
                    DataList_Ordered.ItemsSource = listOrdered;
                }
            }

            
        }

        // 快速排序 - 分治法
        private static void QuickSort<T>(T[] arr, int begin, int end, int oldCompareCount, int oldExchangeCount, int oldRuntimeCount, out int currentCompareCount, out int currentExchangeCount, out int currentRuntimeCount)
            where T: IComparable<T>
        {
            currentCompareCount = oldCompareCount;
            currentExchangeCount = oldExchangeCount;
            currentRuntimeCount = oldRuntimeCount;
            if (begin < end)
            {
                T middle = arr[(begin + end) / 2];
                int leftEnd = begin - 1;
                int rightBegin = end + 1;
                currentRuntimeCount += 4;
                while (true)
                {
                    while (arr[++leftEnd].CompareTo(middle) < 0)
                    {
                        currentCompareCount++;
                        currentRuntimeCount += 2;
                    }
                    currentRuntimeCount += 2;

                    while (arr[--rightBegin].CompareTo(middle) > 0)
                    {
                        currentCompareCount++;
                        currentRuntimeCount += 2;
                    }
                    currentRuntimeCount += 2;

                    if (leftEnd >= rightBegin)
                    {
                        currentRuntimeCount++;
                        break;
                    }

                    Swap(arr, leftEnd, rightBegin);
                    currentRuntimeCount += 3;
                    currentExchangeCount++;
                }
                QuickSort(arr, begin, leftEnd - 1, currentCompareCount, currentExchangeCount, currentRuntimeCount, out int returnCompareCount, out int returnExchangeCount, out int returnRuntimeCount);
                currentCompareCount = returnCompareCount;
                currentExchangeCount = returnExchangeCount;
                currentRuntimeCount = returnRuntimeCount;
                QuickSort(arr, rightBegin + 1, end, currentCompareCount, currentExchangeCount, currentRuntimeCount, out returnCompareCount, out returnExchangeCount, out returnRuntimeCount);
                currentCompareCount = returnCompareCount;
                currentExchangeCount = returnExchangeCount;
                currentRuntimeCount = returnRuntimeCount;
            }
        }

        // 交换数组元素
        private static void Swap<T>(T[] numbers, int i, int j)
        {
            T number = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = number;
        }

        // 希尔排序 - 减治法
        public static void ShellSort<T>(T[] array, out int compareCount, out int exchangeCount, out int runtimeCount)
            where T: IComparable<T>
        {
            compareCount = 0;
            exchangeCount = 0;
            runtimeCount = 0;
            int groupGap = array.Length / 2;
            runtimeCount++;
            while (groupGap >= 1)
            {
                runtimeCount += 2;
                for (int groupIndex = 0; groupIndex < groupGap; groupIndex++)
                {
                    runtimeCount += 2;
                    for (int index = groupIndex + groupGap; index < array.Length; index += groupGap)
                    {
                        T insertVal = array[index];
                        int insertIndex = index;
                        runtimeCount += 4;
                        while (insertIndex - groupGap >= groupIndex && insertVal.CompareTo(array[insertIndex - groupGap]) < 0) 
                        {
                            runtimeCount += 4;
                            array[insertIndex] = array[insertIndex - groupGap];
                            insertIndex -= groupGap;
                            exchangeCount++;
                        }
                        runtimeCount += 3;
                        array[insertIndex] = insertVal;
                        compareCount = exchangeCount + 1;
                    }
                    runtimeCount++;
                }
                runtimeCount += 2;
                //一次插入排序结束，缩小d的值
                groupGap /= 2;
            }
            runtimeCount++;
        }

        // 插入排序 - 减治法
        private static void InsertionSort<T>(T[] arr, out int compareCount, out int exchangeCount, out int runtimeCount)
            where T: IComparable<T>
        {
            compareCount = 0;
            exchangeCount = 0;
            runtimeCount = 0;
            // arr[0]个体为有序子数列
            runtimeCount++;
            for (int index = 1; index < arr.Length; index++)
            {
                runtimeCount += 3;
                T insertVal = arr[index];  // 记录待插入数值
                int insertIndex = index - 1; // 有序子数列末尾索引

                // 满足条件则未找到合适的插入位置
                while (insertIndex >= 0 && arr[insertIndex].CompareTo(insertVal) > 0)
                {
                    arr[insertIndex + 1] = arr[insertIndex];    // 将比待插入数值大的数向右移动
                    insertIndex--;                              // 继续向前比较
                    exchangeCount++;                             // 比较次数自增
                    runtimeCount += 4;
                }
                runtimeCount += 2;
                // 不满足条件则找到了合适的插入位置
                arr[insertIndex + 1] = insertVal;
                compareCount = exchangeCount + 1;
                runtimeCount += 2;
            }
            runtimeCount++;
        }

        // 生成随机种子
        public static int GenerateRandomSeed()
        {
            return (int)DateTime.Now.Ticks;
        }

        // 产生随机数组
        // 输入随机数下限 minValue，上限 maxValue，生成数量 randNum
        public static T[] GenerateRandom<T>(T minValue, T maxValue, int randNum)
        {
            Random ran = new Random(GenerateRandomSeed());
            T[] arr = new T[randNum];

            if (typeof(T) == typeof(int))
            {
                for (int i = 0; i < randNum; i++)
                {
                    arr[i] = (T)Convert.ChangeType(ran.Next(Convert.ToInt32(minValue), Convert.ToInt32(maxValue)), typeof(T));
                }
            }
            else // double
            {
                for (int i = 0; i < randNum; i++)
                {
                    arr[i] = (T)Convert.ChangeType(ran.NextDouble() * (Convert.ToDouble(maxValue) - Convert.ToDouble(minValue)) + Convert.ToDouble(minValue), typeof(T));
                }
            }
            return arr;
        }

        // 插入排序
        private void RadioButton_Method_Insertion_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 0;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 希尔排序
        private void RadioButton_Method_Shell_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 1;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 快速排序
        private void RadioButton_Method_Quick_Checked(object sender, RoutedEventArgs e)
        {
            sortingMethod = 2;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 规模为10
        private void RadioButton_Scale_10_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 10;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 规模为100
        private void RadioButton_Scale_100_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 100;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 规模为1000
        private void RadioButton_Scale_1000_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 1000;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        // 规模为10000
        private void RadioButton_Scale_10000_Checked(object sender, RoutedEventArgs e)
        {
            arrayScale = 10000;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
        }

        //输入校验
        private void TextBox_RunCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex(@"[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        // 显示数组内容，只运行一次
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            flagShowArrayContent = true;
            Label_MaxCompare.Content = 1;
            TextBox_RunCount.Text = "1";
            TextBox_RunCount.IsEnabled = false;
            Expander_Ordered.Visibility = Visibility.Visible;
            Expander_Original.Visibility = Visibility.Visible;
            SortingWindow.Height = 600;
        }

        // 不显示数组内容，允许多次运行求均值
        private void CheckBox_ShowArrayContent_Unchecked(object sender, RoutedEventArgs e)
        {
            flagShowArrayContent = false;
            Label_MaxCompare.Content = RuntimeEstimated(sortingMethod, arrayScale, maxCompareCount);
            TextBox_RunCount.Text = "1";
            TextBox_RunCount.IsEnabled = true;
            Expander_Ordered.Visibility = Visibility.Hidden;
            Expander_Original.Visibility = Visibility.Hidden;
            SortingWindow.Height = 200;
        }

        private void CheckBox_Decimal_Checked(object sender, RoutedEventArgs e)
        {
            flagDecimal = true;
        }

        private void CheckBox_Decimal_Unchecked(object sender, RoutedEventArgs e)
        {
            flagDecimal = false;
        }

        private void TextBox_RunCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Space);
        }
    }
}
