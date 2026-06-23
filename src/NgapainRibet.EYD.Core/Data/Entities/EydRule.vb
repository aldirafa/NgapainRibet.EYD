Namespace Data.Entities

    Public Class EydRule
        Public Property Id As Integer
        Public Property CategoryId As Integer
        Public Property RuleNumber As Integer
        Public Property SubLetter As String = Nothing
        Public Property RuleText As String = String.Empty
        Public Property ExamplesJson As String = Nothing
        Public Property NotesText As String = Nothing

        Public Property Category As EydCategory = Nothing
    End Class

End Namespace
