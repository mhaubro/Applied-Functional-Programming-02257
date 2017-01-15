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