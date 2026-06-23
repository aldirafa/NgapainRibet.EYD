Imports Microsoft.EntityFrameworkCore
Imports NgapainRibet.EYD.Data

Namespace SpellCheck

    Public Class SpellCheckService

        Private ReadOnly _dbPath As String
        Private ReadOnly _wordsLock As New Object()
        Private _words As String() = Nothing

        Public Sub New()
            Me.New(EydDbContext.DefaultDbPath())
        End Sub

        Public Sub New(dbPath As String)
            _dbPath = dbPath
        End Sub

        Public Function IsBaku(word As String) As Boolean
            Dim normalized = word.Trim().ToLowerInvariant()
            Using context As New EydDbContext(_dbPath)
                Return context.KbbiEntries.Any(Function(x) x.Kata = normalized)
            End Using
        End Function

        Public Function Suggest(word As String, Optional maxSuggestions As Integer = 5) As IReadOnlyList(Of SpellSuggestion)
            Dim normalized = word.Trim().ToLowerInvariant()
            Dim allWords = GetWordCache()

            Return allWords.
                Where(Function(w) Math.Abs(w.Length - normalized.Length) <= 2).
                Select(Function(w) New SpellSuggestion With {.Word = w, .Distance = LevenshteinDistance(normalized, w)}).
                Where(Function(s) s.Distance > 0).
                OrderBy(Function(s) s.Distance).
                ThenBy(Function(s) s.Word).
                Take(maxSuggestions).
                ToList()
        End Function

        Private Function GetWordCache() As String()
            If _words Is Nothing Then
                SyncLock _wordsLock
                    If _words Is Nothing Then
                        Using context As New EydDbContext(_dbPath)
                            _words = context.KbbiEntries.Select(Function(x) x.Kata).Distinct().ToArray()
                        End Using
                    End If
                End SyncLock
            End If
            Return _words
        End Function

        Private Shared Function LevenshteinDistance(a As String, b As String) As Integer
            Dim lenA = a.Length
            Dim lenB = b.Length
            Dim dist(lenA, lenB) As Integer

            For i = 0 To lenA
                dist(i, 0) = i
            Next
            For j = 0 To lenB
                dist(0, j) = j
            Next

            For i = 1 To lenA
                For j = 1 To lenB
                    Dim cost = If(a(i - 1) = b(j - 1), 0, 1)
                    dist(i, j) = Math.Min(
                        Math.Min(dist(i - 1, j) + 1, dist(i, j - 1) + 1),
                        dist(i - 1, j - 1) + cost)
                Next
            Next

            Return dist(lenA, lenB)
        End Function

    End Class

End Namespace
