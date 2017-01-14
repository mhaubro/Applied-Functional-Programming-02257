//namespace FunPearls
    module ASTToGeneralTreeConverter
        open GuardedCommands.Frontend.AST
        open Trees

        let rec TreeFromExp = function
                                | Access acc        -> Node("Access", [TreeFromAcc acc])
                                | Addr acc          -> Node("Address", [TreeFromAcc acc])
                                | Apply(f,es) when List.exists (fun x ->  x=f) ["!";"-";"+";"*"; "="; "&&";"||";"<";">";"<>";"<="]
                                                    -> Node("( "+f+" )", List.map TreeFromExp es)
                                | Apply(f,es)       -> Node("[ "+f+" ]", List.map TreeFromExp es)
                                | N i               -> Node("( "+i.ToString()+" )",[])
                                | B b               -> Node("( "+b.ToString()+" )",[])

        and TreeFromAcc = function
                                | AIndex(acc,e)     -> Node("AIndex",
                                                           [TreeFromAcc acc;
                                                            TreeFromExp e ])
                                | AVar s            -> Node ("[ "+s+" ]",[])
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
                                | Block([],stml)    -> Node("Block",List.map TreeFromStm stml)
                                | Block(decl,stml)  -> Node("Block",
                                                           [Node("Declarations",List.map TreeFromDec decl);
                                                            Node("Body",List.map TreeFromStm stml)])
                                | Call(f,es)        -> Node("[ "+f+" ]", List.map TreeFromExp es)
        and TreeFromGC = function
                                | e,stml            -> Node("GC",
                                                           [Node("guard", [TreeFromExp e]);
                                                            Node("body", List.map TreeFromStm stml)])
    
        and TreeFromDec = function
                                | VarDec(t,x)       -> Node("Var: "+ x,[TreeFromTyp t])
                                | FunDec(Some t, f,
                                          decl, stm)-> Node("Fun: "+ f,
                                                                    [Node("args",
                                                                        List.map TreeFromDec decl);
                                                                     Node("returns",
                                                                        [TreeFromTyp t]);
                                                                     TreeFromStm stm])
                                | FunDec(None, f,
                                          decl, stm)-> Node("Proc: "+f,
                                                                    [Node("args",
                                                                        List.map TreeFromDec decl);
                                                                     TreeFromStm stm])
    
        and TreeFromTyp = function
                                | ITyp              -> Node("ITyp",[])
                                | BTyp              -> Node("BTyp",[])
                                | ATyp(t,Some l)    -> Node("Array",
                                                         [Node("length",
                                                            [Node(l.ToString(),[])]);
                                                          Node("type",
                                                            [TreeFromTyp t])])
                                | ATyp(t,None)      -> Node("Array",[TreeFromTyp t])
                                | PTyp t            -> Node("Pointer",[TreeFromTyp t])
                                | FTyp(ts, Some t)  -> Node("Function",
                                                        [Node("return type",
                                                            [TreeFromTyp t]);
                                                         Node("parameter types",
                                                            List.map TreeFromTyp ts)])
                                | FTyp(ts, None)    -> Node("Procedure",List.map TreeFromTyp ts)

        and TreeFromPro = function
                                | P ([], stml)      -> Node("Program",
                                                        List.map TreeFromStm stml)
                                | P (decl, stml)    -> Node("Program", 
                                                        [Node("Declarations",
                                                            List.map TreeFromDec decl);
                                                         Node("Body",
                                                            List.map TreeFromStm stml)])

