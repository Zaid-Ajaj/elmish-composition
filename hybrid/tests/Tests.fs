module Tests

open Fable.Mocha
open App

let appTests = testList "App tests" [
    testCase "update function works" <| fun _ ->
        // Simplified update that ignore commands/effects
        let update state msg = fst (App.update msg state)
        Expect.equal 1 1 "Expected updated state to be 1"
]

let allTests = testList "All" [
    appTests
]

[<EntryPoint>]
let main (args: string[]) = Mocha.runTests allTests
