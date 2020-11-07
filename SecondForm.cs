using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace AlgorithmsVisual
{
    class SecondForm : Form
    {
        private Size sizeForArray;
        private Size sizeForButtons;

        private Button startButton;
        private Button mixButton;
        private Button backButton;
        private Label label1;
        private Label label2;
        private Label label3;

        private int[] array;
        private string algorithmName;
        private int comparisons;
        private int swaps;

        private Dictionary<string, Action> algorithms;

        private Timer timer;
        public SecondForm(int arraySize, string algorithmName)
        {
            sizeForArray = new Size(500, 400);
            sizeForButtons = new Size(200, 400);
            //sizeForArray = new Size(800, 700);
            //sizeForButtons = new Size(400, 700);
            ClientSize = new Size(sizeForArray.Width + sizeForButtons.Width, sizeForArray.Height);
            DoubleBuffered = true;

            array = InitializeArray(arraySize);
            this.algorithmName = algorithmName;
            comparisons = 0;
            swaps = 0;

            startButton = new Button();
            mixButton = new Button();
            backButton = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            InitializeButtons();

            algorithms = new Dictionary<string, Action>();
            algorithms.Add("Bubble sort", BubbleSort);
            algorithms.Add("Coctail sort", CoctailSort);
            algorithms.Add("Comb sort", CombSort);
            algorithms.Add("Selection sort", SelectionSort);
            algorithms.Add("Insertion sort", InsertionSort);
            algorithms.Add("Shell sort", ShellSort);
            algorithms.Add("Merge sort", MergeSort);
            algorithms.Add("Quick sort", QuickSort);
            algorithms.Add("Tree sort", TreeSort);
            algorithms.Add("Heap sort", HeapSort);
            algorithms.Add("LSD Radix sort", LSDRadixSort);
            algorithms.Add("MSD Radix sort", MSDRadixSort);

            //Fix size
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = algorithmName;
            
            Paint += StaticPaintArray;

            if (ArrayIsAlreadySorted())
                startButton.Enabled = false;
        }

        //Рисую массив серым цветом
        private void StaticPaintArray(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

            int w = sizeForArray.Width / array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                var b = Brushes.Gray;
                g.FillRectangle(b, new Rectangle(i * w, sizeForArray.Height - array[i], w, array[i]));
                g.DrawRectangle(new Pen(Color.Black), new Rectangle(i * w, sizeForArray.Height - array[i], w, array[i]));
            }
        }

        //Инициализирую кнопки и лейблы
        private void InitializeButtons()
        {
            var buttonSize = new Size(3 * sizeForButtons.Width / 5, ClientSize.Height / 10);
            //var xOffset = sizeForArray.Width + sizeForButtons.Width / 5;
            var xOffset = sizeForArray.Width + (sizeForButtons.Width - buttonSize.Width) / 2;
            var yOffset = ClientSize.Height / 10;

            startButton.Text = "Start";
            startButton.Size = buttonSize;
            startButton.Location = new Point(xOffset, yOffset);
            startButton.Click += StartButton_Click;

            mixButton.Text = "Mix";
            mixButton.Size = buttonSize;
            mixButton.Location = new Point(xOffset, startButton.Bottom + yOffset);
            mixButton.Click += MixButton_Click;

            backButton.Text = "Back";
            backButton.Size = buttonSize;
            backButton.Location = new Point(xOffset, mixButton.Bottom + yOffset);
            backButton.Click += BackButton_Click;

            label1.Text = "Elements in array:\n" + array.Length;
            label1.TextAlign = ContentAlignment.TopCenter;
            label1.Size = buttonSize;
            label1.Location = new Point(xOffset, backButton.Bottom + yOffset/4);
            label1.BorderStyle = BorderStyle.FixedSingle;

            label2.Text = "Comparisons:\n" + comparisons;
            label2.TextAlign = ContentAlignment.TopCenter;
            label2.Size = buttonSize;
            label2.Location = new Point(xOffset, label1.Bottom + yOffset / 4);
            label2.BorderStyle = BorderStyle.FixedSingle;

            label3.Text = "Swaps:\n" + swaps;
            label3.TextAlign = ContentAlignment.TopCenter;
            label3.Size = buttonSize;
            label3.Location = new Point(xOffset, label2.Bottom + yOffset / 4);
            label3.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(startButton);
            Controls.Add(mixButton);
            Controls.Add(backButton);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label3);
        }

        //Обработчики нажатия кнопок
        private void StartButton_Click(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 10;
            startButton.Enabled = false;
            //Удаляю обработчик StaticPaintArray, так как у каждого алгоритма будет свой обработчик события Paint
            Paint -= StaticPaintArray;
            //Каждый алгоритм настраивает свой обработчик события Tick и события Paint
            algorithms[algorithmName].Invoke();
            //timer.Start();
        }
        private void MixButton_Click(object sender, EventArgs e)
        {
            /*
             * Делаю кнопку старта юзабельной
             * Обнуляю количество операций
             * Создаю новвый массив
             */

            comparisons = 0;
            swaps = 0;
            label2.Text = "Comparisons:\n" + comparisons;
            label3.Text = "Swaps:\n" + swaps;
            if (timer != null)
                timer.Stop();
            array = InitializeArray(array.Length);
            if (ArrayIsAlreadySorted())
                startButton.Enabled = false;
            else
                startButton.Enabled = true;
            //Отрисовываю массив серым цветом, обновляю экран и затем открепляю обработчик StaticPaintArray,
            //чтобы при последовательных нажатиях на кнопку Mix, внутренний контейнер обработчиков событий Paint не переполнился.
            Paint += StaticPaintArray;
            Refresh();
            Paint -= StaticPaintArray;
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (timer != null)
                timer.Stop();
            Close();
        }

        private int[] InitializeArray(int arraySize)
        {
            var result = new int[arraySize];
            var upBorder = sizeForArray.Height;
            var lowBorder = 0;
            var length = upBorder - lowBorder;
            var step = length / arraySize;
            var rnd = new Random();

            //Заполняю массив случайными уникальными числами
            for(int i = 0; i<arraySize;)
            {
                var newVal = rnd.Next(1, arraySize + 1) * step;
                var exist = false;
                for(int j = 0; j < i; j++)
                {
                    if (result[j] == newVal)
                    {
                        exist = true;
                        break;
                    }
                }
                if(!exist)
                {
                    result[i] = newVal;
                    i++;
                }
            }

            //Изменяю размер окна, чтобы не было пустых пространства в области визуализации массива
            int max = result.Max();
            int newHeight = max;
            int newWidth = (sizeForArray.Width / arraySize) * arraySize;
            sizeForArray = new Size(newWidth, newHeight);
            sizeForButtons = new Size(200, newHeight);
            ClientSize = new Size(sizeForArray.Width + sizeForButtons.Width, sizeForArray.Height);

            return result;
        }

        #region Algorithms

        private bool first;
        private void MSDRadixSort()
        {
            first = true;
            var index = 0;
            var maxDigit = GetMaxDigit();
            MSDRadixSort(array, maxDigit, ref index);

            Paint += StaticPaintArray;
            Refresh();
        }
        private void MSDRadixSort(int[] localArray, int k, ref int index)
        {
            first = true;
            int j = index;
            Paint += DynamicPaintArray;
            var container = new List<Queue<int>>();
            for (int l = 0; l < 10; l++)
                container.Add(new Queue<int>());

            foreach (var e in localArray)
            {
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);
                var r = (int)(e / Math.Pow(10, k)) % 10;
                container[r].Enqueue(e);
                j++;
            }
            j = index;
            if(first)
            {
                first = false;
                foreach (var q in container)
                {
                    var list = q.ToList();
                    while (list.Count > 0)
                    {
                        Refresh();
                        System.Threading.Thread.Sleep(timer.Interval);
                        array[j] = list[0];
                        list.RemoveAt(0);
                        j++;
                    }
                }
            }
            --k;
            j = index;
            foreach (var q in container)
            {
                if (q.Count > 1)
                {
                    MSDRadixSort(q.ToArray(), k, ref index);
                }
                else if (q.Count == 1)
                {
                    Refresh();
                    System.Threading.Thread.Sleep(timer.Interval);
                    array[index] = q.Dequeue();
                    index++;
                }
                j++;
            }
            Paint -= DynamicPaintArray;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int m = 0; m < array.Length; m++)
                {
                    var b = Brushes.Gray;
                    if (m == j)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(m * w, sizeForArray.Height - array[m], w, array[m]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(m * w, sizeForArray.Height - array[m], w, array[m]));
                }
            }
        }

        private void LSDRadixSort()
        {
            int arrayAccesses = 0;
            int j = 0;
            Paint += DynamicPaintArray;
            var container = new List<Queue<int>>();
            for (int i = 0; i < 10; i++)
                container.Add(new Queue<int>());

            //определяем максимальный разряд начиная с 0
            var maxDigit = GetMaxDigit();

            for(int k = 0; k<= maxDigit; k++)
            {
                j = 0;
                foreach(var e in array)
                {
                    Refresh();
                    System.Threading.Thread.Sleep(timer.Interval);
                    var r = (int)(e / Math.Pow(10, k)) % 10;
                    container[r].Enqueue(e);
                    j++;
                    arrayAccesses++;
                }
                j = 0;
                foreach(var q in container)
                {
                    while(q.Count>0)
                    {
                        Refresh();
                        System.Threading.Thread.Sleep(timer.Interval);
                        array[j] = q.Dequeue();
                        j++;
                        arrayAccesses++;
                    }
                }
            }
            Paint -= DynamicPaintArray;

            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;
                //label3.Text = "Array accesses:\n" + arrayAccesses;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == j)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private int GetMaxDigit()
        {
            var max = array.Max();
            int i = -1;
            while(max>0)
            {
                max = max / 10;
                i++;
            }
            return i;
        }

        //Происходит построение бинарной кучи с максимальным элементов в корне.
        //Куча строится на месте, путем перестраивания исходного массива, без выделения дополнительной памяти.
        private void HeapSort()
        {
            comparisons = 0;
            swaps = 0;
            int i = 0;
            Paint += DynamicPaintArray1;
            //по окончанию этого цикла, массив будет перестроен в бинарную кучу
            for (i = 0; i<array.Length; i++)
            {
                //первый элемент встает в корень
                if (i == 0)
                    continue;
                //каждый элемент, кроме первого, проталкивается наверх, если он оказался больше своего родителя
                HeapifyUp(i);
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Paint -= DynamicPaintArray1;

            var heapSize = array.Length - 1;
            Paint += DynamicPaintArray2;
            //в этом цикле происходит следующее:
            //первый элемент в массиве (то есть корневой(максимальный) элемент в кучи) меняется местами с последним 
            //и, элемент, оказавшийся в корне проталкивается вниз по кучи.
            //На каждой итерации уменьшается размер кучи.
            for (i = 0; i<array.Length - 1; i++)
            {
                swaps++;
                Swap(ref array[0], ref array[array.Length - 1 - i]);
                heapSize--;
                HeapifyDown(0, heapSize);
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Paint -= DynamicPaintArray2;

            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray1(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == i)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
            void DynamicPaintArray2(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == 0 || k == heapSize)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private void HeapifyDown(int index, int heapSize)
        {
            Paint += DynamicPaintArray;
            Refresh();
            System.Threading.Thread.Sleep(timer.Interval);
            Paint -= DynamicPaintArray;
            if (heapSize == 0)
                return;
            var indexLeft = 2 * index + 1;
            var indexRight = 2 * index + 2;
            if (indexLeft > heapSize)
                return;
            if(indexRight > heapSize)
            {
                comparisons++;
                if (array[indexLeft] > array[index])
                {
                    Swap(ref array[indexLeft], ref array[index]);
                    swaps++;
                }
                return;
            }
            var indexMax = indexRight;
            comparisons += 2;
            if (array[indexLeft] > array[indexRight])
                indexMax = indexLeft;
            if(array[indexMax] > array[index])
            {
                swaps++;
                Swap(ref array[indexMax], ref array[index]);
                HeapifyDown(indexMax, heapSize);
            }
            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == index)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private void HeapifyUp(int index)
        {
            int parentIndex = FindParentIndex(index);
            Paint += DynamicPaintArray;
            comparisons++;
            if (array[parentIndex] >= array[index])
            {
                Paint -= DynamicPaintArray;
                return;
            }
            Refresh();
            System.Threading.Thread.Sleep(timer.Interval);
            swaps++;
            Swap(ref array[parentIndex], ref array[index]);
            if(parentIndex != 0)
            {
                Paint -= DynamicPaintArray;
                HeapifyUp(parentIndex);
            }
            Paint -= DynamicPaintArray;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == index || k == parentIndex)
                        b = Brushes.Red;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private int FindParentIndex(int index)
        {
            if (index % 2 == 0)
                return (index - 2) / 2;
            return (index - 1) / 2;
        }

        //Можно подумать над более красивой визуализацией 
        //и, может быть, над вычислением количества операций
        private void TreeSort()
        {
            comparisons = 0;
            swaps = 0;
            //Colors - окрашивает одинаковы цветом элементы, находящиеся на одном уровне в дереве,
            //по порядку цветов радуги. То есть, красным цветом обозначен корневой элемент (он один), оранжевым
            //цветом обозначены два элемента ниже корневого и так далее.
            //Все уровни ниже 6го (если начинать нумерацию с 0 для корня) окрашены в фиолетовый цвет.
            var colors = new List<Color>();
            var tree = MakeTree(array, colors);
            TreeSort(tree, colors);
        }
        private void TreeSort(Tree tree, List<Color> colors)
        {
            int i = 0;
            Paint += DynamicPaintArray;
            foreach (var e in tree.VisitTree())
            {
                comparisons += e.Level;
                array[i] = e.Data;
                colors[i] = e.Color;
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);

                i++;
            }
            Paint -= DynamicPaintArray;

            Paint += DynamicPaintArray2;
            for (i = 0; i<array.Length; i++)
            {
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Paint -= DynamicPaintArray2;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = new SolidBrush(colors[k]);
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }

            void DynamicPaintArray2(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k > i)
                        b = new SolidBrush(colors[k]);
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private Tree MakeTree(int[] array, List<Color> colors)
        {
            var tree = new Tree();
            int i = 0;
            //var colors = new List<Color>();
            Paint += DynamicPaintArray;
            for (i = 0; i < array.Length; i++)
            {
                colors.Add(tree.Add(new TreeNode(array[i])));
                Refresh();
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Paint -= DynamicPaintArray;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if(k<=i)
                    {
                        b = new SolidBrush(colors[k]);
                    }
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }

            return tree;
        }

        //Сейчас реализовано разбиение Ломуто
        //Можно добавить разбиение Хоара
        private void QuickSort()
        {
            comparisons = 0;
            swaps = 0;
            QuickSort(array, 0, array.Length - 1);
            Paint += StaticPaintArray;
            Refresh();
        }
        private void QuickSort(int[] array, int start, int end)
        {
            if (start == end) return;
            int i = start;
            //Выбираем рандомный опорный элемент
            //var rnd = new Random();
            //var r = rnd.Next(start, end + 1);
            //Swap(ref array[end], ref array[r]);

            //Выбираем опорный элемент - последним
            var pivot = array[end];
            var storeIndex = start;
            Paint += DynamicPaintArray;
            for(i = start; i<=end-1; i++)
            {
                Refresh();
                if(array[i]<=pivot)
                {
                    Swap(ref array[i], ref array[storeIndex]);
                    storeIndex++;
                    swaps++;
                }
                comparisons++;
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Swap(ref array[end], ref array[storeIndex]);
            swaps++;
            if (storeIndex > start) QuickSort(array, start, storeIndex - 1);
            if (storeIndex < end) QuickSort(array, storeIndex + 1, end);
            Paint -= DynamicPaintArray;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == i || k == storeIndex)
                        b = Brushes.Red;
                    else if (array[k] == pivot)
                        b = Brushes.Green;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        
        private void MergeSort()
        {
            comparisons = 0;
            swaps = 0;
            var tempArray = new int[array.Length];
            MergeSort(array, 0, array.Length - 1, tempArray);
            Paint += StaticPaintArray;
            Refresh();
        }
        private void MergeSort(int[] array, int start, int end, int[] tempArray)
        {
            if (start == end) return;
            int middle = (end + start) / 2;
            MergeSort(array, start, middle, tempArray);
            MergeSort(array, middle + 1, end, tempArray);
            Merge(array, start, middle, end, tempArray);
        }
        private void Merge(int[] array, int start, int middle, int end, int[] tempArray)
        {
            int left = start;
            int right = middle + 1;
            int length = end - start + 1;
            int i = 0;
            Paint += DynamicPaintArray;
            for (i = 0; i < length; i++)
            {
                Refresh();
                if (right > end || (left <= middle && array[left] < array[right]))
                {
                    tempArray[i] = array[left];
                    left++;
                }
                else
                {
                    tempArray[i] = array[right];
                    right++;
                }
                System.Threading.Thread.Sleep(timer.Interval);
                comparisons++;
            }
            for (i = 0; i < length; i++)
            {
                Refresh();
                array[i + start] = tempArray[i];
                System.Threading.Thread.Sleep(timer.Interval);
            }
            Paint -= DynamicPaintArray;

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == i + start)
                        b = Brushes.Red;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }

        //Можно подумать над более красивой визуализацией
        private void ShellSort()
        {
            //Такая же сортировка вставками, только добавляется параметр step.
            //При step==1 превращается в сортировку вставками
            comparisons = 0;
            swaps = 0;
            int step = array.Length / 2;
            var i = 0;
            var j = 0;
            Paint += DynamicPaintArray;
            for (step = array.Length / 2; step > 0; step /= 2)
            {
                for(i=step; i<array.Length; i++)
                {
                    for(j=i-step; j>=0 && array[j]>array[j+step]; j-=step)
                    {
                        comparisons++;
                        Refresh();
                        Swap(ref array[j], ref array[j + step]);
                        swaps++;
                        System.Threading.Thread.Sleep(timer.Interval);
                    }
                    comparisons++;
                }
            }
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == j+1)
                        b = Brushes.Red;
                    else if (k == i || k==i-step)
                        b = Brushes.Yellow;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private void InsertionSort()
        {
            comparisons = 0;
            swaps = 0;
            var i = 0;
            var j = 0;
            Paint += DynamicPaintArray;
            for(i = 1; i<array.Length; i++)
            {
                for(j = i - 1; j >= 0; j--)
                {
                    Refresh();
                    comparisons++;
                    if (array[j+1] < array[j])
                    {
                        Swap(ref array[j + 1], ref array[j]);
                        swaps++;
                    }
                    else
                        break;
                    System.Threading.Thread.Sleep(timer.Interval);
                }
            }
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == j + 1)
                        b = Brushes.Red;
                    else if (k == i)
                        b = Brushes.Yellow;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }

        }
        private void SelectionSort()
        {
            comparisons = 0;
            swaps = 0;
            var min = 0;
            var i = 0;
            var j = 0;
            Paint += DynamicPaintArray;
            for(i = 0; i<array.Length-1; i++)
            {
                min = i;
                for(j = i+1; j<array.Length; j++)
                {
                    Refresh();
                    if (array[j] < array[min])
                        min = j;
                    System.Threading.Thread.Sleep(timer.Interval);
                    comparisons++;
                }
                Swap(ref array[i], ref array[min]);
                swaps++;
            }
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == j)
                        b = Brushes.Yellow;
                    else if (k == min || k == i)
                        b = Brushes.Red;
                    else if (k < i)
                        b = Brushes.Green;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }

        }
        private void CombSort()
        {
            comparisons = 0;
            swaps = 0;
            var K = 1.3;
            var jump = array.Length;
            var i = 0;
            bool isSorted = false;
            Paint += DynamicPaintArray;
            #region Самая быстрая реализация и самая неустойчивая
            //while (!isSorted && jump > 1)
            //{
            //    if (jump > 1)
            //        jump = (int)(jump / K);
            //    isSorted = true;
            //    for (i = 0; i <= array.Length - jump - 1; i++)
            //    {
            //        Refresh();
            //        if (array[i] > array[i + jump])
            //        {
            //            Swap(ref array[i], ref array[i + jump]);
            //            isSorted = false;
            //        }
            //        System.Threading.Thread.Sleep(timer.Interval);
            //        operationsCount++;
            //    }
            //}
            #endregion
            #region 50 на 50
            //while (!isSorted && jump>1)
            //{
            //    if (jump > 1)
            //        jump = (int)(jump / K);
            //    isSorted = true;
            //    for (i = 0; i <= array.Length - jump - 1; i++)
            //    {
            //        Refresh();
            //        if (array[i] > array[i + jump])
            //        {
            //            Swap(ref array[i], ref array[i + jump]);
            //            isSorted = false;
            //        }
            //        System.Threading.Thread.Sleep(timer.Interval);
            //        operationsCount++;
            //    }
            //    if (isSorted)
            //    {
            //        for (i = 0; i <= array.Length - 2; i++)
            //            if (array[i] > array[i + 1])
            //                isSorted = false;
            //    }
            //}
            #endregion
            #region Самая медленная и самая устойчивая
            while (!isSorted)
            {
                if (jump > 1)
                    jump = (int)(jump / K);
                else
                {
                    isSorted = true;
                }
                for (i = 0; i <= array.Length - jump - 1; i++)
                {
                    Refresh();
                    if (array[i] > array[i + jump])
                    {
                        swaps++;
                        Swap(ref array[i], ref array[i + jump]);
                        isSorted = false;
                    }
                    comparisons++;
                    System.Threading.Thread.Sleep(timer.Interval);
                }
            }
            #endregion
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == i || k == i + jump)
                    {
                        if (array[i] > array[i + jump])
                            b = Brushes.Red;
                        else
                            b = Brushes.Yellow;
                    }
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private void CoctailSort()
        {
            #region Реализация с помощью таймера
            //var begin = 0;
            //var end = array.Length - 1;
            //var isSorted = true;
            //var i = begin;
            //var j = end - 1;
            //timer.Tick += (sender, args) =>
            //{
            //    //if (isSorted)
            //    //{
            //    //    Paint -= DynamicPaintArray;
            //    //    Paint += StaticPaintArray;
            //    //    Refresh();
            //    //    timer.Stop();
            //    //}

            //    if (i!=end)
            //    {
            //        //isSorted = true;
            //        if (array[i] > array[i + 1])
            //        {
            //            Swap(ref array[i], ref array[i + 1]);
            //            isSorted = false;
            //        }
            //        Refresh();
            //        i++;
            //    }
            //    else
            //    {
            //        if (isSorted)
            //        {
            //            Paint -= DynamicPaintArray;
            //            Paint += StaticPaintArray;
            //            Refresh();
            //            timer.Stop();
            //        }
            //        //isSorted = true;
            //        if (array[j] < array[j - 1])
            //        {
            //            Swap(ref array[j], ref array[j + 1]);
            //            isSorted = false;
            //        }
            //        Refresh();
            //        j--;

            //        if(j == begin)
            //        {
            //            begin++;
            //            end--;
            //            i = begin;
            //            isSorted = true;
            //            j = end - 1;
            //        }
            //    }
            //    operationsCount++;
            //};
            //Paint += DynamicPaintArray;
            //void DynamicPaintArray(object sender, PaintEventArgs e)
            //{
            //    label2.Text = "Operations count:\n" + operationsCount;

            //    var g = e.Graphics;
            //    g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

            //    int w = sizeForArray.Width / array.Length;
            //    for (int k = 0; k < array.Length; k++)
            //    {
            //        var b = Brushes.Gray;
            //        if (k == begin || k == end)
            //            b = Brushes.Black;
            //        else
            //            b = Brushes.Gray;
            //        //if (k == j)
            //        //    b = Brushes.Red;
            //        //else
            //        //    b = Brushes.Gray;
            //        g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
            //        g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
            //    }
            //}
            #endregion

            comparisons = 0;
            swaps = 0;
            var begin = -1;
            var end = array.Length - 2;
            var isSorted = false;
            var i = 0;
            Paint += DynamicPaintArray;
            while (!isSorted)
            {
                begin++;
                isSorted = true;
                for(i = begin; i<=end; i++)
                {
                    Refresh();
                    if (array[i] > array[i + 1])
                    {
                        swaps++;
                        Swap(ref array[i], ref array[i + 1]);
                        isSorted = false;
                    }
                    comparisons++;
                    System.Threading.Thread.Sleep(timer.Interval);
                }
                if (isSorted)
                    break;
                isSorted = true;
                end--;
                for(i = end; i>=begin; i--)
                {
                    if (array[i] > array[i + 1])
                    {
                        swaps++;
                        Swap(ref array[i], ref array[i + 1]);
                        isSorted = false;
                    }
                    comparisons++;
                    Refresh();
                    System.Threading.Thread.Sleep(timer.Interval);
                }
            }
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();

            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k < begin || k > end+1)
                        b = Brushes.Green;
                    else if (k == i)
                        b = Brushes.Red;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }
        }
        private void BubbleSort()
        {
            #region Реализация с помощью таймера
            //operationsCount = 0;
            //var i = array.Length - 1;
            //var j = 0;
            //timer.Tick += (sender, args) =>
            //{
            //    if (j == i)
            //    {
            //        i--;
            //        j = 0;
            //    }
            //    Refresh();
            //    if (i == 0)
            //    {
            //        Paint -= DynamicPaintArray;
            //        Paint += StaticPaintArray;
            //        Refresh();
            //        //startButton.Enabled = true;
            //        timer.Stop();
            //    }

            //    if (array[j] > array[j + 1])
            //        Swap(ref array[j], ref array[j + 1]);
            //    j++;
            //    operationsCount++;
            //};
            #endregion

            comparisons = 0;
            swaps = 0;
            var i = array.Length - 1;
            var j = 0;
            var isSorted = false;
            Paint += DynamicPaintArray;
            for (i = array.Length - 1; i > 0 && !isSorted; i--)
            {
                isSorted = true;
                for (j = 0; j < i; j++)
                {
                    Refresh();
                    if (array[j] > array[j + 1])
                    {
                        swaps++;
                        Swap(ref array[j], ref array[j + 1]);
                        isSorted = false;
                    }
                    comparisons++;
                    System.Threading.Thread.Sleep(timer.Interval);
                }
            }
            Paint -= DynamicPaintArray;
            Paint += StaticPaintArray;
            Refresh();
            //Отрисовываю массив, при этом, ВЫДЕЛЯЮ КРАСНЫМ цветом элемент массива, который "всплывает" вверх по массиву
            void DynamicPaintArray(object sender, PaintEventArgs e)
            {
                label2.Text = "Comparisons:\n" + comparisons;
                label3.Text = "Swaps:\n" + swaps;

                var g = e.Graphics;
                g.DrawLine(new Pen(Color.Black), sizeForArray.Width, sizeForArray.Height, sizeForArray.Width, 0);

                int w = sizeForArray.Width / array.Length;
                for (int k = 0; k < array.Length; k++)
                {
                    var b = Brushes.Gray;
                    if (k == j)
                        b = Brushes.Red;
                    else if (k > i)
                        b = Brushes.Green;
                    else
                        b = Brushes.Gray;
                    g.FillRectangle(b, new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(k * w, sizeForArray.Height - array[k], w, array[k]));
                }
            }

            //Оригинальный алгоритм сортировки пузырьком

            //for(int i = array.Length-1; i>0; i--)
            //{
            //    for(int j = 0; j<i; j++)
            //    {
            //        if (array[j] > array[j + 1])
            //            Swap(ref array[j], ref array[j + 1]);
            //    }
            //}
        }
        private void Swap(ref int a, ref int b)
        {
            var t = a;
            a = b;
            b = t;
        }
        private bool ArrayIsAlreadySorted()
        {
            for(int i = 0; i<array.Length-1;i++)
            {
                if (array[i] > array[i + 1])
                    return false;
            }
            return true;
        }
        #endregion
    }
}