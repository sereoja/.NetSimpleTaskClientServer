using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Creating_Starting_Tasks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //start new Tasks
            //Action action1 =  () =>
            //{

            //};
            //Task task1 = Task.Factory.StartNew(action1);

            //turn into a single statement
            Task task1 = Task.Factory.StartNew(() =>
            {
                
                int counter = 0;
                Random rand = new Random();
                while (counter <= 100)
                {
                    int x = rand.Next();
                    if (x % 3 == 0)
                    {
                        counter++;
                        string data = String.Format(
                            "ID: {0,-10} x: {1}",
                            Thread.CurrentThread.ManagedThreadId, x);
                        // richTextBox1.AppendText(data + "\n");
                        SetText(data, richTextBox1);
                    }
                    Thread.Sleep(30);
                }
            });

            Task task2 = Task.Factory.StartNew(() =>
            {
                int counter = 0;
                Random rand = new Random();
                while (counter <= 100)
                {
                    int x = rand.Next(int.MinValue, 0);
                    if (x % 3 == 0)
                    {
                        counter++;
                        string data = String.Format
                        ("ID: {0,-10} x: {1}",
                            Thread.CurrentThread.ManagedThreadId, x);
                        //richTextBox2.AppendText(data);
                        SetText(data, richTextBox2);
                    }
                    Thread.Sleep(30);
                }
            });
        }
        //method to make thread safe calls from tasks
        //to access a control in the UI thread
        private void SetText(string text,RichTextBox rtbox)
        {
            if(rtbox.InvokeRequired)
            {
                //use a delegate type with same signature
                //as your SetText. You can define your own
                //delegate type, or use the predefined 
                //Action<string,RichTextBox> delegate type
                Action<string, RichTextBox> action = SetText;
                Invoke(action, text, rtbox);
            }
            else
            {
                rtbox.AppendText(text + "\n");
                rtbox.ScrollToCaret();
            }
        }
    }
}
///Lab Assignment Due 10/06
///Add another task3 that computes the average of 10,000,000
///random values, the display the result to a textbox
///
///Add a fourth task4 that computes the average of 
///10,000,000 random negative odd numbers
///Display result to an other text box
