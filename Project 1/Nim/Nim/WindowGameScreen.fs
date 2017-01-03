module WindowGameScreen

let getHeapString heapArray = 
    (fst (Array.fold (fun (s,i) heap -> (s + i.ToString() + ": " + heap.ToString() + "\n",i+1)) ("",0) heapArray)).Trim();;


