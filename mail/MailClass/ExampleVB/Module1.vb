Module Module1

  Sub Main()
    Dim host As String = "", port As Integer = 0
    Dim login As String = "", pwd As String = ""

    Console.WriteLine("Укажите адрес почтового сервера, например pop.yandex.ru")
    host = Console.ReadLine().Trim()

    Console.WriteLine("Укажите номер порта почтового сервера, например 110")
    Integer.TryParse(Console.ReadLine(), port)
    If port <= 0 Then port = 110

    Console.WriteLine("Укажите имя пользователя")
    login = Console.ReadLine().Trim()

    Console.WriteLine("Укажите имя пароль")
    pwd = Console.ReadLine().Trim()

    'Dim myPop3 As New Pop3Lib.Client("pop.yandex.ru", "логин", "пароль")
    Dim myPop3 As New Pop3Lib.Client(host, port, login, pwd)

    Dim m As Pop3Lib.MailItem
    Do While (myPop3.NextMail(m))
      'Dim addressTo As String = m.To.ToString() 'кому адресовано письмо
      'Dim mailText As String = m.GetText() 'получить текст письма, если есть
      'Dim b() As Byte = m.GetBinary() 'получить содержимое файла, если есть
      'If m.IsBinary Then Console.WriteLine("Бинарное содержимое.")
      'If m.IsText Then Console.WriteLine("Текстовое содержимое.")
      '      If m.IsMultipart Then
      '        Console.WriteLine("Смешанное содержимое.")
      '        Dim mc As Pop3Lib.MailItemCollection = m.GetItems()
      '        For Each mi As Pop3Lib.MailItemBase In mc
      '          If m.IsBinary Then Console.WriteLine("У этой части письма бинарное содержимое.")
      '          If m.IsText Then Console.WriteLine("У этой части письма текстовое содержимое.")
      '          Dim partMailText As String = mi.GetText() 'получить текст письма, если есть
      '          Dim partB() As Byte = mi.GetBinary() 'получить содержимое файла, если есть
      '        Next
      '      End If
      Console.Write("Письмо от {0} с темой {1}", m.From, m.Subject)
      Console.WriteLine("Хотите его удалить (y/n)?")
      If Console.ReadLine().ToLower().StartsWith("y") Then
        ' ставим текущему письму отметку об удалении
        myPop3.Delete()
        Console.WriteLine("Письмо помечено для удаления.")
      End If
    Loop

    'закрываем соединение, при этом удаляются все письма, помеченные для удаления
    myPop3.Close()
    Console.ReadKey()
  End Sub

End Module
