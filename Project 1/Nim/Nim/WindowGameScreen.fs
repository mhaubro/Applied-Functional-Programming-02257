module WindowGameScreen

let getHeapString game = 
    (fst (List.fold (fun (s,i) heap -> (s + i.ToString() + ": " + heap.ToString() + "\n",i+1)) ("",0) game)).Trim();;
