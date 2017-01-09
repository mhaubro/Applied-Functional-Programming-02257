namespace GuardedCommands.Frontend
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

         | Apply(f,[e1;e2]) when List.exists (fun x ->  x=f) ["-";"+";"*"; "="; "&&"]        
                            -> tcDyadic gtenv ltenv f e1 e2
         | Apply(f, es) -> tcNaryFunction gtenv ltenv f es

         | _                -> failwith "tcE: not supported yet"

   and tcMonadic gtenv ltenv f e = match (f, tcE gtenv ltenv e) with
                                   | ("-", ITyp) -> ITyp
                                   | ("!", BTyp) -> BTyp
                                   | _           -> failwith "illegal/illtyped monadic expression" 
   
   and tcDyadic gtenv ltenv f e1 e2 = match (f, tcE gtenv ltenv e1, tcE gtenv ltenv e2) with
                                      | (o, ITyp, ITyp) when List.exists (fun x ->  x=o) ["-";"+";"*"]  -> ITyp
                                      | (o, ITyp, ITyp) when List.exists (fun x ->  x=o) ["="] -> BTyp
                                      | (o, BTyp, BTyp) when List.exists (fun x ->  x=o) ["&&";"="]     -> BTyp 
                                      | _                      -> failwith("illegal/illtyped dyadic expression: " + f)

   and tcNaryFunction gtenv ltenv f es = let pts,ft =  match Map.tryFind f gtenv with
                                                       | Some(FTyp(pts, Some ft)) -> (pts,ft)
                                                       | _ -> failwith ("no declaration for function " + f)
                                         let ets = List.map (tcE gtenv ltenv) es
                                         if ets.Length<>pts.Length then failwith ("tcNaryFunction: Expected " + pts.Length.ToString() + " for function "+ f + " but had "+ets.Length.ToString())
                                         List.iter2 (fun e p -> if e=p then () else failwith ("tcNaryFunction: parameter type mismatch in call to function " + f)) pts ets
                                         ft
 
   and tcNaryProcedure gtenv ltenv f es = failwith "type check: procedures not supported yet"
      

/// tcA gtenv ltenv e gives the type for access acc on the basis of type environments gtenv and ltenv
/// for global and local variables 
   and tcA gtenv ltenv = 
         function 
         | AVar x         -> match Map.tryFind x ltenv with
                             | None   -> match Map.tryFind x gtenv with
                                         | None   -> failwith ("no declaration for : " + x)
                                         | Some t -> t
                             | Some t -> t            
         | AIndex(acc, e) -> failwith "tcA: array indexing not supported yes"
         | ADeref e       -> failwith "tcA: pointer dereferencing not supported yes"
 

   and tgC expDeclList gtenv ltenv = List.iter(fun (exp,stmList) -> match tcE gtenv ltenv exp with
                                                                    | BTyp -> List.iter (tcS gtenv ltenv) stmList 
                                                                    | _ -> failwith("Illegal typed Alternative Statement")
                                               ) expDeclList
    


/// tcS gtenv ltenv retOpt s checks the well-typeness of a statement s on the basis of type environments gtenv and ltenv
/// for global and local variables and the possible type of return expressions 
   and tcS gtenv ltenv = function                           
                         | PrintLn e -> ignore(tcE gtenv ltenv e)
                         //Tests an assignment. tcA gets the type of acc, while tcE gets the type of e.
                         | Ass(acc,e) -> if tcA gtenv ltenv acc = tcE gtenv ltenv e 
                                         then ()
                                         else failwith "illtyped assignment"                                

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
                                                                 else failwith "illtyped return"   
                                        | None    -> failwith "tcS: procedures not yet supported"
                         | _              -> failwith "tcS: this statement is not supported yet"

///Adds an element tuple (t,s) to a map gtenv
   and tcGDec gtenv = function  
                      | VarDec(t,s)               -> Map.add s t gtenv
                      | FunDec(topt,f, decs, stm) -> match topt with
                                                        | Some t -> let ts,ps = decs |> List.map (function | VarDec(t',a)-> (t',a)| _ -> failwith "tcGDec: Cannot have nested function declarations")
                                                                                     |> List.unzip
                                                                    let doubles = ps |> List.countBy (fun a -> a)
                                                                                     |> List.where (fun (a,i) -> i > 1)
                                                                    if not(List.isEmpty doubles) then failwith("tcGDec: The following parameters where declared more than once in function " + f + ":\n" + doubles.ToString())
                                                                    let ltenv = tcLDecs Map.empty decs
                                                                                |> Map.add "return" t
                                                                    
                                                                    let gtenv' = Map.add f (FTyp(ts,Some t)) gtenv
                                                                    ignore(tcS gtenv' ltenv stm)
                                                                    gtenv'
                                                        | None   -> failwith "procedure declarations not yet supported"

///Adds all elements from a list to a map. Classic functional iteration through list.
   and tcGDecs gtenv = function
                       | dec::decs -> tcGDecs (tcGDec gtenv dec) decs
                       | _         -> gtenv

   and tcLDec ltenv = function
                       | VarDec(t,s) -> Map.add s t ltenv
                       | FunDec(_)   -> failwith "tcLDec: function declarations are not permitted in this context"
   and tcLDecs ltenv = function
                       | dec::decs -> tcLDecs (tcLDec ltenv dec) decs
                       | _         -> ltenv


/// tcP prog checks the well-typeness of a program prog
   and tcP(P(decs, stms)) = let gtenv = tcGDecs Map.empty decs//Creates a decl list           
                            //Iterates through all statements with the list of decls, applying tcS <decls> Map.empty on all statements
                            List.iter (tcS gtenv Map.empty) stms

  
