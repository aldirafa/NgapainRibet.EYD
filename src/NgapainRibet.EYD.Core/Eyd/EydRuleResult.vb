Imports System.Text.Json

Namespace Eyd

    Public Class EydRuleResult
        Public Property CategorySlug As String
        Public Property RuleNumber As Integer
        Public Property SubLetter As String
        Public Property RuleText As String
        Public Property Examples As IReadOnlyList(Of String)
        Public Property NotesText As String

        Friend Shared Function FromEntity(entity As Data.Entities.EydRule, categorySlug As String) As EydRuleResult
            Dim examples As IReadOnlyList(Of String) = Array.Empty(Of String)()
            If Not String.IsNullOrEmpty(entity.ExamplesJson) Then
                examples = JsonSerializer.Deserialize(Of List(Of String))(entity.ExamplesJson)
            End If

            Return New EydRuleResult With {
                .CategorySlug = categorySlug,
                .RuleNumber = entity.RuleNumber,
                .SubLetter = entity.SubLetter,
                .RuleText = entity.RuleText,
                .Examples = examples,
                .NotesText = entity.NotesText
            }
        End Function
    End Class

End Namespace
