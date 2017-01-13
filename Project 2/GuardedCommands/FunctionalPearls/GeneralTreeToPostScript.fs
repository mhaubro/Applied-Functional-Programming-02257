module GeneralTreeToPostScript
    open Trees
    let psMOVETO ="moveto\n"
    let psLINETO ="lineto\n"
    let psSTROKE ="stroke\n"
    let psSetUp (left:int) (right:int) height = let l, r = (System.Math.Abs left), (System.Math.Abs right)
                                                printfn "%A" (l, r)
                                                let w = System.Math.Max((l+r), 1400)
                                                let h = System.Math.Max(height,1000)
                                                ["<</PageSize[";w.ToString(); h.ToString();"]/ImagingBBox null>> setpagedevice\n 1 1 scale\n ";(w/2).ToString();(h-1).ToString();" translate\n newpath\n/Times-Roman findfont 10 scalefont setfont\n"]
    let psWrapUp = "showpage"
    let leftMax = ref 0
    let rightMax = ref 0
    let heightMax = ref 0
    let rec makeTreePS level shift =
        function Node ((label,f),st) ->
                   let shiftf = shift + f
                   let leveldist = level*40
                   if leveldist + 10 > !heightMax then heightMax := leveldist+10

                   let shiftfstring = shiftf.ToString()
                   let labelPS =  [shiftfstring; (-10-leveldist).ToString() ;psMOVETO;
                                   "("; label ;") dup stringwidth pop 2 div neg 0 rmoveto show\n"]
                   let lineInPS = if level < 1 then [] else
                                    [shiftfstring; (5-leveldist).ToString() ;psMOVETO;
                                     shiftfstring; (20-leveldist).ToString() ;psLINETO]
                   
                   let lineOutPS = if List.isEmpty st then 
                                       if shiftf > !rightMax then rightMax := shiftf
                                       if shiftf < !leftMax then leftMax := shiftf
                                       [psSTROKE] else 
                                       let left, right = match st.Head, (List.last st) with
                                                            |Node((_,left),_),Node((_,right),_) -> left,right
                                       
                                       if right+shiftf > !rightMax then rightMax := right+shiftf
                                       if left+shiftf < !leftMax then leftMax := left+shiftf

                                       [shiftfstring; (-15-leveldist).ToString() ;psMOVETO;
                                        shiftfstring; (-20-leveldist).ToString() ;psLINETO;
                                        (left+shiftf).ToString(); (-20-leveldist).ToString() ;psMOVETO;
                                        (right+shiftf).ToString(); (-20-leveldist).ToString() ;psLINETO;psSTROKE]
                   [labelPS;lineInPS;lineOutPS] @ (List.collect (makeTreePS (level + 1) shiftf) st)

    let rec flatten l= (List.collect (fun i -> i) l)

    let createPostScript w tree = let designtree = intDesign w (design tree)
                                  String.concat " " ((psSetUp !leftMax !rightMax !heightMax) @ flatten ( makeTreePS 0 0 designtree ) @ [psWrapUp])


