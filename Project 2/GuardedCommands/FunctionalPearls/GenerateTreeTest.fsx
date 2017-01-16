#load "Extents.fs"
#load "Trees.fs"
#load "TreeGenerator.fs"
#load "GeneralTreeToPostScript.fs"


open GeneralTreeToPostScript
open TreeGenerator

open System.IO
    ///Helper function to read a sourcefile, parse it
    /// generate a tree from it and generate postscript code for that tree
    /// and finally write it to the target file.
    // Arguments w and h are passed along to generate pretty trees in PS
let producePSgenTree w h tree targetFile =
        let target = (FileInfo(targetFile)).CreateText()
        tree
                |> createPostScript w h
                |> List.iter (function I i -> target.Write(i)  //Let the streamwriter handle converting integers to strings
                                     | S s -> target.Write(s))
        target.Flush()
        target.Close()

let producePSignore w h tree =
        let sb = new System.Text.StringBuilder()
        tree |> createPostScript w h
             //|> List.fold (fun (sb:System.Text.StringBuilder) -> (function I(i) -> sb.Append(i) | S(s) -> sb.Append(s))) sb
             |> ignore
        
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__ + @"\PSfiles");;
let n = 10000
let m = 100

#time
let treeList = List.init 100 (fun i -> (randomizedTree n m, n.ToString()+"_"+m.ToString()+"_"+i.ToString()+".ps"))
#time

printfn "with write"
#time
treeList
    |> List.iter (fun (t,s) -> producePSgenTree 40 40 t s)
#time

printfn "without write"
#time
treeList
    |> List.iter (fun (t,s) -> producePSignore 40 40 t)
#time
