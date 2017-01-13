module PearlUtil
    open System.IO
    open ASTToGeneralTreeConverter
    open GeneralTreeToPostScript
    
    let producePS w (sourceFile, targetFile) =
        let target = (FileInfo(targetFile)).CreateText()
        GuardedCommands.Util.ParserUtil.parseFromFile sourceFile
                |> TreeFromPro
                |> createPostScript w
                |> target.Write
        target.Flush()
        target.Close()