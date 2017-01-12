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
System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__;;

// Test of programs covered by the first task (Section 3.7):
List.iter exec ["Ex1.gc"; "Ex2.gc";"Ex3.gc"; "Ex4.gc"; "Ex5.gc"; "Ex6.gc"; "Skip.gc"];;
List.iter (failtest exec) ["Ex1ill.gc"; "Ex2ill.gc";"Ex3ill.gc"; "Ex4ill.gc"; "Ex5ill.gc"; "Ex6ill.gc"];;

// Test of programs covered by the second task (Section 4.3):
List.iter exec ["Ex7.gc"; "fact.gc"; "factRec.gc"; "factCBV.gc"];;
List.iter (failtest exec) ["Ex7ill.gc"; "factill.gc";"A0ill.gc"; "A1ill.gc"; "A2ill.gc"; "A3ill.gc" ];;

// Test of programs covered by the fourth task (Section 5.4):
List.iter exec ["A0.gc"; "A1.gc"; "A2.gc"; "A3.gc"];;
List.iter (failtest exec) ["Ex7ill.gc"; "factill.gc";"A0ill.gc"; "A1ill.gc"; "A2ill.gc"; "A3ill.gc" ];;

// Test of programs covered by the fifth task (Section 6.1):
List.iter exec ["A4.gc"; "Swap.gc"; "QuickSortV1.gc"];;

(* Cannot execute the following
// Test of programs covered by the fifth task (Section 7.4):
List.iter exec ["par1.gc"; "factImpPTyp.gc"; "QuickSortV2.gc"; "par2.gc"];;

// Test of programs covered by the fifth task using optimized compilation (Section 8.2):
List.iter execOpt ["par1.gc"; "factImpPTyp.gc"; "QuickSortV2.gc"; "par2.gc"];;
 *)