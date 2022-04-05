import React, { useContext } from "react";
import logo from "./logo.svg";
import "./App.css";
import { globalContext } from "./store/globalContext";
import Home from "./pages/Home";
import SignIn from "./pages/SignIn";

function App() {
  const { currentUser } = useContext(globalContext);

  return (
    <>
      {currentUser.jwt ? (
        <div className="home-grid">
          <Home cUser={currentUser} />
        </div>
      ) : (
        <SignIn />
      )}
    </>
  );
}

export default App;
