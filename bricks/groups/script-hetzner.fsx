#I "../../packages/FSharp.Data/lib/net40"
#r "../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#I "../../packages/GroupProgress/lib/net40"
#r "../../packages/GroupProgress/lib/net40/GroupProgress.dll"
#I "../../packages/DotLiquid.1.8.0/lib/NET40"
#r "../../packages/DotLiquid.1.8.0/lib/NET40/DotLiquid.dll"

open System.IO
open System.Text

open GroupProgress
open DotLiquid

let scriptDir = __SOURCE_DIRECTORY__
let dynamicDir = Path.Combine(scriptDir, "..", "..", "dynamic")

type Standing = {
    Group : Main.Dtos.GroupProgress;
    OverallRating : Main.Dtos.StandingsEntry [];
    Ratings : Main.Dtos.StandingsEntry [] []}

let registerSafeType(t : System.Type) =
    let names = t.GetMembers() |> Array.map (fun m -> m.Name)
    Template.RegisterSafeType(t, names)

module Filters =
    // http://stackoverflow.com/questions/2916294/how-to-do-typeof-of-a-module-in-a-fsx-file
    type A = A

    let UtcNow (_ : obj) =
        System.DateTime.UtcNow
    let Dirty (_ : obj) solved rejected =
        if solved + rejected = 0 then 0 else 100 * rejected  / (solved + rejected)
    let Color (_ : obj) ac total =
        if total = 0 then 0 else 10 * ac / total
    let Letter i =
        char (int 'A' + i)
    let StringResult (i : System.Nullable<int>) =
        if i.HasValue then
            match i.Value with
            | 0 -> "+"
            | v when v > 0 -> "+" + string v
            | v -> string v
        else ""
    let ResultClass (i : System.Nullable<int>) =
        if i.HasValue then
            match i.Value with
            | 0 -> "Standings__cell--AC0"
            | v when v > 0 -> "Standings__cell--AC"
            | _ -> "Standings__cell--RJ"
        else ""

do
    // http://stackoverflow.com/questions/2916294/how-to-do-typeof-of-a-module-in-a-fsx-file    
    let aty = typeof<Filters.A>
    let name = aty.FullName.Substring(0, aty.FullName.Length - "+A".Length)
    let ty = aty.Assembly.GetType(name)
    Template.RegisterFilter(ty)

    Template.NamingConvention <- new NamingConventions.CSharpNamingConvention()
    registerSafeType(typeof<Main.Dtos.User>)
    registerSafeType(typeof<Main.Dtos.ProblemInfo>)
    registerSafeType(typeof<Main.Dtos.ContestUserResults>)
    registerSafeType(typeof<Main.Dtos.Contest>)
    registerSafeType(typeof<Main.Dtos.OverallUserResults>)
    registerSafeType(typeof<Main.Dtos.OverallResults>)
    registerSafeType(typeof<Main.Dtos.GroupProgress>)
    registerSafeType(typeof<Main.Dtos.StandingsEntry>)

    let template = Template.Parse(File.ReadAllText(Path.Combine(scriptDir, "progress.liquid"), Encoding.UTF8))

    let oplab = Main.Judge.CreateEjudge "oplab" (Path.Combine ("/home", "ejudge", "judges"))
    let timus = Main.Judge.CreateTimus ()
    let mccme = Main.Judge.CreateMccme ()

    let groupsDir = Path.Combine(dynamicDir, "groups")
    Directory.CreateDirectory(groupsDir) |> ignore

    ["crimson1314"; "crimson1415"; "gainsboro1415"; "nur1415"; "noyabrsk1415"]
    |> Seq.iter (fun n ->
        try
            let progress = Main.gather [oplab; timus; mccme] (Path.Combine(scriptDir, n + ".json"))  
            let overallRating = Main.sortOverallUsersByProblems progress.Users progress.Overall.Results
            let ratings = progress.Contests |> Array.map (fun c -> Main.sortContestUsersByProblems progress.Users c.Results)
            let bag = Hash.FromAnonymousObject({Group = progress; OverallRating = overallRating; Ratings = ratings})
            let result = template.Render(bag)
            File.WriteAllText(Path.Combine(groupsDir, n + ".html"), result, Encoding.UTF8)
        with 
            e -> System.Console.WriteLine(e))

