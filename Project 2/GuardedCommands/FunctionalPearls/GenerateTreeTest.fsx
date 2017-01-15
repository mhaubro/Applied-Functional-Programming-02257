
#load "Trees.fs"
#load "TreeGenerator.fs"

open Trees
open TreeGenerator

#time "on"
generatePlainTree 10 5 |> ignore;;
generateRandomTree 10 2 5 |> ignore;;

