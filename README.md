# Compare Elmish Components

This repository includes two projects: `traditional` and `hybrid` to compare the composition technique of an Elmish application.

The `traditional` directory contains an application that follows Elm-style technique for composition.

The `hybrid` directory contains an application that is a hybrid of Elm with React to compose the application with help of [Feliz.ElmishComponents](https://zaid-ajaj.github.io/Feliz/#/Feliz/ElmishComponents).

The application is a simple login flow with basic parent-child communication in the following manner

```
     App
      |
      |
  |-------|
 Login   Home
```

The implementation of `Login` and `Home` is more or less the same. However, the `App` component which contains the glue code becomes greatly simplified using [Feliz.ElmishComponents](https://zaid-ajaj.github.io/Feliz/#/Feliz/ElmishComponents) because it doesn't have to manage the state of its child components


Each of these directories is a standalone frontend project so to run one of them you have to:
```bash
cd traditional # or cd hybrid
npm intall
npm start
```