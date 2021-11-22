using CustomLanguage.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CustomLanguage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            Logger.Init(consoleTextBox);
            BuildMenuStrip();
        }

        private void BuildMenuStrip()
        {
            var menuStrip = new MenuStrip();

            var mainMenu = new ToolStripMenuItem("Меню");

            var valuesmenu = new ToolStripMenuItem("Переменные");
            mainMenu.DropDownItems.Add(valuesmenu);

            var intItem = new ToolStripMenuItem("ПЕРЕМЕННАЯ ЧИСЛО - объявление переменной типа int");
            intItem.Click += new EventHandler(EnterValue);
            valuesmenu.DropDownItems.Add(intItem);

            var strItem = new ToolStripMenuItem("ПЕРЕМЕННАЯ СТРОКА - объявление переменной типа string");
            strItem.Click += new EventHandler(EnterValue);
            valuesmenu.DropDownItems.Add(strItem);

            var operations = new ToolStripMenuItem("Вычисления");
            mainMenu.DropDownItems.Add(operations);

            var operationsSumItem = new ToolStripMenuItem("СУМ - Сумма");
            operationsSumItem.Click += new EventHandler(EnterCommand);
            operations.DropDownItems.Add(operationsSumItem);

            var operationsRaznItem = new ToolStripMenuItem("РАЗН - Разность");
            operationsRaznItem.Click += new EventHandler(EnterCommand);
            operations.DropDownItems.Add(operationsRaznItem);

            var operationsMulItem = new ToolStripMenuItem("УМН - Умножение");
            operationsMulItem.Click += new EventHandler(EnterCommand);
            operations.DropDownItems.Add(operationsMulItem);

            var operationsStrLenItem = new ToolStripMenuItem("ДЛИНСТР - Длина строки");
            operationsStrLenItem.Click += new EventHandler(EnterCommand);
            operations.DropDownItems.Add(operationsStrLenItem);

            var operationsSubStrItem = new ToolStripMenuItem("ПОДСТР - Подстрока в строке");
            operationsSubStrItem.Click += new EventHandler(EnterCommand);
            operations.DropDownItems.Add(operationsSubStrItem);

            menuStrip.Items.Add(mainMenu);

            this.Controls.Add(menuStrip);
        }

        private void EnterValue(object sender, EventArgs e)
        {
            var element = sender as ToolStripMenuItem;
            var text = element.Text.Split(' ')[0] + " " + element.Text.Split(' ')[1];
            var fulltext = "";
            fulltext = "\r\n" + text;
            if (inputTextBox.Text.Length == 0)
            {
                fulltext = text;
            }
            inputTextBox.Text += fulltext;
        }

        private void EnterCommand(object sender, EventArgs e)
        {
            var element = sender as ToolStripMenuItem;
            var text = element.Text.Split(' ')[0];
            
            text = "\r\nВЫВОД " + element.Text.Split(' ')[0]; ;
            if (inputTextBox.Text.Length == 0)
            {
                text = "ВЫВОД " + element.Text.Split(' ')[0]; ;
            }
            inputTextBox.Text += text;
        }

        void Compile()
        {
            var code = inputTextBox.Text;
            AnalyzeSyntax(code);
        }

        
        void AnalyzeSemantic(string code)
        {
            
            int rowIndex = 0;
            bool isSuccessed = true;
            foreach (var str in code.Split('\n'))
            {
                var strTemp = str.Replace("\r", "");
                strTemp = strTemp.Replace(";", "");
                rowIndex++;
                if (!Match(strTemp))
                {
                    isSuccessed = false;
                    Logger.Log("Syntax error in line " + rowIndex + ": Неверный синтаксис кода.");
                }
            }
            if (isSuccessed)
            {
                Logger.Log("Скомпилировано без ошибок.");
                Parse(code.Replace(";", ""));
            }
        }
        List<string> values = new List<string>();
        void Parse(string code)
        {
            values.Clear();
            int rowIndex = 0;
            bool isSuccessed = true;
            foreach (var str in code.Split('\n'))
            {
                rowIndex++;
                DoAction(str, rowIndex);
            }
        }

        string DoAction(string str,int rowIndex)
        {
            str = str.Replace("\r", "");
            var strArr = str.Split(' ');
            if (GetTypeOfWord(strArr[0]) == "command")
            {
                Logger.Log(DoAction(string.Join(" ", strArr.Skip(1)), rowIndex));
                return "";
            }
            if (GetTypeOfWord(strArr[0]) == "special")
            {
                var val = DoAction(string.Join(" ", strArr.Skip(1)), rowIndex);
                if(values.Count == 0)
                {
                    values.Add(val);
                    return "";
                }
                foreach(var tmp in values)
                {
                    if (val.Split(',')[1] == tmp.Split(',')[1])
                    {
                        Logger.Log("Error in line " + rowIndex + ": Переменная с именем " + val.Split(',')[1] + " уже существует");
                        return "";
                    }
                    
                    
                }
                values.Add(val);
                return "";
            }
            if (GetTypeOfWord(strArr[0]) == "operation")
            {

                switch (strArr[0])
                {
                    case "СУМ":
                        {
                            int num1;
                            int num2;
                            string numStr1 = DoAction(strArr[1], rowIndex);
                            string numStr2 = DoAction(strArr[3], rowIndex);
                            bool succes1 = int.TryParse(numStr1, out num1);
                            bool succes2 = int.TryParse(numStr2, out num2);
                            if (!succes1 || !succes2)
                            {
                                return "Error in line " + rowIndex + ": Применение арифметической операции к нечисловым типам переменных.\n" + numStr1;
                            }
                            return (num1 + num2).ToString();
                        }
                    case "РАЗН":
                        {
                            int num1;
                            int num2;
                            string numStr1 = DoAction(strArr[1], rowIndex);
                            string numStr2 = DoAction(strArr[3], rowIndex);
                            bool succes1 = int.TryParse(numStr1, out num1);
                            bool succes2 = int.TryParse(numStr2, out num2);
                            if (!succes1 || !succes2)
                            {
                                return "Error in line " + rowIndex + ": Применение арифметической операции к нечисловым типам переменных.\n" + numStr1;
                            }
                            return (num1 - num2).ToString();
                        }
                    case "УМН":
                        {
                            int num1;
                            int num2;
                            string numStr1 = DoAction(strArr[1], rowIndex);
                            string numStr2 = DoAction(strArr[3], rowIndex);
                            bool succes1 = int.TryParse(numStr1, out num1);
                            bool succes2 = int.TryParse(numStr2, out num2);
                            if (!succes1 || !succes2)
                            {
                                return "Error in line " + rowIndex + ": Применение арифметической операции к нечисловым типам переменных.\n" + numStr1;
                            }
                            return (num1 * num2).ToString();
                        }
                    case "ДЛИНСТР":
                        {
                            var strret = DoAction(strArr[1], rowIndex);
                            if (strret.Count(x => x == '\'') == 2)
                            {
                                return strret.Replace("'", "").Length.ToString();
                            }
                            return "Error in line " + rowIndex + ": Применение строковой операции к нестроковым типам переменных.\n";
                        }
                    case "ПОДСТР":
                        {
                            var strret = DoAction(strArr[1], rowIndex);
                            var substr = strArr[2].Replace("'", "");
                            var countSubstr = Regex.Matches(strret, substr).Count;
                            if (countSubstr >= 1)
                            {
                                var tmpStr = "";
                                for (int i = 0; i < countSubstr; i++)
                                {
                                    tmpStr += substr;
                                }
                                return tmpStr;
                            }
                            return "Подстроки " + substr + " нет в строке " + strret;


                        }
                



                }
                
                return "";
            }
            if (GetTypeOfWord(strArr[0]) == "type")
            {
                return strArr[0] + "," + DoAction(string.Join(" ", strArr.Skip(1)), rowIndex);
                
            }
            if (GetTypeOfWord(strArr[0], "name") == "name")
            {
                if (strArr.Length > 1)
                {
                    return strArr[0] + "," + strArr[2];
                }
                if(values.Count == 0)
                {
                    return "Error in line " + rowIndex + ": Переменная не существует в данном контексте";
                }
                foreach (var val in values)
                {
                    if (val.Split(',')[1] == strArr[0])
                    {

                        return val.Split(',')[2].Replace("\r","");
                    }
                    
                }
                return "Error in line " + rowIndex + ": Переменная не существует в данном контексте";
            }
            return "";
        }

        string[] commands = { "ВЫВОД" };
        string[] specials = { "ПЕРЕМЕННАЯ" };
        string[] operations = { "ДЛИНСТР", "ПОДСТР", "СУМ", "РАЗН", "УМН" };
        string[] types = { "ЧИСЛО", "СТРОКА" };
        string[] patterns = {       "special type name = value" ,//ШАБЛОН СОЗДАНИЯ ЧИСЛА
                                    "special type name = 'value'",//ШАБЛОН СОЗДАНИЯ СТРОКИ
                                    "command operation name и name",//ШАБЛОН АРИФМ ОПЕРАЦИЙ
                                    "command operation name", // ШАБЛОН ОПЕРАЦИИ ДЛИНЫ СТРОКИ
                                    "command name", // ШАБЛОН ВЫВОДА ПЕРЕМЕННОЙ
                                    "command operation name 'value'"}; //ПОДСТРОКА ШАБЛОН


        string GetTypeOfWord(string word, string currentPatternWord = "")
        {
            if (currentPatternWord == "и")
            {
                return "и";
            }
            if (currentPatternWord == "name")
            {
                return "name";
            }
            if (currentPatternWord == "value")
            {
                return "value";
            }
            if (currentPatternWord == "'value'")
            {
                return "'value'";
            }
            if (word == "=")
            {
                return "=";
            }
            if (commands.Contains(word))
            {
                return "command";
            }
            if (specials.Contains(word))
            {
                return "special";
            }
            if (operations.Contains(word))
            {
                return "operation";
            }
            if (types.Contains(word))
            {
                return "type";
            }
            return "NotFound";
        }

        bool Match(string codeLine)
        {
            int i = 0;
            var codeWords = codeLine.Split(' ');
            bool isMatched = true;
            foreach (var pattern in patterns)
            {
                
                var patternWords = pattern.Split(' ');
                if (patternWords.Length != codeWords.Length)
                {
                    isMatched = false;
                    continue;
                }
                isMatched = true;
                foreach (var patternSingle in patternWords)
                {
                    var codeWordType = GetTypeOfWord(codeWords[i], patternSingle);
                    if (!string.Equals(patternSingle, codeWordType))
                    {
                        isMatched = false;
                        break;
                    }
                    i++;
                }
                i=0;
                if (isMatched)
                {
                    return isMatched;
                }
            }
            return isMatched;
        }

        void AnalyzeSyntax(string code)
        {
            int rowIndex = 0;
            bool isSuccessed = true;
            foreach(var str in code.Split('\n'))
            {
                var strTemp = str.Replace("\r","");
                rowIndex++;
                if (string.IsNullOrWhiteSpace(strTemp) && rowIndex ==1)
                {
                    Logger.Log("Syntax error in line " + rowIndex + ": Файл не может быть пустым");
                    isSuccessed = false;
                    break;
                }
                if (string.IsNullOrWhiteSpace(strTemp) && rowIndex >= 1)
                {
                    continue;
                }
                if (strTemp.EndsWith(";") && strTemp.Length < 2)
                {
                    Logger.Log("Syntax error in line " + rowIndex + ": Лишний специальный символ ';'.");
                    isSuccessed = false;
                }
                if (strTemp.Count(x=>x == ';') >= 2)
                {
                    Logger.Log("Syntax error in line "+ rowIndex +": В строке может содержаться только одна строчка кода.");
                    isSuccessed = false;
                }
                if (!strTemp.EndsWith(";"))
                {
                    Logger.Log("Syntax error in line " + rowIndex + ": Требуется ';'.");
                    isSuccessed = false;
                }
            }
            if (isSuccessed)
            {
                
                AnalyzeSemantic(code);
            }
        }        

        private void OnCompileButtonPressed(object sender, EventArgs e)
        {
            Compile();
        }
    }
}
