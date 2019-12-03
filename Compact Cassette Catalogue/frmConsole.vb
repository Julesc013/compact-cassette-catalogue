Imports System.IO

Public Class frmConsole

    ''Set up command history
    'Dim history As String()
    'Dim counter As Integer = -1
    'Dim position As Integer = counter

    Private Sub FrmConsole_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Reset objects
        lstConsole.Items.Clear()
        txtCommand.Text = Nothing

        'Display help message
        lstConsole.Items.Add("Type ""Help"" to see available commands.")

    End Sub

    Private Sub frmConsole_Show(sender As Object, e As EventArgs) Handles MyBase.Shown

        'Focus on textbox
        txtCommand.Focus()

    End Sub

    Private Sub BtnEnter_Click(sender As Object, e As EventArgs) Handles btnEnter.Click

        'Get string from textbox
        Dim command As String = txtCommand.Text.ToLower 'Get full command sentence (case-insensitive)


        If Not String.IsNullOrWhiteSpace(command) Then
            'If the string is a valid command...

            ''Add this command to the history
            'counter += 1
            'history(counter) = txtCommand.Text 'Keep case sensitive
            'position = counter '<<<<<<<<doesnt work atm

            'Split command for processing
            Dim arguments As String() = command.Split(" "c) 'Split command sentence into words (first word is the main command action)
            Dim keyword As String = arguments(0) 'Get main command word/action

            Dim message As String = Nothing 'Return message for console

            Select Case keyword
                'PARENT COMMANDS

                Case = "help"

                    'If bad format, show template
                    If arguments.Length = 1 Then

                        lstConsole.Items.Add("Built in commands:")
                        lstConsole.Items.Add(vbTab & "Clear")
                        lstConsole.Items.Add(vbTab & "Time")
                        lstConsole.Items.Add(vbTab & "<Setting> [Value]")

                    ElseIf arguments.Length = 2 Then

                        'For each different command, offer specific help

                        If arguments(1) = "clear" Then

                            lstConsole.Items.Add("Clear console history.")

                        ElseIf arguments(1) = "time" Then

                            lstConsole.Items.Add("Display the time the program was loaded.")

                        ElseIf {"setting", "settings", "[setting]", "[settings]"}.Contains(arguments(1)) Then

                            lstConsole.Items.Add("Change program settings:" & vbNewLine &
                                             "  showMessages [True|False]")

                        End If

                    Else

                        message = "Bad syntax, use: help [command]"

                    End If

                Case = "clear", "cls"

                    If arguments.Length = 1 Then

                        lstConsole.Items.Clear()

                    Else

                        message = "Bad syntax, use: clear"

                    End If

                Case = "time"

                    If arguments.Length = 1 Then

                        lstConsole.Items.Add(timeLoaded)

                    Else

                        message = "Bad syntax, use: time"

                    End If

                Case = "quit", "exit", "close"

                    If arguments.Length = 1 Then

                        frmMain.closeApplication() 'Close safely.

                    Else

                        message = "Bad syntax, use: quit"

                    End If

                Case = "kill"

                    If arguments.Length = 1 Then

                        Application.Exit() 'Close violently.

                    Else

                        message = "Bad syntax, use: kill"

                    End If

                Case = "showmessages"

                    message = "Bad syntax, use: showMessages [True|False]"

                    If arguments.Length = 1 Then
                        'Display current value

                        message = "showMessages currently is " & CStr(My.Settings.showMessages)

                    ElseIf arguments.Length = 2 Then
                        'Update value

                        If {"false", "true"}.Contains(arguments(1)) Then
                            'If valid boolean supplied, update value

                            Dim toggle As Boolean = CBool(arguments(1))
                            My.Settings.showMessages = toggle

                            'Build message.
                            Dim now As DateTime = DateTime.Now
                            Dim stamp As String = "[" & consoleStamp(now) & "]"
                            message = stamp & " Set showMessages to " & CStr(toggle)

                        End If

                    End If

                Case = "defaultdirectory"

                    message = "Bad syntax, use: defaultDirectory [Valid folder directory]"

                    If arguments.Length = 1 Then
                        'Display current value

                        message = "defaultDirectory currently is " & CStr(My.Settings.defaultDirectory)

                    ElseIf arguments.Length = 2 Then
                        'Update value

                        Dim folderpath As String = arguments(1)

                        If Directory.Exists(folderpath) Then
                            'If valid directory supplied, update value

                            My.Settings.defaultDirectory = folderpath

                            'Build message.
                            Dim now As DateTime = DateTime.Now
                            Dim stamp As String = "[" & consoleStamp(now) & "]"
                            message = stamp & " Set defaultDirectory to " & CStr(filePath)

                        End If

                    End If

                Case Else 'FOR UNRECOGNISED COMMANDS

                    message = "Command """ & command & """ not recognised."

            End Select

            'Display return message
            If message IsNot Nothing Then
                lstConsole.Items.Add(message)
            End If

            'Reset/clear box
            txtCommand.Text = Nothing

        Else
            'Show error is invalid command
            If My.Settings.showMessages = True Then
                MsgBox("Command cannot be empty or whitespace (spaces)", MsgBoxStyle.Exclamation, "Invalid Command")
            End If

        End If

    End Sub

    Private Sub frmConsole_Close(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'Hide the console instead of closing down the form

        Me.Hide()
        e.Cancel = True

    End Sub

    'Private Sub frmConsole_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
    '    'Scroll through command history

    '    'Sets Handled to true to prevent other controls from
    '    'receiving the key if an arrow key was pressed
    '    Dim bHandled As Boolean = False

    '    Select Case e.KeyCode

    '        Case Keys.Up
    '            'If not at top, go one command up
    '            'If at top, display current position

    '            If position <> 0 Then 'If can scroll further

    '                position -= 1

    '            End If

    '            txtCommand.Text = history(position) 'Load historic command

    '            e.Handled = True

    '        Case Keys.Down
    '            'If not at bottom, go one command down
    '            'If at bottom, clear command box

    '            If position <> counter Then 'If can scroll further

    '                position += 1
    '                txtCommand.Text = history(position) 'Load historic command

    '            Else

    '                txtCommand.Text = Nothing

    '            End If

    '            e.Handled = True

    '    End Select

    'End Sub

End Class