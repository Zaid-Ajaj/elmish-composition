module Home

open Elmish
open Feliz

type State = {
    Username: string
    AccessToken: string
}

type Msg =
    | Logout
    | DoNothing

let init (username: string, accessToken: string) =
    { Username = username; AccessToken = accessToken }, Cmd.none

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
            Html.h1 (sprintf "Hello %s" state.Username)
            Html.button [
                prop.className "btn btn-info btn-lg"
                prop.onClick (fun _ -> dispatch Logout)
                prop.text "Logout"
            ]
        ]
    ]

open Feliz.ElmishComponents

let home (props: {| user: (string * string); logout : unit -> unit |}) =
    React.elmishComponent("Home",init props.user, update props.logout, render)