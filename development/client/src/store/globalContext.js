import { createContext, useState } from "react";

export const globalContext = createContext({});

export default function GlobalContext(props) {
  const [currentUser, setCurrentUser] = useState({
    username: "",
    imgUrl: "",
    imgPublicId: "",
    age: 0,
    gender: "",
    city: "",
    jwt: "",
    withinRange: false,
  });

  return (
    <globalContext.Provider
      value={{
        currentUser,
        setCurrentUser,
      }}
    >
      {props.children}
    </globalContext.Provider>
  );
}
