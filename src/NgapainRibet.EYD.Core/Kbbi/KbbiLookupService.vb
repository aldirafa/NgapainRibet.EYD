Imports Microsoft.EntityFrameworkCore
Imports NgapainRibet.EYD.Data

Namespace Kbbi

    Public Class KbbiLookupService

        Private ReadOnly _dbPath As String

        Public Sub New()
            Me.New(EydDbContext.DefaultDbPath())
        End Sub

        Public Sub New(dbPath As String)
            _dbPath = dbPath
        End Sub

        Public Function FindExact(kata As String) As IReadOnlyList(Of KbbiEntryResult)
            Dim normalized = kata.Trim().ToLowerInvariant()
            Using context As New EydDbContext(_dbPath)
                Return context.KbbiEntries.
                    Where(Function(x) x.Kata = normalized).
                    ToList().
                    Select(Function(x) KbbiEntryResult.FromEntity(x)).
                    ToList()
            End Using
        End Function

        Public Function FindByPrefix(prefix As String, Optional maxResults As Integer = 20) As IReadOnlyList(Of KbbiEntryResult)
            Dim normalized = prefix.Trim().ToLowerInvariant()
            Using context As New EydDbContext(_dbPath)
                Return context.KbbiEntries.
                    Where(Function(x) x.Kata.StartsWith(normalized)).
                    OrderBy(Function(x) x.Kata).
                    Take(maxResults).
                    ToList().
                    Select(Function(x) KbbiEntryResult.FromEntity(x)).
                    ToList()
            End Using
        End Function

        Public Function Search(keyword As String, Optional maxResults As Integer = 20) As IReadOnlyList(Of KbbiEntryResult)
            Dim normalized = keyword.Trim().ToLowerInvariant()
            Using context As New EydDbContext(_dbPath)
                Return context.KbbiEntries.
                    Where(Function(x) x.Kata.Contains(normalized) OrElse (x.ContohJson IsNot Nothing AndAlso x.ContohJson.Contains(normalized))).
                    OrderBy(Function(x) x.Kata).
                    Take(maxResults).
                    ToList().
                    Select(Function(x) KbbiEntryResult.FromEntity(x)).
                    ToList()
            End Using
        End Function

    End Class

End Namespace
