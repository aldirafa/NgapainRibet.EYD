Imports System
Imports NgapainRibet.EYD.Eyd
Imports NgapainRibet.EYD.Kbbi
Imports NgapainRibet.EYD.SpellCheck

Module Program

    Sub Main(args As String())
        Dim kbbi As New KbbiLookupService()
        Dim spellCheck As New SpellCheckService()
        Dim eyd As New EydRuleService()

        Console.WriteLine("=== KBBI Lookup: ""rumah"" ===")
        For Each entry In kbbi.FindExact("rumah")
            Console.WriteLine($"Kata: {entry.Kata}")
            For Each m In entry.Makna
                Console.WriteLine($"  - {m}")
            Next
        Next

        Console.WriteLine()
        Console.WriteLine("=== Spell Check: ""rumah"" vs ""rumahh"" ===")
        Console.WriteLine($"IsBaku(rumah)  = {spellCheck.IsBaku("rumah")}")
        Console.WriteLine($"IsBaku(rumahh) = {spellCheck.IsBaku("rumahh")}")
        Console.WriteLine("Suggestions for ""rumahh"":")
        For Each s In spellCheck.Suggest("rumahh")
            Console.WriteLine($"  - {s.Word} (distance={s.Distance})")
        Next

        Console.WriteLine()
        Console.WriteLine("=== EYD Rules: ""kata-turunan"" ===")
        For Each rule In eyd.GetRules("kata-turunan")
            Dim label = If(String.IsNullOrEmpty(rule.SubLetter), rule.RuleNumber.ToString(), $"{rule.RuleNumber}{rule.SubLetter}")
            Console.WriteLine($"{label}. {rule.RuleText}")
            If rule.Examples.Count > 0 Then
                Console.WriteLine($"   Misalnya: {String.Join(", ", rule.Examples)}")
            End If
        Next
    End Sub

End Module
