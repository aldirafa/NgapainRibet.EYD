Namespace Data.Entities

    Public Class EydCategory
        Public Property Id As Integer
        Public Property GroupSlug As String = String.Empty
        Public Property GroupTitle As String = String.Empty
        Public Property Slug As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property Letter As String = Nothing
        Public Property SourceFile As String = String.Empty

        Public Property Rules As List(Of EydRule) = New List(Of EydRule)()
    End Class

End Namespace
