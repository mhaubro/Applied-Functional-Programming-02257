﻿namespace GuardedCommands.Frontend
// Michael R. Hansen 06-01-2016

open System
open Machine
open GuardedCommands.Frontend.AST

module TypeCheck = 

   /// tcE gtenv ltenv e gives the type for expression e on the basis of type environments gtenv and ltenv
   /// for global and local variables 
   let rec tcE gtenv ltenv = function                            
         | N _              -> ITyp   
         | B _              -> BTyp   
         | Access acc       -> tcA gtenv ltenv acc     
                   
         | Apply(f,[e]) when List.exists (fun x ->  x=f) ["!";"-"]  
                            -> tcMonadic gtenv ltenv f e        

         | Apply(f,[e1;e2]) when List.exists (fun x ->  x=f) ["-";"+";"*"; "="; "&&";"||";"<";">";"<>";"<="]        
                            -> tcDyadic gtenv ltenv f e1 e2
         | Apply(f, es) -> tcNaryFunction gtenv ltenv f es

         | _                -> failwith "tcE: not supported yet"

   and tcMonadic gtenv ltenv f e = match (f, tcE gtenv ltenv e) with
                                   | ("-", ITyp) -> ITyp
                                   | ("!", BTyp) -> BTyp
                                   | _           -> failwith "tcMonadic: illegal/illtyped monadic expression" 
   
   and tcDyadic gtenv ltenv f e1 e2 = match (f, tcE gtenv ltenv e1, tcE gtenv ltenv e2) with
                                      | (o, ITyp, ITyp) when List.exists (fun x ->  x=o) ["-";"+";"*"]  -> ITyp
                                      | (o, ITyp, ITyp) when List.exists (fun x ->  x=o) ["=";"<";">";"<>";"<="] -> BTyp
                                      | (o, BTyp, BTyp) when List.exists (fun x ->  x=o) ["&&";"=";"||"]     -> BTyp 
                                      | _                      -> failwith("tcDyadic: illegal/illtyped dyadic expression: " + f)
   ///Typecheck of function call, returns return type of function, fails between mismatch of type of or number of parameters supplied
   and tcNaryFunction gtenv ltenv f es = match tcNaryCall gtenv ltenv f es with
                                            | Some t -> t // return return type, for further type checking
                                            | None -> failwith"tcNaryFunction: Cannot use procedures as functions"

   ///Typecheck of procedure call, returns unit, fails on mismatch between type of or number of parameters supplied
   and tcNaryProcedure gtenv ltenv f es = match tcNaryCall gtenv ltenv f es with
                                            | Some t -> failwith"tcNaryProcedure: Cannot use functions as procedures"
                                            | None -> () // return  for further type checking

    ///Get the type, if any, of the function/procedure f, also check type and numbers of parameters with the supplied expressions
    and tcNaryCall gtenv ltenv f es =    let pts,ft =  match Map.tryFind f gtenv with   //find type in global type environment
                                                       | Some(FTyp(pts, ft)) -> (pts,ft) //extract list of parameter types and return type if any
                                                       | _ -> failwith ("tcNaryCall: no declaration for function/procedure " + f)
                                         //get types of expressions supplied on call
                                         let ets = List.map (tcE gtenv ltenv) es
                                         //check that the two lists are of equal length
                                         if ets.Length<>pts.Length then failwith ("tcNaryCall: Expected " + pts.Length.ToString() + " for function/procedure "+ f + " but had "+ets.Length.ToString())
                                         //check that each expression in the call has the same type as the formal parameter.
                                         List.iter2 (fun e p -> match e, p with                                                                  
                                                                | ATyp(vartyp, _), ATyp(vartyp2, _) //Check if one is of array-type. If it is, checks that both are either bool or int
                                                                    -> if vartyp = vartyp2 then () else failwith ("tcNaryCall: parameter type mismatch in call to function/procedure " + f)                                                                  
                                                                | _                                 //Otherwise simply compare types
                                                                    -> if e=p then () else failwith ("tcNaryCall: parameter type mismatch in call to function/procedure " + f)
                                                                    ) ets pts
                                         ft // return return type, if any for further type checking
      

   /// tcA gtenv ltenv e gives the type for access acc on the basis of type environments gtenv and ltenv
   /// for global and local variables 
   and tcA gtenv ltenv = 
         function 
         | AVar x         -> match Map.tryFind x ltenv with
                             | None   -> match Map.tryFind x gtenv with
                                         | None   -> failwith ("tcA: no declaration for : " + x)
                                         | Some t -> t
                             | Some t -> t            
         //Array Indexing
         | AIndex(AVar s, e) -> match Map.tryFind s ltenv with
                                | None   -> match Map.tryFind s gtenv with
                                            //Not found
                                            | None   -> failwith ("tcA: no declaration for : " + s)
                                            //Found in local vars
                                            | Some t -> tcE gtenv Map.empty e
                                //Found in global vars
                                | Some t -> tcE Map.empty ltenv e            

         //Error messages for unimplemented stuff
         | AIndex(ADeref e, _) -> failwith "tcA: Pointer dereferencing in array not supported yet"
         | AIndex(AIndex (a,b), _) -> failwith "tcA: Not possible to access array in array"
         | ADeref e       -> failwith "tcA: pointer dereferencing not supported yes"
 

   and tgC expDeclList gtenv ltenv = List.iter(fun (exp,stmList) -> match tcE gtenv ltenv exp with
                                                                    | BTyp -> List.iter (tcS gtenv ltenv) stmList 
                                                                    | _ -> failwith("tgC: Illegal typed Alternative Statement")
                                               ) expDeclList
    

   /// tcS gtenv ltenv retOpt s checks the well-typeness of a statement s on the basis of type environments gtenv and ltenv
   /// for global and local variables and the possible type of return expressions 
   and tcS gtenv ltenv = function                           
                         | PrintLn e -> ignore(tcE gtenv ltenv e)
                         //Tests an assignment. tcA gets the type of acc, while tcE gets the type of e.
                         | Ass(acc,e) -> if tcA gtenv ltenv acc = tcE gtenv ltenv e 
                                         then ()
                                         else failwith "tcS: illtyped assignment"                                

                         //Block is a subblock, where everything is tested
                         //Right now no extra declarations is supported - tests for blocks statements on parents decls
                         | Block([],stms)   -> List.iter (tcS gtenv ltenv) stms
                         | Block(decs,stms) -> List.iter (tcS gtenv (tcLDecs ltenv decs)) stms

                         //Adds alternative statements (If and while) below this line
                         | Alt(GC expDeclList)-> tgC expDeclList gtenv ltenv
                         | Do(GC expDeclList) -> tgC expDeclList gtenv ltenv
                         | Return e -> match e with
                                        | Some e' -> match Map.tryFind "return" ltenv with
                                                     | None   -> failwith "tcS: unexpected return"
                                                     | Some t -> if t = tcE gtenv ltenv e'
                                                                 then ()
                                                                 else failwith "tcS: illtyped return"   
                                        | None    -> failwith "tcS: procedures not yet supported"
                         | Call(name, paramList) -> tcNaryProcedure gtenv ltenv name paramList


   /// Adds an element tuple (t,s) to a map gtenv
   and tcGDec gtenv = function  
                      | VarDec(t,s)               -> tcVarDec t
                                                     Map.add s t gtenv
                      | FunDec(topt,f, decs, stm) -> match topt with
                                                        | Some t -> let ts,ps = decs |> List.map (function | VarDec(t',a)-> (t',a)| _ -> failwith "tcGDec: Cannot have nested function declarations")
                                                                                     |> List.unzip
                                                                    let doubles = ps |> Seq.countBy (fun a -> a)
                                                                                     |> Seq.where (fun (a,i) -> i > 1)
                                                                    if not(Seq.isEmpty doubles) then failwith("tcGDec: The following parameters where declared more than once in function " + f + ":\n" + doubles.ToString())
                                                                    let ltenv = tcFDecs Map.empty decs
                                                                                |> Map.add "return" t
                                                                    
                                                                    let ltenv2 = match stm with
                                                                                 | Block(decl, stml) -> tcLDecs ltenv decl
                                                                                 | _ -> ltenv                                                                                                                               
                                                                 

                                                                    let gtenv' = Map.add f (FTyp(ts,Some t)) gtenv
                                                                    ignore(tcS gtenv' ltenv2 stm)
                                                                    if not (checkReturnStatement gtenv' ltenv2 stm) then failwith "Doesn't return for all branches"
                                                                    gtenv'

                                                        | None   -> let ts,ps = decs |> List.map (function | VarDec(t',a)-> (t',a)| _ -> failwith "tcGDec: Cannot have nested function declarations")
                                                                                     |> List.unzip
                                                                    let doubles = ps |> Seq.countBy (fun a -> a)
                                                                                     |> Seq.where (fun (a,i) -> i > 1)
                                                                    if not(Seq.isEmpty doubles) then failwith("tcGDec: The following parameters where declared more than once in function " + f + ":\n" + doubles.ToString())
                                                                    let ltenv = tcFDecs Map.empty decs
                                                                    
                                                                    let ltenv2 = match stm with
                                                                                 | Block(decl, stml) -> tcLDecs ltenv decl
                                                                                 | _ -> ltenv                                                                                                                               
                                                                 

                                                                    let gtenv' = Map.add f (FTyp(ts,None)) gtenv
                                                                    ignore(tcS gtenv' ltenv2 stm)
                                                                    gtenv'
                                                        

   and checkReturnStatement gtenv ltenv = function
                                           | Return (Some(stm)) -> (tcS gtenv ltenv (Return (Some(stm))))
                                                                   true                                                  
                                           | Block (decList, stmlist) -> List.exists (checkReturnStatement gtenv ltenv) stmlist
                                           //Checks for all branches in an alternate statement, that there is at least one return. If there is, the function will return
                                           | Alt(GC(gclist)) -> List.forall (fun (_, stmlist) -> List.exists (checkReturnStatement gtenv ltenv) stmlist) gclist
                                           //Same as above
                                           | Do(GC(gclist))  -> List.forall (fun (_, stmlist) -> List.exists (checkReturnStatement gtenv ltenv) stmlist) gclist
                                           //If it is not a return statement
                                           | _ -> false

    ///Adds all elements from a list to a map. Classic functional iteration through list.
   and tcGDecs gtenv = function
                       | dec::decs -> tcGDecs (tcGDec gtenv dec) decs
                       | _         -> gtenv
    ///Checks if a type declared in a block or the main program has a negative length/no length
   and tcVarDec typedec = match typedec with
                          | ATyp(a, Some(b)) -> if b < 0 then failwith "tcVarDec: Array with negative length declared" else ()
                          | ATyp(a, None)    -> failwith "tcVarDec: Must declare length for array"
                          | _ -> ()
   and tcVarDecFunc typedec = match typedec with
                              | ATyp(a, Some(b)) -> failwith "tcVarDecFunc: Array in function parameter with length declared"
                              | _ -> ()

    ///For block type-check
   and tcLDec ltenv = function
                       | VarDec(t,s) -> tcVarDec t
                                        Map.add s t ltenv
                       | FunDec(_)   -> failwith "tcLDec: function declarations are not permitted in this context"
   and tcLDecs ltenv = function
                       | dec::decs -> tcLDecs (tcLDec ltenv dec) decs
                       | _         -> ltenv

   and tcFDec ltenv = function
                      | VarDec(t,s) -> tcVarDecFunc t
                                       Map.add s t ltenv
                      | _ -> failwith "tcFDec: Declaration in function is not a variable"

   and tcFDecs ltenv = function
                       | dec::decs -> tcFDecs (tcFDec ltenv dec) decs
                       | _         -> ltenv
                        

   /// tcP prog checks the well-typeness of a program prog
   and tcP(P(decs, stms)) = let gtenv = tcGDecs Map.empty decs//Creates a decl list           
                            //Iterates through all statements with the list of decls, applying tcS <decls> Map.empty on all statements
                            List.iter (tcS gtenv Map.empty) stms
