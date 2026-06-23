Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports Markdig
Imports Markdig.Extensions.Tables
Imports Markdig.Syntax
Imports Markdig.Syntax.Inlines
Imports NgapainRibet.EYD.Data.Entities

Namespace EydImport

    ''' <summary>
    ''' Walks the Markdig AST for a single EYD markdown export and extracts a category
    ''' plus its rules. The exporter prepends ~90 lines of nav/header HTML before the real
    ''' content; that HTML often fails to terminate as a clean Markdig block before the real
    ''' "# Letter. Title" heading (CommonMark HTML-block mode just consumes raw lines until a
    ''' blank line, so the heading text gets swallowed as inert text instead of parsed). To
    ''' avoid relying on Markdig's block boundaries there, the real content's start line is
    ''' located directly in the raw text via regex first, and only that slice is parsed.
    ''' </summary>
    Public Class EydMarkdownParser

        Private Shared ReadOnly Pipeline As MarkdownPipeline = New MarkdownPipelineBuilder().UseAdvancedExtensions().Build()

        Private Shared ReadOnly CategoryHeadingLinePattern As New Regex("(?m)^#\s+([A-Z])\.\s*(.+?)\s*$", RegexOptions.Compiled)
        Private Shared ReadOnly RuleHeadingPattern As New Regex("^(\d+)\.\s*(.+)$", RegexOptions.Compiled)
        Private Shared ReadOnly SubRuleHeadingPattern As New Regex("^([a-z])\.\s*(.+)$", RegexOptions.Compiled)

        Public Function Parse(filePath As String, groupSlug As String, groupTitle As String, slug As String) As EydCategory
            Dim markdownText As String = IO.File.ReadAllText(filePath)

            Dim startMatch = CategoryHeadingLinePattern.Match(markdownText)
            If Not startMatch.Success Then
                Return Nothing
            End If

            Dim category As New EydCategory With {
                .GroupSlug = groupSlug,
                .GroupTitle = groupTitle,
                .Slug = slug,
                .Title = startMatch.Groups(2).Value.Trim(),
                .Letter = startMatch.Groups(1).Value,
                .SourceFile = filePath
            }

            Dim document As MarkdownDocument = Markdown.Parse(markdownText.Substring(startMatch.Index), Pipeline)

            Dim currentRule As EydRule = Nothing
            Dim examples As New List(Of String)()
            Dim expectingExamples As Boolean = False

            For Each block As Block In document
                Dim h1 = TryCastHeading(block, 1)
                If h1 IsNot Nothing Then
                    ' This is the category heading itself, already captured above; skip it.
                    Continue For
                End If

                Dim h4 = TryCastHeading(block, 4)
                If h4 IsNot Nothing Then
                    FlushExamples(currentRule, examples)
                    expectingExamples = False

                    Dim text = GetPlainText(h4.Inline)
                    Dim m = RuleHeadingPattern.Match(text)
                    If m.Success Then
                        currentRule = New EydRule With {
                            .RuleNumber = Integer.Parse(m.Groups(1).Value),
                            .SubLetter = Nothing,
                            .RuleText = m.Groups(2).Value.Trim()
                        }
                        category.Rules.Add(currentRule)
                    End If
                    Continue For
                End If

                Dim h6 = TryCastHeading(block, 6)
                If h6 IsNot Nothing Then
                    FlushExamples(currentRule, examples)
                    expectingExamples = False

                    Dim text = GetPlainText(h6.Inline)
                    Dim m = SubRuleHeadingPattern.Match(text)
                    If m.Success AndAlso currentRule IsNot Nothing Then
                        Dim subRule As New EydRule With {
                            .RuleNumber = currentRule.RuleNumber,
                            .SubLetter = m.Groups(1).Value,
                            .RuleText = m.Groups(2).Value.Trim()
                        }
                        category.Rules.Add(subRule)
                        currentRule = subRule
                    End If
                    Continue For
                End If

                If currentRule Is Nothing Then
                    Continue For
                End If

                Dim paragraph = TryCast(block, ParagraphBlock)
                If paragraph IsNot Nothing Then
                    Dim text = GetPlainText(paragraph.Inline).Trim()
                    If text = "Misalnya:" Then
                        expectingExamples = True
                    Else
                        currentRule.NotesText = If(currentRule.NotesText, "") & text & Environment.NewLine
                    End If
                    Continue For
                End If

                Dim listBlock = TryCast(block, ListBlock)
                If listBlock IsNot Nothing AndAlso expectingExamples Then
                    For Each item As Block In listBlock
                        Dim listItem = TryCast(item, ListItemBlock)
                        If listItem Is Nothing Then Continue For

                        For Each inner As Block In listItem
                            Dim innerParagraph = TryCast(inner, ParagraphBlock)
                            If innerParagraph IsNot Nothing Then
                                examples.Add(GetPlainText(innerParagraph.Inline).Trim())
                            End If
                        Next
                    Next
                    Continue For
                End If

                Dim table = TryCast(block, Table)
                If table IsNot Nothing AndAlso expectingExamples Then
                    For Each rowBlock As Block In table
                        Dim row = TryCast(rowBlock, TableRow)
                        If row Is Nothing Then Continue For

                        Dim cellTexts As New List(Of String)()
                        For Each cellBlock As Block In row
                            Dim cell = TryCast(cellBlock, TableCell)
                            If cell Is Nothing Then Continue For

                            Dim cellParagraph = TryCast(cell.FirstOrDefault(), ParagraphBlock)
                            If cellParagraph IsNot Nothing Then
                                cellTexts.Add(GetPlainText(cellParagraph.Inline).Trim())
                            End If
                        Next

                        If cellTexts.Count > 0 Then
                            examples.Add(String.Join(" -> ", cellTexts))
                        End If
                    Next
                    Continue For
                End If

                ' ThematicBreakBlock (* * *) and any other block types are ignored:
                ' example groups are flattened into a single list for v1.
            Next

            FlushExamples(currentRule, examples)

            Return category
        End Function

        Private Shared Function TryCastHeading(block As Block, level As Integer) As HeadingBlock
            Dim heading = TryCast(block, HeadingBlock)
            If heading IsNot Nothing AndAlso heading.Level = level Then
                Return heading
            End If
            Return Nothing
        End Function

        Private Shared Sub FlushExamples(rule As EydRule, examples As List(Of String))
            If rule IsNot Nothing AndAlso examples.Count > 0 Then
                rule.ExamplesJson = Text.Json.JsonSerializer.Serialize(examples)
            End If
            examples.Clear()
        End Sub

        Private Shared Function GetPlainText(inline As ContainerInline) As String
            If inline Is Nothing Then
                Return String.Empty
            End If

            Dim sb As New StringBuilder()
            AppendPlainText(inline, sb)
            Return sb.ToString()
        End Function

        Private Shared Sub AppendPlainText(container As ContainerInline, sb As StringBuilder)
            For Each child As Inline In container
                Select Case True
                    Case TypeOf child Is LiteralInline
                        sb.Append(DirectCast(child, LiteralInline).Content.ToString())
                    Case TypeOf child Is ContainerInline
                        AppendPlainText(DirectCast(child, ContainerInline), sb)
                    Case TypeOf child Is LineBreakInline
                        sb.Append(" ")
                    Case Else
                        ' Skip other inline types (autolinks, html, etc.) for v1 plain-text extraction.
                End Select
            Next
        End Sub

    End Class

End Namespace
