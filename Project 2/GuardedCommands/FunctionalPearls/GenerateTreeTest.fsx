#load "Extents.fs"
#load "Trees.fs"
#load "TreeGenerator.fs"
#load "GeneralTreeToPostScript.fs"

open GeneralTreeToPostScript
open TreeGenerator
        
let n = 100
let m = 3
let l = 10

let treeListList = List.init n (fun i -> i,List.init l (fun _ -> randomizedTree (100 * i) m))

let sw = System.Diagnostics.Stopwatch.StartNew()
let compare f1 f2 i = let testf = (fun f j -> //sw.Reset()
                                              //sw.Restart()
                                              let starttime = sw.Elapsed.TotalMilliseconds
                                              f j |> ignore
                                              let endtime = sw.Elapsed.TotalMilliseconds
                                              (starttime, endtime)) in
                      let t1, t2 = testf f1 i, testf f2 i
                      (snd t1 - fst t1, snd t2 - fst t2)

let comparelist f1 f2 = List.map (fun i -> compare f1 f2 i)
let lf = (float) l
treeListList
   |> List.map (fun (i,list) -> list
                                |> comparelist (createPostScriptConc 40 40) (createPostScriptPlus 40 40)
                                |> List.fold (fun (a,b) (x, y) -> (a+x,b+y)) (0.0,0.0)
                                |> (fun (a,b) -> (i,((float) a / lf, (float) b / lf))))
