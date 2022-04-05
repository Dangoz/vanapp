import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";

import GlobalContext from "./store/globalContext";

ReactDOM.render(
  <GlobalContext>
    <App />
  </GlobalContext>,
  document.getElementById("root")
);
