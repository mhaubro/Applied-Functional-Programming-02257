#load @"C:\Users\Krarup\Source\Repos\Applied-Functional-Programming-02257\Project 2\GuardedCommands\GuardedCommands\Script.fsx"
#load "Extents.fs"
#load "Trees.fs"
#load "ASTToGeneralTreeConverter.fs"
#load "GeneralTreeToPostScript.fs"
#load "PearlUtil.fs"

open Extents
open ASTToGeneralTreeConverter
open GeneralTreeToPostScript
open PearlUtil

System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__ + @"\GCfiles");;

//let factRecASTree = GuardedCommands.Util.ParserUtil.parseFromFile("factRec.gc")
//let factRecTree = TreeFromPro(factRecASTree)
//Trees.design( factRecTree) ;;
//Trees.intDesign 100.0 (factRecTree) ;;

//let rec flattenTree = function Trees.Node(l,st) -> l::(List.collect flattenTree st)
//createPostScript factRecTree
//flattenTree factRecTree;;


["Ex7"; "fact"; "factRec"; "factCBV"]
    |> List.map (fun f -> (f+".gc",f+".ps"))
    |> List.iter (producePS 40 40)