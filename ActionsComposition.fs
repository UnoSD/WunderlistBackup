module ActionsComposition

open Actions
open CommandLineActions

let syncAction args =
    syncAction ParseArguments.parseCredentials CommandLineActions.asyncSaveToDatabase args

let mapActionTypeToAction args actionType = 
    let syncAction () = 
        syncAction args
    
    mapActionTypeToAction viewAction syncAction actionType

let queryUserForAction args = 
    mapActionTypeToAction args |>
    queryUserForAction commandLinePick