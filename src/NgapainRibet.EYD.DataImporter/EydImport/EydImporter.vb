Imports NgapainRibet.EYD.Data
Imports NgapainRibet.EYD.Data.Entities

Namespace EydImport

    Public Class EydImporter

        Private Shared ReadOnly GroupTitles As New Dictionary(Of String, String) From {
            {"penggunaan-huruf", "Penggunaan Huruf"},
            {"penggunaan-tanda-baca", "Penggunaan Tanda Baca"},
            {"penulisan-kata", "Penulisan Kata"},
            {"unsur-serapan", "Penulisan Unsur Serapan"},
            {"surat-keputusan", "Surat Keputusan"}
        }

        Private ReadOnly _parser As New EydMarkdownParser()

        Public Function Import(eydRootDir As String, context As EydDbContext) As (Categories As Integer, Rules As Integer)
            Dim categoryCount As Integer = 0
            Dim ruleCount As Integer = 0

            For Each filePath In IO.Directory.EnumerateFiles(eydRootDir, "*.md", IO.SearchOption.AllDirectories)
                Dim relativePath = IO.Path.GetRelativePath(eydRootDir, filePath)
                Dim segments = relativePath.Split(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)

                ' segments.Length = 2 -> "<group>\.md" (group-level intro, e.g. unsur-serapan\.md)
                ' segments.Length = 3 -> "<group>\<slug>\.md" (a rule-bearing subcategory)
                ' segments.Length = 1 -> root "\.md" intro, has no group, skipped.
                If segments.Length < 2 Then
                    Continue For
                End If

                Dim groupSlug = segments(0)
                Dim slug = If(segments.Length >= 3, segments(1), groupSlug)
                Dim groupTitle As String = Nothing
                If Not GroupTitles.TryGetValue(groupSlug, groupTitle) Then
                    groupTitle = groupSlug
                End If

                Dim category = _parser.Parse(filePath, groupSlug, groupTitle, slug)

                ' Files whose first heading doesn't match the "Letter. Title" pattern
                ' (root intro, surat-keputusan, unsur-serapan's own Roman-numeral intro)
                ' yield no category — nothing to import.
                If category Is Nothing Then
                    Continue For
                End If

                context.EydCategories.Add(category)
                categoryCount += 1
                ruleCount += category.Rules.Count
            Next

            context.SaveChanges()

            Return (categoryCount, ruleCount)
        End Function

    End Class

End Namespace
