using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Mail;
using Pop3Lib;
namespace server_1
{
    public partial class Form1 : Form
    {
        Server Serv;
        public static List<string>  Waiting_list = new List<string>();
        public static List<string> Scripts = new List<string>();
        public static List<string> fileNameScripts = new List<string>();
        Thread myThread;
        Thread myMailThread;
        public static string AUTOIT;
        public static string TMP;
        public static string NJ;
        public static string EXE;
        public static string APP;
        public string MAIL_NAME;
        public string MAIL_PASS;
        public string MAIL_SERV;
        public string MAIL_CLIE;
        public string MAIL_POP3;
        public string MAIL_PPOR;
        public string MAIL_SMTP;
        public string MAIL_SPOR;
        public bool SERVER_START = false;
        public static bool EMAIL_STOP = true;
        public static bool ANSVER_EMAIL_STOP = true;
        public static bool FORM_CLOSING = false;
        public static int port;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            toolStripMenuItem5.Enabled = true;
            toolStripMenuItem4.Enabled = true;
            button2.Enabled = true;
            button1.Enabled = false;
            toolStripMenuItem3.Enabled = false;
            SERVER_START = true;
            if (textBox2.Text != "")
                port = Convert.ToInt32(textBox2.Text.ToString());
            else
                port = Convert.ToInt32("11000");
            label3.Text = "IP для подключения " + System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            if (sender != null) textBox1.Text = "Запуск сервера..." + Environment.NewLine + textBox1.Text;
            Serv = new Server(); // Создаем новый экземпляр класса сервера
            Serv.Create(port, textBox1);

            myThread = new Thread(RunList); //Создаем новый объект потока (Thread)
            myThread.Start();
            checkBox2.Enabled = false;
            if (checkBox2.Checked)
            {
                ANSVER_EMAIL_STOP = false;
                EMAIL_STOP = false;
                myMailThread = new Thread(MailServer); //Создаем новый объект потока (Thread)
                myMailThread.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toolStripMenuItem5.Enabled = false;
            toolStripMenuItem4.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = true;
            toolStripMenuItem3.Enabled = true;
            SERVER_START = false;
            checkBox2.Enabled = true;
            try
            {
                
                textBox1.Text = "Производится остановка локального сервера..." + Environment.NewLine + textBox1.Text;
                Serv.Close(); // раз выход – значит выход. Серв-нах.
                myThread.Abort();
                textBox1.Text = "Локальный сервер остановлен..." + Environment.NewLine + textBox1.Text;
                if (!checkBox2.Checked)
                {    
                    if (sender != null) textBox1.Text = "Сервер остановлен полностью..." + Environment.NewLine + textBox1.Text;
                }
                else
                {
                    button1.Enabled = false;
                    textBox1.Text = "Производиться остановка E-Mail сервера..." + Environment.NewLine + textBox1.Text;
                    Thread StopMailThread = new Thread(STOP_EMAIL_SERVER); //Создаем новый объект потока (Thread)
                    StopMailThread.Start();
                }
            }catch(Exception err)
            {
                textBox1.Text = "\nERROR:\n" + err.ToString() + "\n" + textBox1.Text;
            }
        }

        private void STOP_EMAIL_SERVER()
        {
            EMAIL_STOP = true;
            while (!ANSVER_EMAIL_STOP)
            { }
            myMailThread.Abort();
            if (!FORM_CLOSING)
            {
                button1.Invoke(new Action<bool>((s) => button1.Enabled = s), true);
                textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "E-Mail сервер остановлен..." + Environment.NewLine + "Сервер остановлен полностью..." + Environment.NewLine + textBox1.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FORM_CLOSING = true;
            if (SERVER_START) 
                button2_Click(sender, e);
            StreamWriter outFile = new StreamWriter(File.Open("SCRIPTS.INI", FileMode.Create));
            int i = 1;
            if (Scripts.Count>0)
            {
                outFile.WriteLine(Scripts.ElementAt(0));
                outFile.Write(fileNameScripts.ElementAt(0));
            }
            while (Scripts.Count>i)
            {
                outFile.WriteLine(Environment.NewLine+Scripts.ElementAt(i));
                outFile.Write(fileNameScripts.ElementAt(i));
                ++i;
            }
            outFile.Close();
            StreamWriter set = new StreamWriter(File.Open("SETTINGS.INI", FileMode.Create));
            set.WriteLine(AUTOIT);
            set.WriteLine(NJ);
            set.WriteLine(EXE);
            set.WriteLine(TMP);
            set.WriteLine(APP);
            set.Close();
            set = new StreamWriter(File.Open("MAIL.INI", FileMode.Create));
            set.WriteLine(MAIL_NAME);
            set.WriteLine(MAIL_PASS);
            set.WriteLine(MAIL_SERV);
            set.WriteLine(MAIL_CLIE);
            set.WriteLine(MAIL_POP3);
            set.WriteLine(MAIL_PPOR);
            set.WriteLine(MAIL_SMTP);
            set.WriteLine(MAIL_SPOR);
            if (checkBox1.Checked)
                set.WriteLine("YES");
            else
                set.WriteLine("NO");
            set.Close();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                ShowInTaskbar = false;
            else
                ShowInTaskbar = true;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            button1_Click(sender, e);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            toolStripMenuItem2_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader inFile = new StreamReader(File.Open("SCRIPTS.INI", FileMode.Open));
            while(!inFile.EndOfStream)
            {
                Scripts.Add(inFile.ReadLine());
                fileNameScripts.Add(inFile.ReadLine());
            }
            inFile.Close();

            StreamReader set = new StreamReader(File.Open("SETTINGS.INI", FileMode.Open));
            AUTOIT = set.ReadLine();
            NJ = set.ReadLine();
            EXE = set.ReadLine();
            TMP = set.ReadLine();
            APP = set.ReadLine();
            set.Close();
            button15_Click(sender, e);
            set = new StreamReader(File.Open("MAIL.INI", FileMode.Open));
            MAIL_NAME = set.ReadLine();
            MAIL_PASS = set.ReadLine();
            MAIL_SERV = set.ReadLine();
            MAIL_CLIE = set.ReadLine();
            MAIL_POP3 = set.ReadLine();
            MAIL_PPOR = set.ReadLine();
            MAIL_SMTP = set.ReadLine();
            MAIL_SPOR = set.ReadLine();
            string check = set.ReadLine();
            if (check.Equals("YES"))
                checkBox1.Checked = true;
            set.Close();
            button16_Click(sender, e);
        }
       
        private void RunList()
        {

            while (true)
            {
                if (Waiting_list.Count > 0)
                {
                    if (Waiting_list.ElementAt(0).EndsWith(".nj") || Waiting_list.ElementAt(0).EndsWith(".nj.tmp"))
                    {
                        System.Diagnostics.Process proc;
                        if (Waiting_list.ElementAt(0).EndsWith(".nj"))
                        {
                            proc = System.Diagnostics.Process.Start(AUTOIT + "\\"+ APP, NJ+"\\" + Waiting_list.ElementAt(0)); //Запускаем блокнот
                            proc.WaitForExit();//и ждем, когда он завершит свою работу
                            Waiting_list.RemoveAt(0);
                        }
                        else
                        {
                            proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, TMP + "\\" + Waiting_list.ElementAt(0)); //Запускаем блокнот
                            proc.WaitForExit();//и ждем, когда он завершит свою работу
                            if (Waiting_list.ElementAt(0).EndsWith(".tmp"))
                                File.Delete(TMP + "\\" + Waiting_list.ElementAt(0));
                            Waiting_list.RemoveAt(0);
                        }
                    }
                    else
                    {
                        string st = EXE + "\\" + Waiting_list.ElementAt(0);
                        System.Diagnostics.Process proc = new System.Diagnostics.Process(); //Запускаем блокнот
                        proc.StartInfo.FileName =st;
                        proc.Start();
                        proc.WaitForExit();//и ждем, когда он завершит свою работу
                        Waiting_list.RemoveAt(0);
                    }
                }
                else
                    Thread.Sleep(1000);
            }
        }

        private void MailServer()
        {
            try
            {
                Pop3Lib.Client myPop3;
                Pop3Lib.MailItem m;
                bool CLIENT_CHECK;

                while (!EMAIL_STOP)
                {
                    myPop3 = new Pop3Lib.Client(MAIL_POP3, Convert.ToInt32(MAIL_PPOR), MAIL_NAME, MAIL_PASS);
                    while (myPop3.NextMail(out m))
                    {
                        if (checkBox1.Checked)
                            CLIENT_CHECK = m.From.ToString().IndexOf(MAIL_CLIE) != -1;
                        else
                            CLIENT_CHECK = true;
                        if (CLIENT_CHECK)
                        {
                            Pop3Lib.Result ss = new Pop3Lib.Result(m.Source.ToString());
                            string command = ss.Message();
                            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "E-Mail client: " + command + Environment.NewLine + textBox1.Text);
                            
                            string mess = MailCommand(command);
                            string from = m.From.ToString();
                            myPop3.SendMail(MAIL_SMTP, Convert.ToInt32(MAIL_SPOR), MAIL_SERV, from, "Server", mess);
                            // ставим текущему письму отметку об удалении
                            myPop3.Delete();
                        }
                    }
                    myPop3.Close();
                    Thread.Sleep(20000);
                }
                ANSVER_EMAIL_STOP = true;
            }
            catch (Exception err)
            {
                textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "Сбой в работе E-Mail.\n Сервер остановлен.\n ERROR:\n" + err.ToString()+ "\n" + textBox1.Text);
                    
               // MessageBox.Show("Сбой в работе E-Mail.\n Сервер остановлен.\n ERROR:\n" + err.ToString(), "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string MailCommand(string command)
        {
            string mess = "К сожалению Ваша команда не распознана, проверьте правильность ее написания и повторите попытку.\n" +
                                          "Возможно Вы забыли поставить в конце команды \"END COMMAND\" ?" +
                                          "Для получения краткой справки отправьте сообщение с текстом \"TEST\".";
            if (command.StartsWith("SCRIPT LIST"))
            {
                mess = "Для выполнения скриптов отправьте \"SCRIPT RUN\" далле названия скриптов, или их номера в формате \"NUMBER Х\", в конце с новой строки \"END COMMAND\" названия и номера нужно указывать каждый в отдельной строке.\n" + 
                       "Список скриптов:\n";
                for (int i = 0; i < Scripts.Count; ++i)
                {
                    mess += i.ToString() + ". " + Scripts.ElementAt(i) + "\n";
                }
                    return mess;
            }
            if (command.StartsWith("SCRIPT RUN"))
            {
                int index;
                string err_list = "";
                index = command.IndexOf("\n");
                command = command.Remove(0, index+1);
                while ((!command.StartsWith("END COMMAND")) && (command != ""))
                {
                    int i=0;
                    string tmp = "";
                    while (command.Length > i && command[i] != '\r')
                        tmp += command[i++];
                    if (tmp.StartsWith("NUMBER"))
                    {
                        tmp = tmp.Remove(0, 7);
                        int k = Convert.ToInt32(tmp);
                        
                        Waiting_list.Add(fileNameScripts.ElementAt(k));
                    }
                    else 
                    {
                        int k = Scripts.IndexOf(tmp);
                        if (k != -1)
                            Waiting_list.Add(fileNameScripts.ElementAt(k));
                        else
                            err_list += tmp + "\n";
                    }
                    index = command.IndexOf("\n");
                    if (index != -1)
                    {
                        command = command.Remove(0, index+1);
                    }
                    else
                        command = "";
                }
                if (command != "")
                    if (err_list == "")
                        mess = "Скрипты поставлены в очередь ожидания и скоро буду исполнены.";
                    else
                        mess = "Скрипты \n" + err_list + "не добавлены в очередь ожидания, проверьте правильность их имен.\nОстальные скрипты поставлены в очередь и будут испольнены сервером.";
                return mess;
            }
            if (command.StartsWith("TEST"))
            {
                mess = "Здравствуйте!\n Вас приветствует программа Nightjar.\n Сервер работает нормально.\n"+
                       "Для получения списка доступных на сервере скриптов отправьте \"SCRIPT LIST\".\n" +
                       "Для выполнения скрипта отправьте \"SCRIPT RUN\" далле названия скриптов, или их номера в формате \"NUMBER Х\", в конце с новой строки \"END COMMAND\" названия и номера нужно указывать каждый в отдельной строке.\n" + 
                       "Для выполнения кода отправьте \"CODE RUN\" далее с новой строки код для выполнения и после него с новой строки \"END COMMAND\".\n" +
                       "Для перезапуска сервера отправьте \"SERVER RESTART\".";

                return mess;
            }
            if (command.StartsWith("CODE RUN"))
            {
                int index = command.IndexOf("\n");
                command = command.Remove(0, index + 1);
                string tmp = "";
                while ((!command.StartsWith("END COMMAND")) && (command != ""))
                {
                    index = command.IndexOf("\n");
                    tmp += command.Substring(0, index + 1);
                    
                    if (index != -1)
                    {
                        command = command.Remove(0, index + 1);
                    }
                    else
                        command = "";
                }
                if (command != "")
                {
                    string fName = "TMP" + System.DateTime.Now.ToString().Replace(':', '.') + ".nj.tmp";
                    StreamWriter outFile = new StreamWriter(File.Open(Form1.TMP + "\\" + fName, FileMode.Create));
                    outFile.Write(tmp);
                    outFile.Close();
                    Form1.Waiting_list.Add(fName);

                    mess = "Скрипт поставлен в очередь ожидания и скоро будет исполнен.";
                }

                return mess;
            }
            if (command.StartsWith("SERVER RESTART"))
            {
                Thread RestartServ = new Thread(RESTART_SERVER); //Создаем новый объект потока (Thread)
                RestartServ.Start();
                mess = "Сервер перезапускаеться. Возможно какое-то врем он будет недоступен.";
                return mess;
            }
            return mess;
        }

        public void RESTART_SERVER()
        {
            Serv.Close(); // раз выход – значит выход. Серв-нах.
            myThread.Abort();
            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "Локальный сервер остановлен." + Environment.NewLine + textBox1.Text);
            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "Остановка E-Mail сервера..." + Environment.NewLine + textBox1.Text);

            FORM_CLOSING = true;
            STOP_EMAIL_SERVER();
            FORM_CLOSING = false;
            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "E-Mail сервер остановлен." + Environment.NewLine + textBox1.Text);
            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "Запуск сервера..." + Environment.NewLine + textBox1.Text);

            Serv = new Server(); // Создаем новый экземпляр класса сервера
            Serv.Create(port, textBox1);

            myThread = new Thread(RunList); //Создаем новый объект потока (Thread)
            myThread.Start();
            ANSVER_EMAIL_STOP = false;
            EMAIL_STOP = false;
            myMailThread = new Thread(MailServer); //Создаем новый объект потока (Thread)
            myMailThread.Start();
            textBox1.Invoke(new Action<string>((s) => textBox1.Text = s), "Сервер запущен..." + Environment.NewLine + textBox1.Text);
                
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Введите имя файла")
            {
                textBox3.Text = "";
                textBox3.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.ForeColor = Color.Gray;
                textBox3.Text = "Введите имя файла";
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Введите краткое описание скрипта")
            {
                textBox4.Text = "";
                textBox4.ForeColor = SystemColors.WindowText;
            } 
            
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.ForeColor = Color.Gray;
                textBox4.Text = "Введите краткое описание скрипта";
            }
            if (Scripts.LastIndexOf(textBox4.Text) != -1)
            {
                textBox4.ForeColor = Color.Gray;
                //textBox4.Text = "Введите краткое описание скрипта";
            }
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Scripts.ToArray());
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox3.ForeColor = SystemColors.WindowText;
            textBox3.Text = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox3.ForeColor.Equals(Color.Gray))
            {
                MessageBox.Show("Введите Имя файла!", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox4.ForeColor.Equals(Color.Gray))
            {
                MessageBox.Show("Введите описание скрипта!", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FileInfo f = new FileInfo(textBox3.Text);
            FileInfo newFile;
            if (textBox3.Text.EndsWith(".nj"))
                newFile = new FileInfo(NJ + "\\" + openFileDialog1.SafeFileName);
            else
                newFile = new FileInfo(EXE + "\\" + openFileDialog1.SafeFileName);
            if (newFile.Exists)
            {
                var res = MessageBox.Show("Такой файл уже существует!\nХотите перезаписать файл?", "Nightjar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                    newFile.Delete();
                else
                    return;
            }
            f.CopyTo(newFile.FullName);
            fileNameScripts.Add(openFileDialog1.SafeFileName);
            Scripts.Add(textBox4.Text);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Scripts.ToArray());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox3.ForeColor.Equals(Color.Gray))
            {
                MessageBox.Show("Введите Имя файла!", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox4.ForeColor.Equals(Color.Gray))
            {
                MessageBox.Show("Введите описание скрипта!", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                FileInfo f = new FileInfo(NJ + "\\" + textBox3.Text + ".nj");
                FileStream fs;
                System.Diagnostics.Process proc;
                if (f.Exists)
                {
                    var res = MessageBox.Show("Такой файл уже существует!\nХотите перезаписать файл?", "Nightjar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        f.Delete();
                        fs = f.Create();
                        fs.Close();
                        string ss = f.Name;
                        int index = fileNameScripts.IndexOf(f.Name);
                        Scripts.RemoveAt(index);
                        Scripts.Insert(index, textBox4.Text);
                        proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, f.FullName);
                        listBox1.Items.Clear();
                        listBox1.Items.AddRange(Scripts.ToArray());
                    }
                    return;
                }
                fs = f.Create();
                fs.Close();
                fileNameScripts.Add(f.Name);
                Scripts.Add(textBox4.Text);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(Scripts.ToArray());
                proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, f.FullName);
            }catch(Exception exc)
            {
                MessageBox.Show("Произошла ошибка создания файла, возможно в поле имя файла введён путь к файлу", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выбирите скрипт из списка.", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FileInfo f;
            string fileName = fileNameScripts.ElementAt(listBox1.SelectedIndex);
            if (fileName.EndsWith(".nj"))    
                f = new FileInfo(NJ + "\\" + fileName);
            else
                f = new FileInfo(EXE + "\\" + fileName);
            var res = MessageBox.Show("Вы действительно хотите удалить скрипт с описанием: \""+Scripts.ElementAt(listBox1.SelectedIndex)+"\"?", "Nightjar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                f.Delete();
                
                string ss = f.Name;
                int index = fileNameScripts.IndexOf(f.Name);
                Scripts.RemoveAt(index);
                fileNameScripts.RemoveAt(index);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(Scripts.ToArray());
            }
            return;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выбирите скрипт из списка.", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox4.Text == "Введите краткое описание скрипта")
            {
                MessageBox.Show("Введите описание", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int index = listBox1.SelectedIndex;
            Scripts.RemoveAt(index);
            Scripts.Insert(index, textBox4.Text);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Scripts.ToArray());
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            textBox4.Text = listBox1.SelectedItem.ToString();
            if (fileNameScripts.ElementAt(listBox1.SelectedIndex).EndsWith(".exe"))
                button6.Enabled = false;
            else
                button6.Enabled = true;
        }

        private void listBox1_Leave(object sender, EventArgs e)
        {
            
            button6.Enabled = true;
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            if (Scripts.LastIndexOf(textBox4.Text) != -1)
            {
                textBox4.SelectionStart = 0;
                textBox4.SelectionLength = textBox4.Text.Length;
                textBox4.ForeColor = SystemColors.WindowText;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выбирите скрипт из списка.", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, NJ + "\\" + fileNameScripts.ElementAt(listBox1.SelectedIndex));        
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выбирите скрипт из списка.", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string fName = fileNameScripts.ElementAt(listBox1.SelectedIndex);
            System.Diagnostics.Process proc;
            if (fName.EndsWith(".nj"))
            {
                proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, NJ + "\\" + fName); //Запускаем блокнот
            }
            else
            {
                string st = EXE + "\\" + fName;
                proc = new System.Diagnostics.Process(); //Запускаем блокнот
                proc.StartInfo.FileName = st;
                proc.Start();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            folder(1);
        }
        private void folder(int i)
        {
            folderBrowserDialog1.ShowDialog();
            if (i == 1)
                textBox5.Text = folderBrowserDialog1.SelectedPath;
            if (i == 2)
                textBox6.Text = folderBrowserDialog1.SelectedPath;
            if (i == 3)
                textBox7.Text = folderBrowserDialog1.SelectedPath;
            if (i == 4)
                textBox8.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            folder(2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            folder(3);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            folder(4);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            AUTOIT = textBox5.Text;
            NJ = textBox6.Text;
            EXE = textBox7.Text;
            TMP = textBox8.Text;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox5.Text = AUTOIT;
            textBox6.Text = NJ;
            textBox7.Text = EXE;
            textBox8.Text = TMP;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.AddRange(Scripts.ToArray());
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(toolStripComboBox1.SelectedIndex.ToString(), "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(toolStripComboBox1.SelectedIndex.ToString(), "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string fName = fileNameScripts.ElementAt(toolStripComboBox1.SelectedIndex);
            System.Diagnostics.Process proc;
            if (fName.EndsWith(".nj"))
            {
                proc = System.Diagnostics.Process.Start(AUTOIT + "\\" + APP, NJ + "\\" + fName); //Запускаем блокнот
            }
            else
            {
                string st = EXE + "\\" + fName;
                proc = new System.Diagnostics.Process(); //Запускаем блокнот
                proc.StartInfo.FileName = st;
                proc.Start();
            }
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://nightjar.esy.es/");
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            
            System.Diagnostics.Process.Start("reference.htm");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            MAIL_NAME = textBox10.Text;
            MAIL_PASS = textBox11.Text;
            MAIL_SERV = textBox12.Text;
            MAIL_CLIE = textBox13.Text;
            MAIL_POP3 = textBox14.Text;
            MAIL_PPOR = textBox16.Text;
            MAIL_SMTP = textBox15.Text;
            MAIL_SPOR = textBox17.Text;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            textBox10.Text = MAIL_NAME;
            textBox11.Text = MAIL_PASS;
            textBox12.Text = MAIL_SERV;
            textBox13.Text = MAIL_CLIE;
            textBox14.Text = MAIL_POP3;
            textBox16.Text = MAIL_PPOR;
            textBox15.Text = MAIL_SMTP;
            textBox17.Text = MAIL_SPOR;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if ((tabControl1.SelectedIndex == 2 || tabControl1.SelectedIndex == 3) && SERVER_START)
            {
                tabControl1.SelectedIndex = 0;
                MessageBox.Show("Перед изменением настроек остановите сервер", "Nightjar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        
    }

    public class Server // класс сервера.
    {
        private int LocalPort;
        private Thread ServThread; // экземпляр потока
        TcpListener Listener; // листенер))))
        TextBox Out;
        public void Create(int port, TextBox TB1)
        {
            LocalPort = port;
            ServThread = new Thread(new ThreadStart(ServStart));
            ServThread.Start(); // запустили поток. Стартовая функция – ServStart, как видно выше
            Out = TB1;
        }

        public void Close() // Закрыть серв?
        {
            Listener.Stop();
            ServThread.Abort();
        }

        private string GetCommand(String IN)
        {
            string OUT = IN;
            if (IN == "SCRIPTS")
            {
                OUT = "";
                int i = 0;
                while (Form1.Scripts.Count > i)
                {
                    OUT += Form1.Scripts.ElementAt(i++) + "*";
                }

            }
            if (IN == "CONNECT")
                OUT = "OK";
               
            if (IN.StartsWith("S*"))
            {
                string fNsme = "";
                int i = 2;
                while (IN[i]!='*')
                    fNsme += IN[i++];
                ++i;
                IN = IN.Remove(0, i);
                if (fNsme.Equals(""))
                    fNsme = "New File " + System.DateTime.Now.ToString().Replace(':', '.');
                fNsme += ".nj";
                StreamWriter outFile = new StreamWriter(File.Open(Form1.NJ + "\\" + fNsme, FileMode.Create));
                outFile.Write(IN);
                outFile.Close();
                if (Form1.Scripts.IndexOf("Добавлено с телефона: " + fNsme.Substring(0, fNsme.Length - 3)) == -1)
                {
                    Form1.Scripts.Add("Добавлено с телефона: " + fNsme.Substring(0,fNsme.Length-3));
                    Form1.fileNameScripts.Add(fNsme);
                }
                OUT = "OK";
            }
            if (IN.StartsWith("RS*"))
            {
                IN = IN.Remove(0,3);
                string fName = "TMP" + System.DateTime.Now.ToString().Replace(':','.')+".nj.tmp";
                StreamWriter outFile = new StreamWriter(File.Open(Form1.TMP + "\\" +fName, FileMode.Create));
                outFile.Write(IN);
                outFile.Close();
                Form1.Waiting_list.Add(fName);
                OUT = "OK";
            }
            if (IN.StartsWith("RUN*"))
            {
                string scr = IN.Substring(4,IN.Length-4);
                int el = Convert.ToInt32(scr);
                string eladd = Form1.fileNameScripts.ElementAt(el);
                Form1.Waiting_list.Add(eladd);
                OUT = "OK_SCRIPT";
            }
            if (IN == "PLAYER_PLAY")
            {
                Form1.Waiting_list.Add("Play.nj");
                OUT = "PLAYER_OK";
            }
            if (IN == "PLAYER_PREV")
            {
                Form1.Waiting_list.Add("Prev.nj");
                OUT = "PLAYER_OK";
            }
            if (IN == "PLAYER_NEXT")
            {
                Form1.Waiting_list.Add("Next.nj");
                OUT = "PLAYER_OK";
            }
            return OUT;
        }

        private void ServStart()
        {
            Socket ClientSock; // сокет для обмена данными.
            string data;
            byte[] cldata = new byte[1024]; // буфер данных
            Listener = new TcpListener(LocalPort);
            while (true)
            {
                Listener.Start(); // начали слушать           
                Out.Invoke(new Action<string>((s) => Out.Text = s), "Ожидаю подключени клиентов на порт [" + Convert.ToString(LocalPort) + "]..." + Environment.NewLine + Out.Text);

                try
                {
                    ClientSock = Listener.AcceptSocket(); // пробуем принять клиента
                }
                catch
                {
                    ServThread.Abort(); // нет – жаль(
                    return;
                }
                int i = 0;

                if (ClientSock.Connected)
                {
                    Out.Invoke(new Action<string>((s) => Out.Text = s), "Клиент подключился " + ClientSock.RemoteEndPoint + Environment.NewLine + Out.Text);
                    while (true)
                    {
                        try
                        {
                            cldata[0] = 0;
                            i = ClientSock.Receive(cldata); // попытка чтения данных
                        }
                        catch { }

                        try
                        {
                            if (i > 0)
                            {
                                data = Encoding.UTF8.GetString(cldata, 0, i);
                                Out.Invoke(new Action<string>((s) => Out.Text = s), "Данные: " + data + Environment.NewLine + Out.Text);
                                if (data == "END") // если END – клиент дисконектился
                                {
                                    ClientSock.Send(Encoding.UTF8.GetBytes("CLOSE"));
                                    ClientSock.Dispose();
                                    
                                    Out.Invoke(new Action<string>((s) => Out.Text = s), "Клиент отсоединился." + Environment.NewLine + Out.Text);
                                    Listener.Stop();
                                    break;
                                }
                                else
                                {   // шлем ответ.
                                    ClientSock.Send(Encoding.UTF8.GetBytes(GetCommand(data)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ClientSock.Dispose(); // ну эт если какая хрень..
                            Listener.Stop();
                            Out.Invoke(new Action<string>((s) => Out.Text = s), "ERROR!! =(" + Environment.NewLine + ex + Environment.NewLine + Out.Text);
                            break;
                           
                        }
                    }
                }

            }

        }

        public string NJ { get; set; }

        public string TMP { get; set; }
    }
}
