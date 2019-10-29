module App

open Feliz
open Elmish

type User = 
    { Username: string 
      AccessToken : string }

[<RequireQualifiedAccess>]
type Page = 
    | Login of Login.State
    | Home of Home.State

type State = 
    { CurrentUser: User option
      CurrentPage: Page }

[<RequireQualifiedAccess>]
type Msg =
    | LoginMsg of Login.Msg
    | HomeMsg of Home.Msg

/// The init function initializes Login to be the first page
let init() = 
    let loginState, loginCmd = Login.init()

    let state = { 
        CurrentUser = None
        CurrentPage = Page.Login loginState 
    }

    let cmd = Cmd.map Msg.LoginMsg loginCmd

    state, cmd
   
let update (msg: Msg) (state: State) =
    match msg, state.CurrentPage with
    // intercept LoginSuccess message from Login and navigate to Home
    | Msg.LoginMsg (Login.Msg.LoginSuccess signedInUser), Page.Login _ -> 
        // user has logged in -> go to home
        let homeState, homeCmd = Home.init signedInUser.Username signedInUser.AccessToken
        let nextState = {
            CurrentUser = Some { Username = signedInUser.Username; AccessToken = signedInUser.AccessToken }
            CurrentPage = Page.Home homeState
        }

        nextState, Cmd.map Msg.HomeMsg homeCmd

    | Msg.LoginMsg loginMsg, Page.Login loginState  -> 
        let nextLoginState, nextLoginCmd = Login.update loginMsg loginState 
        let nextState = { state with CurrentPage = Page.Login nextLoginState }
        let nextCmd = Cmd.map Msg.LoginMsg nextLoginCmd
        nextState, nextCmd

    // Intercept Logout from Home and come back to Login
    | Msg.HomeMsg (Home.Msg.Logout), Page.Home _ -> 
        init()
        
    | Msg.HomeMsg homeMsg, Page.Home homeState -> 
        let nextHomeState, nextHomeCmd = Home.update homeMsg homeState
        let nextState = { state  with CurrentPage = Page.Home nextHomeState }
        let nextCmd = Cmd.map Msg.HomeMsg nextHomeCmd
        nextState, nextCmd

    | _ ->
        state, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentPage with 
    | Page.Login loginState -> Login.render loginState (Msg.LoginMsg >> dispatch)
    | Page.Home homeState -> Home.render homeState (Msg.HomeMsg >> dispatch)