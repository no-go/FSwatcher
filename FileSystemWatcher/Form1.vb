Public Class Form1

    Private data As DataTable
    Private fsw As System.IO.FileSystemWatcher
    Private countrows As Integer

    Public Sub fswEvent(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs)
        If cb_pause.Checked Then
            Return
        End If

        If tb_filter.Text.Length > 0 Then
            If Not e.FullPath.ToString().Contains(tb_filter.Text) Then
                Return
            End If
        End If

        If e.ChangeType = System.IO.WatcherChangeTypes.Created Then
            addRow("new", e.FullPath, "")
        ElseIf e.ChangeType = System.IO.WatcherChangeTypes.Deleted Then
            addRow("delete", e.FullPath, "")
        ElseIf e.ChangeType = System.IO.WatcherChangeTypes.Changed Then
            addRow("change", e.FullPath, "")
        End If

    End Sub

    Public Sub fswRename(ByVal sender As Object, ByVal e As System.IO.RenamedEventArgs)
        If tb_filter.Text.Length > 0 Then
            If Not e.OldFullPath.ToString().Contains(tb_filter.Text) Then
                Return
            End If
        End If

        If Not cb_pause.Checked Then
            addRow("rename", e.OldFullPath, e.FullPath)
        End If
    End Sub

    Private Sub addRow(actionname As String, fullPath1 As String, fullPath2 As String)
        Dim R As DataRow = data.NewRow
        R("Action") = actionname
        R("Timestamp") = Now().ToString("yyyy-MM-dd HH:mm:ss.ffff")
        R("File1") = fullPath1
        R("File2") = fullPath2
        data.Rows.Add(R)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        data = New DataTable
        data.Columns.Add("Action")
        data.Columns.Add("Timestamp")
        data.Columns.Add("File1")
        data.Columns.Add("File2")

        DataGridView1.DataSource = data
        DataGridView1.Columns("Action").Width = 60
        DataGridView1.Columns("Timestamp").Width = 180
        DataGridView1.Columns("File1").Width = 450
        DataGridView1.Columns("File2").Width = 300

        For t As Integer = 65 To 90
            Try
                fsw = New System.IO.FileSystemWatcher
                fsw.Path = Convert.ToChar(t) & ":\"
                fsw.Filter = "*"
                fsw.IncludeSubdirectories = True
                fsw.EnableRaisingEvents = True
                AddHandler fsw.Changed, AddressOf fswEvent
                AddHandler fsw.Created, AddressOf fswEvent
                AddHandler fsw.Deleted, AddressOf fswEvent
                AddHandler fsw.Renamed, AddressOf fswRename
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If cb_pause.Checked Then
            Return
        End If

        DataGridView1.DataSource = data
        If countrows < DataGridView1.Rows.Count Then
            countrows = DataGridView1.Rows.Count
            DataGridView1.Sort(DataGridView1.Columns("Timestamp"), System.ComponentModel.ListSortDirection.Descending)
        End If
    End Sub

    Private Sub btn_clear_Click(sender As Object, e As EventArgs) Handles btn_clear.Click
        data.Rows.Clear()
    End Sub
End Class
