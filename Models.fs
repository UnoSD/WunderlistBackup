namespace WunderlistBackup

open FSharp.Data

type FoldersProvider  = JsonProvider< "FoldersJsonSample.json"  , RootName = "Folder"  >
type ListsProvider    = JsonProvider< "ListsJsonSample.json"    , RootName = "List"    >
type TasksProvider    = JsonProvider< "TasksJsonSample.json"    , RootName = "Task"    >
type SubtasksProvider = JsonProvider< "SubtasksJsonSample.json" , RootName = "Subtask" >
type NotesProvider    = JsonProvider< "NotesJsonSample.json"    , RootName = "Notes"   >
type FilesProvider    = JsonProvider< "FilesJsonSample.json"    , RootName = "Files"   >

type Folder  = FoldersProvider.Folder
type WList   = ListsProvider.List
type Task    = TasksProvider.Task
type Subtask = SubtasksProvider.Subtask
type Note    = NotesProvider.Note
type File    = FilesProvider.File