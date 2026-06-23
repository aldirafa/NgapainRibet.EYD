Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports NgapainRibet.EYD.Data
Imports NgapainRibet.EYD.Data.Entities

Namespace KbbiImport

    Friend Class KbbiJsonEntry
        <JsonPropertyName("lema")>
        Public Property Lema As List(Of String) = New List(Of String)()

        <JsonPropertyName("makna")>
        Public Property Makna As List(Of String) = New List(Of String)()

        <JsonPropertyName("contoh")>
        Public Property Contoh As List(Of String) = Nothing

        <JsonPropertyName("varian")>
        Public Property Varian As List(Of String) = Nothing

        <JsonPropertyName("kata")>
        Public Property Kata As String = String.Empty
    End Class

    Public Class KbbiJsonImporter

        Private Const BatchSize As Integer = 2000

        Public Function Import(jsonPath As String, context As EydDbContext) As Integer
            Using stream = IO.File.OpenRead(jsonPath)
                Dim options As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
                Dim entries = JsonSerializer.Deserialize(Of List(Of KbbiJsonEntry))(stream, options)

                If entries Is Nothing Then
                    Return 0
                End If

                Dim total As Integer = 0
                Dim batch As New List(Of KbbiEntry)(BatchSize)

                For Each entry In entries
                    batch.Add(New KbbiEntry With {
                        .Kata = entry.Kata,
                        .LemaJson = JsonSerializer.Serialize(entry.Lema),
                        .MaknaJson = JsonSerializer.Serialize(entry.Makna),
                        .ContohJson = If(entry.Contoh Is Nothing, Nothing, JsonSerializer.Serialize(entry.Contoh)),
                        .VarianJson = If(entry.Varian Is Nothing, Nothing, JsonSerializer.Serialize(entry.Varian))
                    })

                    If batch.Count >= BatchSize Then
                        context.KbbiEntries.AddRange(batch)
                        context.SaveChanges()
                        total += batch.Count
                        batch.Clear()
                    End If
                Next

                If batch.Count > 0 Then
                    context.KbbiEntries.AddRange(batch)
                    context.SaveChanges()
                    total += batch.Count
                End If

                Return total
            End Using
        End Function

    End Class

End Namespace
