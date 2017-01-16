module PearlUtil
    open System.IO
    open ASTToGeneralTreeConverter
    open GeneralTreeToPostScript
    open GeneralTreeToPostScriptAlt
    
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
    
    ///Helper function to read a sourcefile, parse it
    /// generate a tree from it and generate postscript code for that tree
    /// and finally write it to the target file.
    // Arguments w and h are passed along to generate pretty trees in PS
    let producePSAlt w h (sourceFile, targetFile) =
        let target = (FileInfo(targetFile)).CreateText()
        GuardedCommands.Util.ParserUtil.parseFromFile sourceFile
                |> TreeFromPro
                |> createPostScriptAlt w h
                |> target.Write
        target.Flush()
        target.Close()

    
    ///Helper function to read a sourcefile, parse it
    /// generate a tree from it and generate postscript code for that tree
    /// and finally write it to the target file.
    // Arguments w and h are passed along to generate pretty trees in PS
    let producePSgenTree w h tree targetFile =
        let target = (FileInfo(targetFile)).CreateText()
        tree
                |> createPostScript w h
                |> List.iter (function I i -> target.Write(i)  // Let the streamwriter handle converting integers to strings
                                     | S s -> target.Write(s)) // Simply write when it is a string
        target.Flush()
        target.Close()

    let sw = System.Diagnostics.Stopwatch.StartNew()
    let compare f1 f2 i = let testf = (fun f j -> let starttime = sw.Elapsed
                                                  f j |> ignore
                                                  let endtime = sw.Elapsed
                                                  (starttime, endtime)) in
                          let t1, t2 = testf f1 i, testf f2 i
                          (snd t1 - fst t1, snd t2 - fst t2)

    let comparelist f1 f2 = List.map (fun i -> compare f1 f2 i)