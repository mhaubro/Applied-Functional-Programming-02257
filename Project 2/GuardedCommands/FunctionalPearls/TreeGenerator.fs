module TreeGenerator

    open Trees
    open System
    open System.Threading

    /// Creates a tree with a fixed number of children.
    let rec generatePlainTree depth numChilds = if (depth < 0) then failwith "Wrong input: Can't make tree with negative depth."
                                                if (numChilds < 0) then failwith "Wrong input: Can't make tree with negative number of children."
                                                match (depth, numChilds) with
                                                | (n,m) when n > 0 -> Node("node", (List.init m (fun _ -> generatePlainTree (n-1) m)))
                                                | (n,m) when n = 0 -> Node("leaf", [])
                                                | _ -> failwith "Unexpected error."


    /// Hidden function that takes a System.Random object, to avoid creating a new one each time.
    let rec private generateRandomTree' depth minC maxC (random:System.Random) = if (depth < 0) then failwith "Wrong input: Can't make tree with negative depth."
                                                                                 if (maxC < 0) then failwith "Wrong input: max is negative."
                                                                                 if (minC < 0) then failwith "Wrong input: min is negative."
                                                                                 match (depth, maxC, minC) with
                                                                                 | (n,max,min) when n > 0 -> Node("node", (List.init (random.Next(min,max)) (fun _ -> generateRandomTree' (n-1) min max random)))
                                                                                 | (n,max,min) when n = 0 -> Node("leaf", [])
                                                                                 | _ -> failwith "Unexpected error."

    let rnd  = new Random()
    /// Creates a tree with each node having between min and max number of children.
    let generateRandomTree depth minC maxC = generateRandomTree' depth minC maxC rnd

    let rec generateSimpleTree depth = match depth with
                                        | n when n=0 -> Node("leaf",[])
                                        | n when n>0 -> Node("node",[generateSimpleTree (n-1)])
                                        | _ -> failwith "Unexpected error."
                                        
    let rec insertNodeRandomly m (rnd:System.Random) = function| Node(l,[]) -> Node("node",[Node("leaf",[])])
                                                               | Node(l, cl) when List.length cl > rnd.Next(m) || rnd.Next()%2=0 
                                                                            -> let f = insertNodeRandomly m rnd 
                                                                               match List.splitAt (rnd.Next(0,cl.Length)) cl with
                                                                                    | [],[]  -> failwith"WHAT?"
                                                                                    | b, a::cl' -> Node(l,b @ f(a)::cl')
                                                                                    | b::cl',[] -> Node(l,f(b)::cl')
                                                               | Node(l,cl) -> Node(l,Node("leaf",[])::cl)

    let insertNodesRandomly n m tree = List.init n ( fun _ -> insertNodeRandomly m rnd)
                                                                        |> List.fold (fun t' insert -> insert t') tree                                             
    
    let rec doToSomeLeaf f tree = match tree with 
                                    | Node(l, []) as n -> f(n)
                                    | Node(l, cl) -> match List.splitAt (rnd.Next(0,cl.Length)) cl with
                                                        | [],[]  -> failwith"WHAT?"
                                                        | b, a::cl' -> Node(l,b @ f(a)::cl')
                                                        | b::cl',[] -> Node(l,f(b)::cl')

    let replaceSomeLeaf subtree = doToSomeLeaf (fun _ -> subtree)
    
    let randomIntsFromSum n max = List.unfold (fun v ->
                                                    if v=0 then None else
                                                        if v>=max then 
                                                            let d = rnd.Next(1,max+1) in Some(d, (v-d))
                                                            else Some(v,0)) n;;


    let randomizedTree n i = 
                let root = Node("root",[Node("leaf",[])])
                //insertNodesRandomly (n-2) i root
                let qq = Math.Max(3,i+1)
                randomIntsFromSum (n-2) i
                                    |> List.map (fun j -> insertNodesRandomly (j) (rnd.Next(2,qq)))
                                    |> List.map doToSomeLeaf
                                    |> List.fold (fun t' insert -> insert t') root

    // Tried to make the functions asyncronous, but the overhead of the thread handling was to big for it to be efficient.