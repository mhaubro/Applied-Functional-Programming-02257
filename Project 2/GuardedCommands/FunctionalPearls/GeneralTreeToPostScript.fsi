module GeneralTreeToPostScript
    open Trees
    type psIns = ///PS instruction string
                     | S of string
                     ///PS instruction integer
                     | I of int

    val createPostScript : w:int -> h:int-> tree:string tree -> psIns list

