module Home

open Elmish
open Feliz
open SharedTypes 

type State = {
    CurrentUser: SignedInUser
}

type Msg =
    | Logout
    | DoNothing

let init (user: SignedInUser) : State * Cmd<Msg> =
    { CurrentUser = user }, Cmd.none

let update logout msg state =
    match msg with
    | Logout -> state, Cmd.ofSub (fun _ -> logout())
    | _ -> state, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        prop.style [
            style.padding 20
            style.textAlign.center
        ]

        prop.children [
            Html.h1 (sprintf "Hello %s" state.CurrentUser.Username)
            Html.button [
                prop.className "btn btn-info btn-lg"
                prop.onClick (fun _ -> dispatch Logout)
                prop.text "Logout"
            ]
        ]
    ]

open Feliz.ElmishComponents
open SharedTypes

type HomeProps = {
    user: SignedInUser
    logout : unit -> unit
}

let home (props: HomeProps) =
    React.elmishComponent("Home", init props.user, update props.logout, render)