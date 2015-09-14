using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AsynchronousTasks.Resources;
using System.Threading.Tasks;

namespace AsynchronousTasks
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Action> todoList = new List<Action>();
        List<StackPanel> UIElements = new List<StackPanel>();

        /// <summary>
        /// Class constructor
        /// </summary>
        public MainPage() {
            InitializeComponent();

            // Setup UI Elements
            UIElements.Add(Task1Panel);
            UIElements.Add(Task2Panel);
            UIElements.Add(Task3Panel);
            UIElements.Add(Task4Panel);

            // Setup dummy tasks
            todoList.Add(() => boringTask(0));
            todoList.Add(() => boringTask(1));
            todoList.Add(() => boringTask(2));
            todoList.Add(() => boringTask(3));
        }

        /// <summary>
        /// This routine will simulate the long running task. 
        /// This routine is going to run asynchronously using the Async
        /// mechanism on a separate thread. We'll use a dispatcher to
        /// switch back to the UI thread to update individual progress
        /// bars for each task item
        /// </summary>
        /// <param name="index"></param>
        private void boringTask(int index) {
            for (int i = 0; i < 100; i++) {

                //this loop will run off the UI thread and simulates the 
                //long boring task that needs to be peformed
                for (int j = 0; j < 10000000; j++) ;

                //Now that the boring task is over, lets update the progress
                //for this task by switching to the UI thread
                this.Dispatcher.BeginInvoke(new Action(()=>{
                    ProgressBar pgBar = UIElements[index].Children[3] as ProgressBar;
                    double step = 1;                
                    pgBar.Value += step;
                }));
            }
        }

        /// <summary>
        /// Routine to perform the boring tasks
        /// </summary>
        public async void performAllTasks() {
            for (int k = 0; k < todoList.Count; k++) {
                
                // pre-task UI stuff
                Image bullet = UIElements[k].Children[0] as Image;
                Image chk = UIElements[k].Children[1] as Image;
                ProgressBar pgBar = UIElements[k].Children[3] as ProgressBar;
                bullet.Visibility = System.Windows.Visibility.Visible;
                chk.Visibility = System.Windows.Visibility.Collapsed;
                
                // perform the task, wait for it to get over
                Action taskTodo = todoList[k];
                await Task.Run(()=>taskTodo());

                // post-task UI stuff
                bullet.Visibility = System.Windows.Visibility.Collapsed;
                chk.Visibility = System.Windows.Visibility.Visible;
                pgBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Triggers when the page loads. Start performing all tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e) {
            performAllTasks();
            Console.WriteLine("\n---- starting app ---\n");
        }
    }
}   