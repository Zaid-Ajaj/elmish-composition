module App

open Elmish
open SharedTypes

type Page =
    | Login
    | Home of SignedInUser

type State = { CurrentPage : Page }

type Msg =
    | SignedIn of SignedInUser
    | Logout

let init() =
    let initialState = { CurrentPage = Login }
    initialState, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | SignedIn user -> { state with CurrentPage = Home user  }, Cmd.none
    | Logout -> init()

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentPage with
    | Login -> Login.login { signedIn = SignedIn >> dispatch }
    | Home user -> Home.home { user = user; logout = fun _ -> dispatch Logout  }