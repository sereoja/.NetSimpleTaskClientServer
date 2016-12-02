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

namespace Task_Cancellation
{
    /*
     * Task Cancellation
     * =================
     * Check msdn documentatio for task cancellation
     * http://msdn.microsoft.com/en-us/library/dd997396(v=vs.110).aspx
     * 
     * Step 1: Create a CancellationTokenSource object (cts)
     *         This object (cts) defines a property Token 
     *                              (of type CancellationToken)
     *         It also defines a method Cancel()
     *         
     * Step 2: To cancel a task (from a button) all we have to do is call the
     * Cancel method on the object cts.  cts.Cancel()
     * 
     * Step 3: The Task uses the token (this token is passed to the task)
     *         to find out if anybody has requested a cancellation
     *         if so, it may cancel itself.
     *         
     * Since Cancellation is designed to be cooperative, it is up to 
     * the Task to decide whether to cancel or not.
     */
    public partial class Form1 : Form
    {
        CancellationTokenSource cts = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();

            //Action action = () =>
            //{
            //    for(int n=1000000; n<2000000; n++)
            //    {
            //        if (n%3==0 && n%7==0)
            //        {
            //            SetText(n.ToString(), listBox1);
            //        }
            //    }
            //};
            //Task task = Task.Factory.StartNew(action, cts.Token);
            //or in a single statement
            Task task = Task.Factory.StartNew(
               () =>
               {
                   for (int n = 1000000; n < 1500000; n++)
                   {
                       if (cts.Token.IsCancellationRequested)
                       {
                           //do clean up. close resources
                           cts.Token.ThrowIfCancellationRequested();
                       }

                       if (n % 3 == 0 && n % 7 == 0)
                       {
                           SetText(n.ToString(), listBox1);
                       }
                   }
               }, cts.Token);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (cts != null)
                cts.Cancel(); //Request to cancel task
            //once the Cancel() method is called, the 
            //cts.Token.IsCancellationRequested becomes true, which
            //is checked by the task
            
        }
        //cross threading method
        private void SetText(string s,ListBox listbox)
        {
            if (listbox.InvokeRequired)
            {
                Action<string,ListBox> action = SetText;
                listBox1.Invoke(action, s,listbox);
            }
            else
            {
                listBox1.Items.Add(s);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }
    }
}
