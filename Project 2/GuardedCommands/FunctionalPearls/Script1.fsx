#load "../GuardedCommands/Script.fsx"
#load "Extents.fs"
#load "Trees.fs"
#load "TreeGenerator.fs"
#load "ASTToGeneralTreeConverter.fs"
#load "GeneralTreeToPostScript.fs"
#load "PearlUtil.fs"

open Extents
open ASTToGeneralTreeConverter
open GeneralTreeToPostScript
open TreeGenerator
open PearlUtil

System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__ + @"\GCfiles");;
       
["Ex1"; "Ex2";"Ex3"; "Ex4"; "Ex5"; "Ex6"; "Skip";
 "Ex7"; "fact"; "factRec"; "factCBV";
 "A0"; "A1"; "A2"; "A3";"A4"; "Swap"; "QuickSortV1"]
        |> List.map (fun f -> (f+".gc",f+".ps"))
        |> List.iter (produceASTPS 40 40)


produceASTPS 40 40 ("A0LongName.gc","A0LongName.ps");;