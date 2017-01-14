//namespace FunPearls
    module GeneralTreeToPostScript
        open Trees
        let psMOVETO ="moveto\n"
        let psLINETO ="lineto\n"
        let psSTROKE ="stroke\n"
        let psSetUp (left:int) (right:int) height = let l, r = (System.Math.Abs left)+25, (System.Math.Abs right)+25
                                                    //printfn "%A" (l, r)
                                                    let w = System.Math.Max((l+r), 1400)
                                                    let h = System.Math.Max(height,1000)
                                                    ["<</PageSize[";w.ToString(); h.ToString();"]/ImagingBBox null>> setpagedevice\n 1 1 scale\n ";(l).ToString();(h-1).ToString();" translate\n newpath\n/Times-Roman findfont 10 scalefont setfont\n"]
        let psWrapUp = "showpage"
        let leftMax = ref 0
        let rightMax = ref 0
        let heightMax = ref 0
        let updateMax r l h = if r > !rightMax then rightMax := r
                              if l < !leftMax then leftMax := l
                              if h > !heightMax then heightMax := h
        let resetMax = fun () -> leftMax := 0
                                 rightMax := 0
                                 heightMax := 0
        let rec makeTreePS level shift =
            function Node ((label,f),st) ->
                       let shiftf = shift + f
                       let leveldist = level*40
                       
                       let shiftfstring = shiftf.ToString()
                       let labelPS =  [shiftfstring; (-10-leveldist).ToString() ;psMOVETO;
                                       "("; label ;") dup stringwidth pop 2 div neg 0 rmoveto show\n"]
                       let lineInPS = if level < 1 then [] else
                                        [shiftfstring; (5-leveldist).ToString() ;psMOVETO;
                                         shiftfstring; (20-leveldist).ToString() ;psLINETO]
                   
                       let lineOutPS = if List.isEmpty st then 
                                           updateMax shiftf shiftf (leveldist+10)
                                           [psSTROKE] else 
                                           let left, right = match st.Head, (List.last st) with
                                                                |Node((_,left),_),Node((_,right),_) -> left,right

                                           updateMax (shiftf+right) (shiftf+left) (leveldist+10)

                                           [shiftfstring; (-15-leveldist).ToString() ;psMOVETO;
                                            shiftfstring; (-20-leveldist).ToString() ;psLINETO;
                                            (left+shiftf).ToString(); (-20-leveldist).ToString() ;psMOVETO;
                                            (right+shiftf).ToString(); (-20-leveldist).ToString() ;psLINETO;psSTROKE]
                       [labelPS;lineInPS;lineOutPS] @ (List.collect (makeTreePS (level + 1) shiftf) st)

        let rec flatten l= (List.collect (fun i -> i) l)

        let createPostScript w tree = //resetMax()
                                      let designtree = intDesign w (design tree)
                                      String.concat " " ((psSetUp !leftMax !rightMax !heightMax) @ flatten ( makeTreePS 0 0 designtree ) @ [psWrapUp])


