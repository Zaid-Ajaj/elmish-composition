module App

open Feliz
open Elmish
open Fable.Core

type User =
    { Username: string
      AccessToken : string }

[<StringEnum>]
type Page =
    | Login
    | Home

type State = {
    CurrentUser: User option
    CurrentPage : Page
}

type Msg =
    | SignedIn of User
    | Logout

let init() =
    let initialState = {
        CurrentPage = Login
        CurrentUser = None
    }

    initialState, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | SignedIn user -> { state with CurrentUser = Some user; CurrentPage = Home  }, Cmd.none
    | Logout -> init()

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentPage with
    | Login ->
        let signedIn (username, token) =
            let user = { Username = username; AccessToken = token }
            dispatch (SignedIn user)

        Login.login {| signedIn = signedIn |}

    | Home ->
        match state.CurrentUser with
        | Some user ->
            Home.home
                {| user = user.Username, user.AccessToken;
                   logout = fun _ -> dispatch Logout |}

        | None ->
            // illegal state
            Html.none