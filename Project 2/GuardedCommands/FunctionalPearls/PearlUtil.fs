module PearlUtil
    open System.IO
    open ASTToGeneralTreeConverter
    open GeneralTreeToPostScript
    
    ///Helper function to read a sourcefile, parse it
    /// generate a tree from it and generate postscript code for that tree
    /// and finally write it to the target file.
    // Arguments w and h are passed along to generate pretty trees in PS
    let producePS w h (sourceFile, targetFile) =
        let target = (FileInfo(targetFile)).CreateText()
        GuardedCommands.Util.ParserUtil.parseFromFile sourceFile
                |> TreeFromPro
                |> createPostScript w h
                |> List.iter (function I i -> target.Write(i)  //Let the streamwriter handle converting integers to strings
                                     | S s -> target.Write(s))
        target.Flush()
        target.Close()
    
    let producePlusOpPS w h (sourceFile, targetFile) =
        let target = (FileInfo(targetFile)).CreateText()
        GuardedCommands.Util.ParserUtil.parseFromFile sourceFile
                |> TreeFromPro
                |> createPostScriptPlus w h
                |> (fun (s:string) -> target.Write(s))
        target.Flush()
        target.Close()

    let producePSString w h parseTree = createPostScriptConc w h parseTree
        

    let producePSStringBuilder w h parseTree =
        createPostScriptStringList w h parseTree
            |> List.fold (fun (sb:System.Text.StringBuilder) s -> sb.Append(s)) (new System.Text.StringBuilder())
    
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let compare f1 f2 i = let testf = (fun f j -> let starttime = sw.Elapsed
                                                  f j |> ignore
                                                  let endtime = sw.Elapsed
                                                  (starttime, endtime)) in
                          let t1, t2 = testf f1 i, testf f2 i
                          (snd t1 - fst t1, snd t2 - fst t2)

    let comparelist f1 f2 = List.map (fun i -> compare f1 f2 i)