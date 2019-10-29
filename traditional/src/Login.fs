module Login

open System
open Elmish
open Feliz

/// ============ Types =================

type SignedInUser =
    { Username : string
      AccessToken : string }

type LoginResult =
    | Success of user:SignedInUser
    | UsernameDoesNotExist
    | PasswordIncorrect
    | LoginError of errorMsg:string

type LoginInfo =
    { Username : string
      Password : string }

type Msg =
    | Login
    | ChangeUsername of string
    | ChangePassword of string
    | LoginSuccess of user:SignedInUser
    | LoginFailed of error:string
    | UpdateValidationErrors

type State = {
    LoggingIn: bool
    InputUsername: string
    UsernameValidationErrors: string list
    PasswordValidationErrors: string list
    InputPassword: string
    HasTriedToLogin: bool
    LoginError: string option
}

/// ============ Http(login) =================

let httpLogin (loginInfo: LoginInfo) =
    let request info = async {
        // simulate server worK
        do! Async.Sleep 1500
        return Success { Username = info.Username; AccessToken = "access-token" }
    }

    let successHandler = function
        | Success signedInUser -> LoginSuccess signedInUser
        | UsernameDoesNotExist -> LoginFailed "Username does not exist"
        | PasswordIncorrect -> LoginFailed "The password you entered is incorrect"
        | LoginError error -> LoginFailed error

    Cmd.OfAsync.either
        request loginInfo
        successHandler
        (fun ex -> LoginFailed "Unknown error occured while logging in")


/// ============ State(init, update) =================

let init() =
    { InputUsername = ""
      InputPassword = ""
      UsernameValidationErrors =  [ ]
      PasswordValidationErrors =  [ ]
      HasTriedToLogin = false
      LoginError = None
      LoggingIn = false }, Cmd.ofMsg UpdateValidationErrors

let validateInput (state: State) =
    let usernameRules =
      [ String.IsNullOrWhiteSpace(state.InputUsername), "Field 'Username' cannot be empty"
        state.InputUsername.Trim().Length < 5, "Field 'Username' must at least have 5 characters" ]
    let passwordRules =
      [ String.IsNullOrWhiteSpace(state.InputPassword), "Field 'Password' cannot be empty"
        state.InputPassword.Trim().Length < 5, "Field 'Password' must at least have 5 characters" ]
    let usernameValidationErrors =
        usernameRules
        |> List.filter fst
        |> List.map snd
    let passwordValidationErrors =
        passwordRules
        |> List.filter fst
        |> List.map snd

    usernameValidationErrors, passwordValidationErrors

let update msg (state: State) =
    match msg with
    | ChangeUsername name ->
        let nextState = { state with InputUsername = name }
        nextState, Cmd.ofMsg UpdateValidationErrors

    | ChangePassword pass ->
        let nextState = { state with InputPassword = pass }
        nextState, Cmd.ofMsg UpdateValidationErrors

    | UpdateValidationErrors ->
        let usernameErrors, passwordErrors = validateInput state
        let nextState =
            { state with UsernameValidationErrors = usernameErrors
                         PasswordValidationErrors = passwordErrors }
        nextState, Cmd.none

    | Login ->
        let state = { state with HasTriedToLogin = true }
        let usernameErrors, passwordErrors = validateInput state
        let canLogin = List.isEmpty usernameErrors && List.isEmpty passwordErrors
        if not canLogin then
            state, Cmd.none
        else
          let nextState = { state with LoggingIn = true }
          let credentials  =
            { Username = state.InputUsername
              Password = state.InputPassword  }
          nextState, httpLogin credentials

    | LoginSuccess token ->
        let nextState = { state with LoggingIn = false }
        nextState, Cmd.none

    | LoginFailed error ->
        let nextState =
            { state with LoginError = Some error
                         LoggingIn = false }

        nextState, Cmd.none


/// ============ View(render) =================

let formatErrors (errors: string list) = 
    match errors with 
    | [ ] -> 
        Html.none
    | _ -> 
        Html.ul [
            for error in errors -> 
            Html.li [
                prop.style [ style.color.crimson; style.fontSize 12 ]
                prop.text error
            ]
        ]

let applicationIcon = 
    Html.img [
        prop.src "/img/fable_logo.png"
        prop.style [ style.height 80; style.width 100 ]
    ]

let div (className: string) (children: ReactElement list) = 
    Html.div [ 
        prop.className className; 
        prop.children children 
    ]

let centered (children: ReactElement list) = 
    Html.div [
        prop.style [ style.textAlign.center ]
        prop.children children
    ]


let render (state: State) (dispatch: Msg -> unit) : ReactElement = 
    
    let canLogin = Seq.forall id [
        state.InputUsername.Trim().Length >= 5
        state.InputPassword.Trim().Length >= 5 
    ]

    let loginButtonClass = 
        if canLogin 
        then "btn btn-success btn-lg"
        else "btn btn-info btn-lg"

    let formContent = [
        centered [ applicationIcon ]
    
        Html.br [ ]
    
        Html.input [
            prop.style [ style.marginBottom 15 ]
            prop.className [ "form-control"; "form-control-lg" ]
            prop.valueOrDefault state.InputUsername
            prop.placeholder "Username"
            prop.onChange (ChangeUsername >> dispatch)
        ]
    
        if state.HasTriedToLogin then formatErrors state.UsernameValidationErrors
    
        Html.input [
            prop.style [ style.marginTop 15; style.marginBottom 15 ]
            prop.className [ "form-control"; "form-control-lg" ]
            prop.valueOrDefault state.InputPassword
            prop.withType.password
            prop.placeholder "Password"
            prop.onChange (ChangePassword >> dispatch)
        ]
    
        if state.HasTriedToLogin then formatErrors state.PasswordValidationErrors
    
        centered [
            Html.button [
                prop.className loginButtonClass
                prop.onClick (fun _ -> dispatch Login)
                prop.text (if state.LoggingIn then ". . ." else "Login")
            ]
        ]
    ]

    Html.div [
        prop.className "container"
        prop.style [ style.width 400; style.textAlign.center; style.marginTop 20 ]
        prop.children [
            div "card" [
                Html.div [
                    prop.className "card-block"
                    prop.style [ style.padding 30; style.textAlign.left; style.borderRadius 15 ]
                    prop.children formContent
                ]
            ]
        ]
    ]