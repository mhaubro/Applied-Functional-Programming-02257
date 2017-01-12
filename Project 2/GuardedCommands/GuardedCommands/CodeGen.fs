﻿namespace GuardedCommands.Backend
// Michael R. Hansen 05-01-2016
// This file is obtained by an adaption of the file MicroC/Comp.fs by Peter Sestoft
open System
open Machine

open GuardedCommands.Frontend.AST
module CodeGeneration =


(* A global variable has an absolute address, a local one has an offset: *)
   type Var = 
     | GloVar of int                   (* absolute address in stack           *)
     | LocVar of int                   (* address relative to bottom of frame *)

(* The variable environment keeps track of global and local variables, and 
   keeps track of next available offset for local variables *)

   type varEnv = Map<string, Var*Typ> * int

(* The function environment maps function name to label and parameter decs *)

   type ParamDecs = (Typ * string) list
   type funEnv = Map<string, label * Typ option * ParamDecs>

/// CE vEnv fEnv e gives the code for an expression e on the basis of a variable and a function environment
   let rec CE vEnv fEnv = 
       function
       | N n          -> [CSTI n]
       | B b          -> [CSTI (if b then 1 else 0)]
       | Access acc   -> CA vEnv fEnv acc @ [LDI] 

        // code generations for unary functions i.e. negation of ints and booleans
       | Apply("-", [e]) -> CE vEnv fEnv e @  [CSTI 0; SWAP; SUB]
       | Apply("!", [e]) -> CE vEnv fEnv e @  [NOT]

        // code geneartion for binary operators - at least those without short circuit semantics
       | Apply(o,[e1;e2]) when List.exists (fun x -> o=x) ["-";"+"; "*"; "=";"<";">";"<=";"<>"]
                             -> let ins = match o with
                                          | "-"  -> [SUB]
                                          | "+"  -> [ADD]
                                          | "*"  -> [MUL]
                                          | "="  -> [EQ] 
                                          | "<"  -> [LT]
                                          | ">"  -> [SWAP; LT; NOT] 
                                          | "<="  -> [SWAP; LT; NOT]
                                          | ">="  -> [LT; NOT]
                                          | "<>"  -> [EQ; NOT]
                                          | _    -> failwith "CE: this case is not possible"
                                CE vEnv fEnv e1 @ CE vEnv fEnv e2 @ ins

         // code generation for composition of boolean statements with conjunction and disjunction
       | Apply(o,[e1;e2]) when List.exists (fun x -> o=x) ["&&";"||"]
                             -> let label = newLabel();
                                let jt = match o with // we jump differently depending on wether true or false should dominate
                                              | "&&" -> [IFZERO label]
                                              | "||" -> [IFNZRO label]
                                              | _    -> failwith "CE: this case is not possible"
                                CE vEnv fEnv e1     // Evaluate first expression
                              @ [DUP] @ jt          // duplicate and jump if value of first dominates
                              @ [INCSP -1]          // if no jump, then second of second dominates, so first value can be disregarded
                              @ CE vEnv fEnv e2     // evaluate second expression
                              @ [Label label]       // label used for jumping

        // code generation for application of programatically defined functions
       | Apply(f,es)        -> match Map.tryFind f fEnv with                                // get function
                                | Some(label,Some(t),pDecs) -> let ps = List.length es
                                                               List.collect (CE vEnv fEnv) es    // evaluate parameter values
                                                                @ [CALL(ps, label)]              // perform function call
                                | _    -> failwith "CE: Please perform typecheck to find out what you did wrong"

       | _            -> failwith "CE: not supported yet"
       

/// CA vEnv fEnv acc gives the code for an access acc on the basis of a variable and a function environment
   and CA vEnv fEnv = function | AVar x         -> match Map.find x (fst vEnv) with
                                                   | (GloVar addr,_) -> [CSTI addr]
                                                   | (LocVar addr,_) -> [GETBP; CSTI addr; ADD]
                               | AIndex(acc, e) -> CA vEnv fEnv acc // push the adress of the array to the stack
                                                 @ [LDI] // goes to the address of the index
                                                 @ CE vEnv fEnv e // push the index from the expression to the stack                                                                                                  
                                                 @ [ADD] // adds the index to the array pointer

                               | ADeref e       -> failwith "CA: pointer dereferencing not supported yet"

  
(* Bind declared variable in env and generate code to allocate it: *)   
   let allocate (kind : int -> Var) (typ, x) (vEnv : varEnv)  =
    let (env, fdepth) = vEnv 
    match typ with
    | ATyp (ATyp _, _) -> 
      raise (Failure "allocate: array of arrays not permitted")
    | ATyp (t, Some i) -> 
      let newEnv = (Map.add x (kind (fdepth+i), typ) env, fdepth+i+1)
      let code = [INCSP i] // increment the stackposition by i, to leave room for the array
               @ [GETSP] // push the address of the current stack position
               @ [CSTI (i-1)] // push the size-1 of the array
               @ [SUB] // substract size-1 of the array from the address to get the address of the first element on the top of the stack
      (newEnv, code) // return the envirionment and code
    | _ -> 
      let newEnv = (Map.add x (kind fdepth, typ) env, fdepth+1)
      let code = [INCSP 1]
      (newEnv, code)
   let rec CSF vEnv fEnv = function
                            //Gets code for an expression e, prints, reduces stack with 1 (Removes last thing pushed, the value of e)
       | PrintLn e        -> CE vEnv fEnv e @ [PRINTI; INCSP -1] 

       | Ass(acc,e)       -> CA vEnv fEnv acc @ CE vEnv fEnv e @ [STI; INCSP -1]

       //If-statement. 
       | Alt(gcl)         -> CSalt vEnv fEnv gcl
       //Do-while statement. 
       | Do(gcl)          -> CSrep vEnv fEnv gcl

       | Block([],stms)   -> CSs vEnv fEnv stms
       | Block(decs,stms) -> let vEnv',vCode = decs
                                                |> List.choose (function |VarDec(t,s)->Some(t,s)|_->None)
                                                |> List.fold (fun (vE,code) dec -> let (vE',code') = allocate LocVar dec vE in (vE',code@code')) (vEnv,[])
                             let decsSize = List.fold(fun size dec -> match dec with
                                                                          | VarDec(ATyp(_,Some(i)),_) -> size + i + 1
                                                                          | VarDec(_,_) -> size + 1
                                                                          | _ -> failwith "CS: invalid declaration in Block statement") 0 decs
                             vCode @ CSs vEnv' fEnv stms @
                             [INCSP -decsSize]
                             
       | Call(f,es)        -> match Map.tryFind f fEnv with                                // get function
                                | Some(label,Some(t),pDecs) -> let ps = List.length es
                                                               List.collect (CE vEnv fEnv) es    // evaluate parameter values
                                                                @ [CALL(ps, label)]              // perform function call
                                | _    -> failwith "CE: Please perform typecheck to find out what you did wrong"

       | Return (Some e)        -> CE vEnv fEnv e @ [RET (snd vEnv)] //snd vEnv contains the height of the current frame on the stack
       | Return None            -> [RET (snd vEnv - 1)]
                                                          
       | _                -> failwith "CS: this statement is not supported yet"


                      
/// CS vEnv fEnv s gives the code for a statement s on the basis of a variable and a function environment   
//Creates the code                      
//vEnv = variable environment, fenv = function
   and CS vEnv fEnv = function
                            //Gets code for an expression e, prints, reduces stack with 1 (Removes last thing pushed, the value of e)
       | PrintLn e        -> CE vEnv fEnv e @ [PRINTI; INCSP -1] 

       | Ass(acc,e)       -> CA vEnv fEnv acc @ CE vEnv fEnv e @ [STI; INCSP -1]

       //If-statement. 
       | Alt(gcl)         -> CSalt vEnv fEnv gcl
       //Do-while statement. 
       | Do(gcl)          -> CSrep vEnv fEnv gcl

       | Block([],stms)   -> CSs vEnv fEnv stms
       | Block(decs,stms) -> let vEnv',vCode = decs
                                                |> List.choose (function |VarDec(t,s)->Some(t,s)|_->None)
                                                |> List.fold (fun (vE,code) dec -> let (vE',code') = allocate GloVar dec vE in (vE',code@code')) (vEnv,[])
                             let decsSize = List.fold(fun size dec -> match dec with
                                                                          | VarDec(ATyp(_,Some(i)),_) -> size + i + 1
                                                                          | VarDec(_,_) -> size + 1
                                                                          | _ -> failwith "CS: invalid declaration in Block statement") 0 decs
                             vCode @ CSs vEnv' fEnv stms @
                             [INCSP -decsSize]
                             

       | Return (Some e)        -> CE vEnv fEnv e @ [RET (snd vEnv)] //snd vEnv contains the height of the current frame on the stack
       | Return None            -> [RET (snd vEnv - 1)]
                                                          
       | _                -> failwith "CS: this statement is not supported yet"
       //CSs is the function called in CS, creating everythin.
       //List.Collect -> CS vEnv fEnv is done for every element, the results concatednated and returned in a new list
   and CSs vEnv fEnv stms = List.collect (CS vEnv fEnv) stms 


   ///Transforms if..fi to code
       //Strategy: 
       //All the way through, Statement b is written code for. If b = 0, jump to next bool statement.
       //If b = 1, execute somecode ending with goto end line of code (of if)
   and CSalt vEnv fEnv = function
       | GC []               -> [STOP]
       | GC gcl              -> let lastLabel = newLabel() in    //make label for end of this if..fi
                                let nextLabel = ref "" in        //make ref to hold next label in this if..fi
                                let currLabel = ref lastLabel in //make ref to hold current label in this if..fi
                                List.foldBack (fun (b, sl) c ->      // fold back so last label ends up last - could also be done forwardslike for optimization (superfluous label at very begining)
                                                nextLabel := !currLabel     //folding back, so make sure to goto currlabel next 
                                                currLabel := newLabel()     //make new label for current
                                                [Label !currLabel] @        //label this position in case previous guard was TRUE
                                                CE vEnv fEnv b @            //evaluate guard, leaves value on top of stack
                                                [IFZERO !nextLabel] @       //if FALSE continue to next guard
                                                CSs vEnv fEnv sl @          //otherwise evaluate statements
                                                [GOTO lastLabel] @          //and leave this if..fi
                                                 c)                         // and collect code
                                              gcl [STOP; Label lastLabel]   //terminate if no applicable guard otherwise there was a goto lastlabel

   ///Transforms do..od to code
       //Strategy: Statement b is written code for. If b = 0 -> next bool statement.
       //If b = 1, execute some code, goto start line of code (of do).
   and CSrep vEnv fEnv = function
       | GC []               -> []  
       | GC gcl              -> let firstLabel = newLabel() in
                                let lastLabel = newLabel() in
                                let nextLabel = ref "" in
                                let currLabel = ref lastLabel in
                                [Label firstLabel] @
                                List.foldBack (fun (b, sl) c ->
                                                nextLabel := !currLabel
                                                currLabel := newLabel()
                                                [Label !currLabel] @
                                                CE vEnv fEnv b @
                                                [IFZERO !nextLabel] @
                                                CSs vEnv fEnv sl @ 
                                                [GOTO firstLabel] @
                                                 c)
                                              gcl [Label lastLabel]
(* ------------------------------------------------------------------- *)

(* Build environments for global variables and functions *)
//Obs: This function is running despite not implementing as/if, since it is only related to variable declaration
   let makeGlobalEnvs decs = 
       //Function definition
       let rec addv decs vEnv fEnv = 
           match decs with 
           | []         -> (vEnv, fEnv, [])
           | dec::decr  -> 
             match dec with
             //Variable declaration in guardedcommands-code
             | VarDec (typ, var) -> //Allokerer dec - bliver kørt på samtlige elementer
                                    let (vEnv1, code1) = allocate GloVar (typ, var) vEnv
                                    //Allokerer decs ved at køre rekursivt på resten
                                    let (vEnv2, fEnv2, code2) = addv decr vEnv1 fEnv
                                    //Returns global environment, which is a list of code
                                    (vEnv2, fEnv2, code1 @ code2)
             //inspired by MICRO-C, we simply add the function to the environment, saving a label for it. Compiling functions comes later.
             | FunDec (tyOpt, f, xs, body) -> addv decr vEnv (Map.add f (newLabel(),tyOpt,List.choose (function | VarDec(t,s)->Some(t,s)|_-> None) xs) fEnv)
       //Return element
       addv decs (Map.empty, 0) Map.empty

(* Bind declared parameters in env: *)

   let bindParam (env, fdepth) (typ, x)  : varEnv =
       let env' = env |> Map.add x (LocVar fdepth, typ)
       (env', fdepth+1)

   let bindParams paras ((env, fdepth) : varEnv) : varEnv = 
    List.fold bindParam (env, fdepth) paras;

//ENTRY POINT
/// CP prog gives the code for a program prog
   let CP (P(decs,stms)) = 
       resetLabels()
       //(It seems that "(gvM, _) as" is unnecessary, since gvM isn't used?//MH --- den skal bruges til funktionerne :)
       let ((gvM,_) as gvEnv, fEnv, initCode) = makeGlobalEnvs decs
       let compileFun (tyOpt, f, xs, body) =
            let (labf, _, paras) = Map.find f fEnv
            let (envf, fdepthf) = bindParams paras (gvM, 0)//<- altså lige her!
            let code = CSF (envf, fdepthf) fEnv body 
            [Label labf] @ code @ [RET (fdepthf-1)] //return in case of procedure - in function they are enforced by typecheck
            (*  is it necesarry to handle local variables explicitly? will block handle it?
                tune in later to find out the answers to these and other similarly trivial questions.*)
       //The above and the following are adapted from MICRO-C
       let funcode = decs
                      |> List.choose (function 
                                        | FunDec(topt, f, paras, stm) -> Some (topt, f, paras, stm) 
                                        | _-> None)
                      |> List.map compileFun
                      |> List.collect (fun l->l)
       initCode @ CSs gvEnv fEnv stms @ [STOP] @ funcode



