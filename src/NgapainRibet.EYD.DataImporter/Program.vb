Imports System
Imports NgapainRibet.EYD.Data
Imports NgapainRibet.EYD.DataImporter.EydImport
Imports NgapainRibet.EYD.DataImporter.KbbiImport

Module Program

    Sub Main(args As String())
        Dim repoRoot = ResolveArg(args, "--repo-root", IO.Path.GetFullPath(IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")))
        Dim kbbiJsonPath = ResolveArg(args, "--kbbi-json", IO.Path.Combine(repoRoot, "kbbi_v6_data.json"))
        Dim eydDir = ResolveArg(args, "--eyd-dir", IO.Path.Combine(repoRoot, "eyd"))
        Dim outPath = ResolveArg(args, "--out", IO.Path.Combine(repoRoot, "src", "NgapainRibet.EYD.Core", "eyd.sqlite"))

        Console.WriteLine($"KBBI JSON : {kbbiJsonPath}")
        Console.WriteLine($"EYD dir   : {eydDir}")
        Console.WriteLine($"Output db : {outPath}")

        If IO.File.Exists(outPath) Then
            IO.File.Delete(outPath)
        End If

        Using context As New EydDbContext(outPath)
            context.Database.EnsureCreated()

            Console.WriteLine("Importing KBBI entries...")
            Dim kbbiImporter As New KbbiJsonImporter()
            Dim kbbiCount = kbbiImporter.Import(kbbiJsonPath, context)
            Console.WriteLine($"  {kbbiCount} KBBI entries imported.")

            Console.WriteLine("Importing EYD rules...")
            Dim eydImporter As New EydImporter()
            Dim result = eydImporter.Import(eydDir, context)
            Console.WriteLine($"  {result.Categories} categories, {result.Rules} rules imported.")
        End Using

        Console.WriteLine("Done.")
    End Sub

    Private Function ResolveArg(args As String(), name As String, defaultValue As String) As String
        For i As Integer = 0 To args.Length - 2
            If String.Equals(args(i), name, StringComparison.OrdinalIgnoreCase) Then
                Return args(i + 1)
            End If
        Next
        Return defaultValue
    End Function

End Module
