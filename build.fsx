// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
#r @"packages/DotLiquid.1.8.0/lib/NET40/DotLiquid.dll"

open System.IO
open System.Text

open Fake
open DotLiquid

let iDir = "./intermediate"
let bDir = "./build"

let buildPage title source destination =
    let body = File.ReadAllText(source, Encoding.UTF8)
    let template = Template.Parse(File.ReadAllText("./bricks/layout/page.liquid", Encoding.UTF8))
    let bag = Hash.FromDictionary(dict [("title", title :> obj); ("body", body :> obj)])
    File.WriteAllText(iDir @@ destination, template.Render(bag), Encoding.UTF8)

let callGulp target = 
    let processName = sprintf "./node_modules/.bin/gulp%s" (if EnvironmentHelper.isLinux then "" else ".cmd")
    ExecProcess (fun info ->
        info.FileName <- processName
        info.Arguments <- target) System.TimeSpan.MaxValue
    |> ignore

let buildOlymp () =
    CopyFiles (iDir @@ "css/") (!! "./bricks/olymp/*.css")
    CopyDir bDir "./bricks/olymp" (fun n -> n.EndsWith("7z") || n.EndsWith("pdf"))

    buildPage "турниры()" "./bricks/olymp/olymp.html" "olymp.html"
    CreateDir (iDir @@ "olymp")

    CreateDir (iDir @@ "olymp" @@ "ejik")
    buildPage "ёjik(2013)" "./bricks/olymp/olymp/ejik/2013.html" "olymp/ejik/2013.html"
    buildPage "ёjik(2014-весна)" "./bricks/olymp/olymp/ejik/2014s.html" "olymp/ejik/2014s.html"
    buildPage "ёjik(2014-осень)" "./bricks/olymp/olymp/ejik/2014a.html" "olymp/ejik/2014a.html"

    CreateDir (iDir @@ "olymp" @@ "arhimed")
    buildPage "архимед(2013)" "./bricks/olymp/olymp/arhimed/2013.html" "olymp/arhimed/2013.html"

    CreateDir (iDir @@ "olymp" @@ "contest.kpml")
    buildPage "contest.kpml.ru(2013)" "./bricks/olymp/olymp/contest.kpml/2013.html" "olymp/contest.kpml/2013.html"
    buildPage "contest.kpml.ru(2014)" "./bricks/olymp/olymp/contest.kpml/2014.html" "olymp/contest.kpml/2014.html"

Target "BuildStaticPages" (fun _ ->
    buildPage "главная()" "./bricks/index/index.html" "index.html"
    buildPage "задачи()" "./bricks/ejudge/ejudge.html" "ejudge.html"
    buildPage "фмл()" "./bricks/courses/kpml.html" "kpml.html"
    buildPage "рои(2014-2015)" "./bricks/roi/roi.html" "roi.html"
    buildOlymp ()

    CopyFile (iDir @@ "css/") "./bricks/groups/progress.css"
    buildPage "группы()" "./bricks/groups/groups.html" "groups.html"
    buildPage "отбор(УРКОП-2013)" "./bricks/groups/urkop.html" "urkop.html"
)

Target "CopyAssets" (fun _ ->
    let cssFiles = !! "./bricks/layout/*.css"
    CopyFiles (iDir @@ "css/") cssFiles
    CopyFile (bDir) "./bricks/layout/favicon.ico"
)

Target "GulpDefault" (fun _ ->
    callGulp "default"
)

Target "BuildDynamicPages" (fun _ ->
    buildPage "группа(crimson, 1314)" "./dynamic/groups/crimson1314.html" "crimson1314.html"
    buildPage "группа(crimson, 1415)" "./dynamic/groups/crimson1415.html" "crimson1415.html"
    buildPage "группа(gainsboro, 1415)" "./dynamic/groups/gainsboro1415.html" "gainsboro1415.html"
    buildPage "группа('Новый Уренгой', 1415)" "./dynamic/groups/nur1415.html" "nur1415.html"
    buildPage "группа(Ноябрьск)" "./dynamic/groups/noyabrsk1415.html" "noyabrsk1415.html"
    buildPage "группа(Зимняя школа 2015, A)" "./dynamic/groups/yamal15.html" "yamal15.html"
)

Target "Clean" (fun _ ->
    CleanDir iDir
    CleanDir bDir
)

Target "Default" (fun _ ->
    callGulp "default"
//    CleanDir iDir
//    DeleteDir iDir
)

Target "ScheduledUpdate" (fun _ ->
    callGulp "html"
//    CleanDir iDir
//    DeleteDir iDir
)

"Clean" ==> "BuildStaticPages" ==> "CopyAssets" ==> "Default"
"BuildDynamicPages" ==> "ScheduledUpdate"

// start build
RunTargetOrDefault "Default"
