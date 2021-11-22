using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomLanguage.Scripts
{
    public static class Logger
    {
        static TextBox consoleTextBox;

        public static void Init(TextBox textBox)
        {
            consoleTextBox = textBox;
        }

        public static void Log(string message)
        {
            consoleTextBox.Text += Environment.NewLine + DateTime.Now +">>" + message;
        }
    }
}
