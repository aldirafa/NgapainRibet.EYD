Imports System.Text.Json
Imports NgapainRibet.EYD.Data.Entities

Namespace Kbbi

    Public Class KbbiEntryResult
        Public Property Kata As String
        Public Property Lema As IReadOnlyList(Of String)
        Public Property Makna As IReadOnlyList(Of String)
        Public Property Contoh As IReadOnlyList(Of String)
        Public Property Varian As IReadOnlyList(Of String)

        Friend Shared Function FromEntity(entity As KbbiEntry) As KbbiEntryResult
            Return New KbbiEntryResult With {
                .Kata = entity.Kata,
                .Lema = DeserializeArray(entity.LemaJson),
                .Makna = DeserializeArray(entity.MaknaJson),
                .Contoh = DeserializeArray(entity.ContohJson),
                .Varian = DeserializeArray(entity.VarianJson)
            }
        End Function

        Private Shared Function DeserializeArray(json As String) As IReadOnlyList(Of String)
            If String.IsNullOrEmpty(json) Then
                Return Array.Empty(Of String)()
            End If
            Return JsonSerializer.Deserialize(Of List(Of String))(json)
        End Function
    End Class

End Namespace
