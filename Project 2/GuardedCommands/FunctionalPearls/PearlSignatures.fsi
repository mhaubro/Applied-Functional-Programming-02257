namespace FunPearls

open Trees
open GuardedCommands.Frontend.AST

    module ASTToGeneralTreeConverter =
        val convertASTToGeneral : Program -> string tree
    module GeneralTreeToPostScript = 
        val createPostScript : string tree -> string
        