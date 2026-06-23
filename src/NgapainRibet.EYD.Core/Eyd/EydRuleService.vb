Imports Microsoft.EntityFrameworkCore
Imports NgapainRibet.EYD.Data

Namespace Eyd

    Public Class EydRuleService

        Private ReadOnly _dbPath As String

        Public Sub New()
            Me.New(EydDbContext.DefaultDbPath())
        End Sub

        Public Sub New(dbPath As String)
            _dbPath = dbPath
        End Sub

        Public Function GetCategories() As IReadOnlyList(Of EydCategoryResult)
            Using context As New EydDbContext(_dbPath)
                Return context.EydCategories.
                    OrderBy(Function(c) c.GroupSlug).ThenBy(Function(c) c.Letter).
                    Select(Function(c) New EydCategoryResult With {
                        .GroupSlug = c.GroupSlug,
                        .GroupTitle = c.GroupTitle,
                        .Slug = c.Slug,
                        .Title = c.Title,
                        .Letter = c.Letter
                    }).
                    ToList()
            End Using
        End Function

        Public Function GetCategoriesByGroup(groupSlug As String) As IReadOnlyList(Of EydCategoryResult)
            Using context As New EydDbContext(_dbPath)
                Return context.EydCategories.
                    Where(Function(c) c.GroupSlug = groupSlug).
                    OrderBy(Function(c) c.Letter).
                    Select(Function(c) New EydCategoryResult With {
                        .GroupSlug = c.GroupSlug,
                        .GroupTitle = c.GroupTitle,
                        .Slug = c.Slug,
                        .Title = c.Title,
                        .Letter = c.Letter
                    }).
                    ToList()
            End Using
        End Function

        Public Function GetRules(categorySlug As String) As IReadOnlyList(Of EydRuleResult)
            Using context As New EydDbContext(_dbPath)
                Dim rules = context.EydRules.
                    Include(Function(r) r.Category).
                    Where(Function(r) r.Category.Slug = categorySlug).
                    OrderBy(Function(r) r.RuleNumber).ThenBy(Function(r) r.SubLetter).
                    ToList()

                Return rules.Select(Function(r) EydRuleResult.FromEntity(r, categorySlug)).ToList()
            End Using
        End Function

        Public Function GetRule(categorySlug As String, ruleNumber As Integer, Optional subLetter As String = Nothing) As EydRuleResult
            Using context As New EydDbContext(_dbPath)
                Dim rule = context.EydRules.
                    Include(Function(r) r.Category).
                    FirstOrDefault(Function(r) r.Category.Slug = categorySlug AndAlso r.RuleNumber = ruleNumber AndAlso r.SubLetter = subLetter)

                Return If(rule Is Nothing, Nothing, EydRuleResult.FromEntity(rule, categorySlug))
            End Using
        End Function

        Public Function SearchRules(keyword As String, Optional maxResults As Integer = 20) As IReadOnlyList(Of EydRuleResult)
            Dim normalized = keyword.Trim()
            Using context As New EydDbContext(_dbPath)
                Dim rules = context.EydRules.
                    Include(Function(r) r.Category).
                    Where(Function(r) r.RuleText.Contains(normalized) OrElse (r.ExamplesJson IsNot Nothing AndAlso r.ExamplesJson.Contains(normalized))).
                    Take(maxResults).
                    ToList()

                Return rules.Select(Function(r) EydRuleResult.FromEntity(r, r.Category.Slug)).ToList()
            End Using
        End Function

    End Class

End Namespace
