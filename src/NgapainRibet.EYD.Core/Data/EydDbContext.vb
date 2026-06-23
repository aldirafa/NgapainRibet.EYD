Imports Microsoft.EntityFrameworkCore
Imports NgapainRibet.EYD.Data.Entities

Namespace Data

    Public Class EydDbContext
        Inherits DbContext

        Private ReadOnly _dbPath As String

        Public Property KbbiEntries As DbSet(Of KbbiEntry)
        Public Property EydCategories As DbSet(Of EydCategory)
        Public Property EydRules As DbSet(Of EydRule)

        Public Sub New()
            Me.New(DefaultDbPath())
        End Sub

        Public Sub New(dbPath As String)
            _dbPath = dbPath
        End Sub

        Public Shared Function DefaultDbPath() As String
            Return IO.Path.Combine(AppContext.BaseDirectory, "eyd.sqlite")
        End Function

        Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
            optionsBuilder.UseSqlite($"Data Source={_dbPath}")
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            modelBuilder.Entity(Of KbbiEntry)(
                Sub(e)
                    e.ToTable("KbbiEntries")
                    e.HasKey(Function(x) x.Id)
                    e.Property(Function(x) x.Kata).IsRequired()
                    e.HasIndex(Function(x) x.Kata).HasDatabaseName("IX_KbbiEntries_Kata")
                End Sub)

            modelBuilder.Entity(Of EydCategory)(
                Sub(e)
                    e.ToTable("EydCategories")
                    e.HasKey(Function(x) x.Id)
                    e.HasIndex(Function(x) New With {x.GroupSlug, x.Slug}).IsUnique().HasDatabaseName("IX_EydCategories_GroupSlug_Slug")
                    e.HasMany(Function(x) x.Rules).WithOne(Function(r) r.Category).HasForeignKey(Function(r) r.CategoryId)
                End Sub)

            modelBuilder.Entity(Of EydRule)(
                Sub(e)
                    e.ToTable("EydRules")
                    e.HasKey(Function(x) x.Id)
                    e.HasIndex(Function(x) New With {x.CategoryId, x.RuleNumber, x.SubLetter}).HasDatabaseName("IX_EydRules_CategoryId_RuleNumber_SubLetter")
                End Sub)
        End Sub

    End Class

End Namespace
