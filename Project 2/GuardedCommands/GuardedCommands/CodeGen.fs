namespace GuardedCommands.Backend
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

       | Apply("-", [e]) -> CE vEnv fEnv e @  [CSTI 0; SWAP; SUB]
       | Apply("!", [e]) -> CE vEnv fEnv e @  [NOT]

       | Apply("&&",[b1;b2]) -> let labend   = newLabel()
                                let labfalse = newLabel()
                                CE vEnv fEnv b1 @ [IFZERO labfalse] @ CE vEnv fEnv b2
                                @ [GOTO labend; Label labfalse; CSTI 0; Label labend]

       | Apply(o,[e1;e2]) when List.exists (fun x -> o=x) ["-";"+"; "*"; "="]
                             -> let ins = match o with
                                          | "-"  -> [SUB]
                                          | "+"  -> [ADD]
                                          | "*"  -> [MUL]
                                          | "="  -> [EQ] 
                                          | _    -> failwith "CE: this case is not possible"
                                CE vEnv fEnv e1 @ CE vEnv fEnv e2 @ ins
       | Apply(f,es)        -> match Map.tryFind f fEnv with
                                | Some(label,Some(t),pDecs) -> let ps = List.length es
                                                               let dps = List.length pDecs
                                                               if ps = dps then
                                                                  List.collect (CE vEnv fEnv) es
                                                                   @ [CALL(ps, label)]
                                                               else failwith("CE: The function " + f + " takes " + dps.ToString() + 
                                                                        " arguments, but it was only supplied with " + ps.ToString())
                                | None -> failwith ("CE: The function " + f + " has not been defined")
                                | _    -> failwith "CE: Cannot use procedure as function"

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
      let newEnv = (Map.add x (kind fdepth, typ) env, fdepth+i)
      let code = [INCSP i] // increment the stackposition by i, to leave room for the array
               @ [GETSP] // push the address of the current stack position
               @ [CSTI (i-1)] // push the size-1 of the array
               @ [SUB] // substract size-1 of the array from the address to get the address of the first element on the top of the stack
      (newEnv, code) // return the envirionment and code
    | _ -> 
      let newEnv = (Map.add x (kind fdepth, typ) env, fdepth+1)
      let code = [INCSP 1]
      (newEnv, code)



                      
/// CS vEnv fEnv s gives the code for a statement s on the basis of a variable and a function environment   
//Creates the code                      
//vEnv = variable environment, fenv = function
   let rec CS vEnv fEnv = function
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
                             vCode @ CSs vEnv' fEnv stms

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
            let code = CS (envf, fdepthf) fEnv body 
            [Label labf] @ code @ [RET (List.length paras-1)]
            (*  is it necesarry to handle local variables explicitly? will block handle it?
                tune in later to find out the answers to these and other similarly trivial questions.*)
       //The above and the following are adapted from MICRO-C
       let funcode = decs
                      |> List.choose (function | FunDec(topt, f, paras, stm) -> Some (topt, f, paras, stm) | _-> None)
                      |> List.map compileFun
                      |> List.collect (fun l->l)
       initCode @ CSs gvEnv fEnv stms @ [STOP]  @ funcode



