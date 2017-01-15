module GeneralTreeToPostScriptAlt
        let I = fun i -> i.ToString()
        open Trees
        ///PS instruction to move to position represented by two most recent elements on stack
        let psMOVETO = " moveto\n"
        ///PS instruction to move and draw a line to position represented by two most recent elements on stack
        let psLINETO =" lineto\n"
        ///PS instruction to "flush" lines drawn so far, or somesuch
        let psSTROKE =" stroke\n"
        ///Space, i.e. the string " " as a PS instruction
        let psSPACE = " "
        ///generate PS for creating the setting for a tree
        let psSetUp (left:int) (right:int) height lh nw = let l, r = (System.Math.Abs left)+nw, (System.Math.Abs right)+nw
                                                          let w = l+r 
                                                          let h = height+lh/2
                                                          ["<</PageSize[";I w;psSPACE; I h;"]/ImagingBBox null>> ";//make a bounding box for drawing the tree inside
                                                           "setpagedevice\n 1 1 scale\n ";//ensure scaling is same on both axis
                                                            I l;psSPACE;I (h-1);" translate\n newpath\n";//move coordinates so entire tree fits
                                                            "/Times-Roman findfont 10 scalefont setfont\n"]//some font for drawing labels
        ///ensure that the PS actually does something
        let psWrapUp = "showpage"
        ///Calculate absolute positioning of and PS to draw a node and its subtrees
        let rec makeTreePS h level shift =
                function Node ((label,f),st) ->
                           //start by calculating stuff to avoid repetition
                           ///absolute horizontal position from relative position
                           let shiftf = shift + f 
                           ///absolute vertical position (negated, because trees grow downwards)
                           let levelh = level*h//ToDo: make width a parameter?
                           let halfh, quarterh, eighth = h/2, h/4, h/8;
                           ///precalculated string of absolute position
                           let shiftfstring = I shiftf
                           ///PS for label of node
                           let labelPS =  [shiftfstring;psSPACE;I (-levelh); psMOVETO;//move to absolute position
                                           "("; label ;") dup stringwidth pop 2 div neg 0 rmoveto show\n"]//center node on absolute position
                           ///PS for line in, if appropiate
                           let lineInPS = if level < 1 then [] else
                                            [shiftfstring;psSPACE;I (quarterh-levelh) ;psMOVETO;
                                             shiftfstring;psSPACE;I (halfh-levelh);psLINETO]
                           ///PS for line out, including horizontal bar, if appropriate
                           let lineOutPS = if List.isEmpty st then 
                                               [] else 
                                               //find relative positions of the children that are farthest away
                                               //the list is assumed to be ordered so that this works
                                               let left, right = match st.Head, (List.last st) with | Node((_,left),_),Node((_,right),_) -> left,right
                                               let levelandhalfh = -halfh-levelh
                                               let levelandhalfhString =I levelandhalfh;
                                               [shiftfstring;psSPACE; I(quarterh+levelandhalfh) ;psMOVETO;//move below label of node
                                                shiftfstring;psSPACE; levelandhalfhString ;psLINETO;    //make line down to horizontal bar
                                                I (left+shiftf);psSPACE; levelandhalfhString ;psMOVETO; //move to above leftmost child
                                                I (right+shiftf);psSPACE; levelandhalfhString ;psLINETO]//draw line to above rightmost child
                           //Perform similarly on each subtree and collect information to be returned
                           st |> List.fold (fun (left, right, height, ps) n -> //fold list so we can collect information on absolute positions
                                                       let (left', right', height', ps') = makeTreePS h (level + 1) shiftf n // determine absolute extent and height of subtree
                                                       let rightMax  = if right' > right then right' else right            // if greater than what we have so far
                                                       let leftMax = if left' < left then left' else left                  // update maximum absolute extent
                                                       let heightMax = if height' > height then height' else height        // and height
                                                       (leftMax,rightMax,heightMax, ps @ ps'))
                                                       (shiftf, shiftf, levelh, [lineInPS; labelPS; lineOutPS; [psSTROKE]])// starting with absolute extent of this 
                           
        ///Helper function to flatten a list of lists into a list
        let rec flatten l = (List.collect (fun i -> i) l)

        ///Generate a list of postscript instructions for a string tree where
        ///each level is h apart vertically and each node is no closer than w to its siblings
        let createPostScriptAlt w h tree = let designtree = intDesign w (design tree)
                                           let leftMax, rightMax, heightMax, treePS = makeTreePS h 0 0 designtree
                                           String.concat "" ((psSetUp leftMax rightMax heightMax h w) @ flatten ( treePS ) @ [psWrapUp])


