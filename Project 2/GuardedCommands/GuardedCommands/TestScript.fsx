// Michael R. Hansen 05-01-2016

// You must revise 4 pathes occurring in this file 
// The first three are:

#if INTERACTIVE
#r "bin/Debug/FSharp.PowerPack.dll"
#r "bin/Debug/Machine.dll"
#r "bin/Debug/VirtualMachine.dll";
#endif

#load "AST.fs"
#load "Parser.fs"
#load "Lexer.fs"
#load "TypeCheck.fs"
#load "CodeGen.fs"
#load "CodeGenOpt.fs"
#load "Util.fs"
 

open GuardedCommands.Util
open GuardedCommands.Frontend.TypeCheck
open GuardedCommands.Frontend.AST
open GuardedCommands.Backend.CodeGeneration

open ParserUtil
open CompilerUtil

open Machine
open VirtualMachine

// You must revise this path
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__ + "//GCfiles");;
try
    List.iter (fun (f,v) -> List.iter f v) 
                    [( exec,           ["Ex1.gc"; "Ex2.gc";"Ex3.gc"; "Ex4.gc"; "Ex5.gc"; "Ex6.gc"; "Skip.gc";"Ex7.gc"; "fact.gc"; "factRec.gc"; "factCBV.gc";"LocalBlockTest.gc";"A0.gc"; "A1.gc"; "A2.gc"; "A3.gc"]);
                    (( failtest exec), ["Ex1ill.gc"; "Ex2ill.gc";"Ex3ill.gc"; "Ex4ill.gc"; "Ex5ill.gc"; "Ex6ill.gc"; "Ex7ill.gc"; "factill.gc";"A0ill.gc"; "A1ill.gc"; "A2ill.gc"; "A3ill.gc" ])]
with | Failure(msg) ->  printf"!!!!!!!!!!! SOME TEST FAILED !!!!!!!!!!!"

