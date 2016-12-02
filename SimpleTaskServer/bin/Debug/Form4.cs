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

namespace TaskContinuation_TaskWithReturnValue
{
    public partial class Form1 : Form
    {
        /*
         Task continuation
         *****************
         It is very common to have a task1 do and complete
         some work, then pass its result to a second task2
         where task2 starts right after task1 has completed.
         That means task2 continues right after task1 completes

            Use the Task class ContinueWith method. its syntax:

 public Task ContinueWith(Action<Task> continuationAction)


         */
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //chain two tasks.
            //will start task1, then start task2 when task1
            //completes, using the ContinueWith method
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
            Task task2 = task1.ContinueWith(t =>
            {
                //the parameter t is of type Task
                //the system will call ContinueWith and pass it
                //the completed task (previous task), in this case
                //it is task1
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
        private void SetText(string text, RichTextBox rtbox)
        {
            if (rtbox.InvokeRequired)
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
        //=========================================================
        private void button3_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            richTextBox1.Text = rand.Next().ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //start a task for a method that returns a value
            //The System.Threading.Tasks library defines two classes
            ///Task and Task<TResult>. Use Task for method that returns
            ///a void.
            ///Use Task<TResult> for method or lambda expression that
            ///returns a value

            ///option 1: using named method
            Func<double> function1 = AverageValueOfRandomNumbers;
            Task<double> task1 = Task<double>.Factory.StartNew(function1);
            //to get the returned value, use the
            //Result property of the Task<TResult> class
            // double avg = task1.Result;//blocking statement
            //it will wait until the result is returned by
            //the task.
            Task task2 = task1.ContinueWith(t =>
            {
                double avg = t.Result;
                SetText("avg=" + avg, richTextBox2);
            });
            //display it
            //richTextBox1.Text = "Avg = " + avg;
            
        }
        private double AverageValueOfRandomNumbers()
        {
            Random rand = new Random();
            double total = 0;
            int counter = 0;
            for(int i=1; i <= 500000000; i++)
            {
                int x = rand.Next();
                if (x % 3 != 0)
                {
                    total += x;
                    counter++;
                }
            }
            return total / counter;
        }
    }
}

///Lab Assignment
///Create 3 tasks t1,t2,t3 have each do some work and display
///have t2 starts when t1 completes
///have t3 starts when t2 completes
///
///
///create a task for a method that returns an array of
///1000 random even values
///have second task that uses the returned array and 
///display its min, max and average values