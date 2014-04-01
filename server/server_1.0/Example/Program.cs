using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Mail;
namespace Example
{
  class Program
  {
    static void Main(string[] args)
    {
      /*
       * ВНИМАНИЕ. НЕ ЗАБУДЬТЕ УКАЗАТЬ АДРЕС ПОЧТОВОГО СЕРВЕРА, СУЩЕСТВУЮЩИЕ ИМЯ ПОЛЬЗОВАТЕЛЯ И ПАРОЛЬ!
       */
      // Pop3Lib.Client myPop3 = new Pop3Lib.Client("pop.yandex.ru", "логин", "пароль");
        
            Pop3Lib.Client myPop3 = new Pop3Lib.Client("pop.yandex.ru", 110, "app.nightjar", "frpj546546");
         
            Pop3Lib.MailItem m;
            
            while (myPop3.NextMail(out m))
            {
                try
                {
                    if (m.From.ToString().IndexOf("vint1024@gmail.com") != -1)
                    {
                        Console.Write("Письмо от {0} с темой {1}", m.From, m.Subject);
                        Console.WriteLine("Ответить? (y/n)?");
                        if (Console.ReadLine().ToLower().StartsWith("y"))
                        {
                            
                            Pop3Lib.Result ss = new Pop3Lib.Result(m.Source.ToString());

                            Pop3Lib.MailItem f;
                            ss.ParseMail(out f);

                            Console.WriteLine(f.);

                            string mess = "Hello, I'm Naightjar application.";

                            //myPop3.SendMail("smtp.yandex.ru", "app.nightjar@ya.ru", "vint1024@gmail.com", "Server", mess);
                            /*
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("app.nightjar@ya.ru");
                            mail.To.Add(new MailAddress("vint1024@gmail.com"));
                            mail.Subject = "Server";
                            mail.Body = mess;
                            SmtpClient client = new SmtpClient();
                            client.Host = "smtp.yandex.ru";
                            client.Port = 587;
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential("app.nightjar", "frpj546546");
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.Send(mail);
                            mail.Dispose();
                             * */
                        }
                        Console.WriteLine("Хотите его удалить (y/n)?");
                        if (Console.ReadLine().ToLower().StartsWith("y"))
                        {
                            // ставим текущему письму отметку об удалении
                            myPop3.Delete();
                            Console.WriteLine("Письмо помечено для удаления.");
                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("\n" + err);
                    Console.Read();

                }
            }

            myPop3.Close();
            Console.ReadKey();
       
    }
  }
}
