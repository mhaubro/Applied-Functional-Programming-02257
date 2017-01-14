module PearlUtil
    open System.IO
    open ASTToGeneralTreeConverter
    open GeneralTreeToPostScript
    
    let producePS w h (sourceFile, targetFile) =
        let target = (FileInfo(targetFile)).CreateText()
        GuardedCommands.Util.ParserUtil.parseFromFile sourceFile
                |> TreeFromPro
                |> createPostScript w h
                |> target.Write
        target.Flush()
        target.Close()