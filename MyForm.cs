using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmsVisual
{
    class MyForm : Form
    {
        private Label label1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label2;
        private TextBox textBox1;
        private Button button1;

        private string[] algorithms;
        private string[] sortAlgorithms;
        //private string[] searchAlgorithms;
        private Dictionary<string, string[]> algorithmTypesToAlgorithms;

        public MyForm()
        {
            algorithms = new[] { "Sort" };//, "Search" };
            sortAlgorithms = new[] { "Bubble sort", "Coctail sort", "Comb sort", 
                "Selection sort",
                "Insertion sort",
                "Shell sort",
                "Merge sort",
                "Quick sort",
                "Tree sort",
                "Heap sort",
                "LSD Radix sort",
                "MSD Radix sort"};
            //searchAlgorithms = new[] { "Usual search", "Binary search" };
            algorithmTypesToAlgorithms = new Dictionary<string, string[]>
            {
                {"Sort", sortAlgorithms }
                //{"Search", searchAlgorithms }
            };

            label1 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            label2 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            InitializeComponents();

            //Fix size
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        //Other methods
        private void InitializeComponents()
        {
            var font = new Font("Arial", 14);

            label1.Text = "Select the algorithm";
            label1.Font = font;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            //label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            label1.Location = new Point(0, 0);

            comboBox1.Items.AddRange(algorithms);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;
            comboBox1.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            comboBox1.Location = new Point(0, label1.Bottom);
            comboBox1.SelectedIndexChanged += (sender, args) => SetupComboBox2();

            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            comboBox2.Location = new Point(0, comboBox1.Bottom);
            SetupComboBox2();

            label2.Text = "Input array size";
            label2.Font = font;
            label2.TextAlign = ContentAlignment.MiddleCenter;
            //label2.BorderStyle = BorderStyle.FixedSingle;
            label2.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            label2.Location = new Point(0, comboBox2.Bottom);

            textBox1.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            textBox1.Location = new Point(0, label2.Bottom);

            button1.Size = new Size(ClientSize.Width, ClientSize.Height / 6);
            button1.Location = new Point(0, textBox1.Bottom);
            button1.Text = "Next";
            button1.Click += Button1_Click;

            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(comboBox2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(button1);
        }
        private void SetupComboBox2()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(algorithmTypesToAlgorithms[(string)comboBox1.SelectedItem]);
            comboBox2.SelectedIndex = 0;
        }

        //Events
        private void Button1_Click(object sender, EventArgs e)
        {
            int arraySize;
            if (!int.TryParse(textBox1.Text, out arraySize))
                MessageBox.Show("Incorrect array size");
            else
            {
                if (arraySize < 2 || arraySize > 250)
                    MessageBox.Show("Incorrect array size.\nInput array size from 2 to 100.");
                else
                {
                    var s = new SecondForm(arraySize, (string)comboBox2.SelectedItem);
                    s.Show();
                }
            }
        }
    }
}
