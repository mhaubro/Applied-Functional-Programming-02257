//namespace FunPearls
    module ASTToGeneralTreeConverter
        open GuardedCommands.Frontend.AST
        open Trees
        let nameFormat s = String.concat "" ["["; s; "]"]
        let vardecFormat s = String.concat "" ["Var:["; s; "]"]
        let fundecFormat s = String.concat "" ["Fun:["; s; "]"]
        let procdecFormat (s:string) = String.concat "" ["Proc:["; s; "]"]


        let rec TreeFromExp = function
                                | Access acc        -> Node("Access", [TreeFromAcc acc])
                                | Addr acc          -> Node("Address", [TreeFromAcc acc])
                                | Apply(f,es) when List.exists (fun x ->  x=f) ["!";"-";"+";"*"; "="; "&&";"||";"<";">";"<>";"<="]
                                                    -> Node(f, List.map TreeFromExp es)
                                | Apply(f,es)       -> Node(nameFormat f, List.map TreeFromExp es)
                                | N i               -> Node(i.ToString(),[])
                                | B b               -> Node(b.ToString(),[])

        and TreeFromAcc = function
                                | AIndex(acc,e)     -> Node("AIndex",
                                                           [TreeFromAcc acc;
                                                            TreeFromExp e ])
                                | AVar x            -> Node (nameFormat x,[])
                                | ADeref e          -> Node("ADeref", [TreeFromExp e])

        and TreeFromStm = function
                                | PrintLn e         -> Node("PrintLn",[TreeFromExp e])
                                | Ass(acc,e)        -> Node("Assign",
                                                           [TreeFromAcc acc;
                                                            TreeFromExp e ])
                                | Return (Some e)   -> Node("Return",[TreeFromExp e])
                                | Return None       -> Node("Return",[])
                                | Alt (GC [])       -> Node("Abort",[])
                                | Alt (GC gcl)      -> Node("Alt",List.map TreeFromGC gcl)
                                | Do (GC [])        -> Node("Skip",[])
                                | Do (GC gcl)       -> Node("Rep",List.map TreeFromGC gcl)
                                | Block([],stml)    -> Node("Block",[TreeFromStml stml])
                                | Block(decl,stml)  -> Node("Block",
                                                           [TreeFromDecl decl;
                                                            TreeFromStml stml])
                                | Call(f,es)        -> Node(nameFormat f, List.map TreeFromExp es)
        and TreeFromStml stml = Node("Body",List.map TreeFromStm stml)
        and TreeFromGC = function
                                | e,stml            -> Node("GC",
                                                           [Node("guard", [TreeFromExp e]);
                                                            TreeFromStml stml])
    
        and TreeFromDec = function
                                | VarDec(t,s)       -> Node(vardecFormat s,[TreeFromTyp t])
                                | FunDec(Some t, s,
                                          decl, stm)-> Node(fundecFormat s,
                                                                    [TreeFromArgs decl;
                                                                     Node("returns",
                                                                        [TreeFromTyp t]);
                                                                     TreeFromStm stm])
                                | FunDec(None, s,
                                          decl, stm)-> Node(procdecFormat s,
                                                                    [TreeFromArgs decl;
                                                                     TreeFromStm stm])
        and TreeFromDecl decl = Node("Decl",List.map TreeFromDec decl)
        and TreeFromArgs decl = Node("Args", match decl with
                                                | [] -> [Node("none",[]) ]
                                                | _  -> List.map TreeFromDec decl
                                        )
        and TreeFromTyp = function
                                | ITyp              -> Node("ITyp",[])
                                | BTyp              -> Node("BTyp",[])
                                | ATyp(t,Some l)    -> Node("Array",
                                                         [Node("length",
                                                            [Node(l.ToString(),[])]);
                                                          Node("type",
                                                            [TreeFromTyp t])])
                                | ATyp(t,None)      -> Node("Array",[TreeFromTyp t])
                                | PTyp t            -> Node("Pointer",[TreeFromTyp t])// will probably not be used
                                | FTyp(ts, Some t)  -> Node("Function",// is never used
                                                        [Node("return type",
                                                            [TreeFromTyp t]);
                                                         Node("parameter types",
                                                            List.map TreeFromTyp ts)])
                                | FTyp(ts, None)    -> Node("Procedure",List.map TreeFromTyp ts)// is never used

        and TreeFromPro = function
                                | P ([], stml)      -> Node("Program",
                                                        List.map TreeFromStm stml)
                                | P (decl, stml)    -> Node("Program", 
                                                        [TreeFromDecl decl;
                                                         TreeFromStml stml])

