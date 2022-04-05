import React, { useState, useContext, useEffect } from "react";
import { registerApi, loginApi, checkLocationApi } from "../api/user.api";
import { globalContext } from "../store/globalContext";
import heart from "../heart.gif";

function SignIn() {
  const { currentUser, setCurrentUser } = useContext(globalContext);

  const [toggle, setToggle] = useState(false);
  const [registrant, setRegistrant] = useState({
    username: "",
    password: "",
    age: 0,
    city: "",
    gender: "",
    // img: null,
  });

  const [credentials, setCredentials] = useState({
    username: "",
    password: "",
  });

  useEffect(() => {
    checkLocationApi((err, result) => {
      if (err) {
        console.log(err);
      } else {
        if (result.data) {
          setCurrentUser({ ...currentUser, withinRange: true });
        }
      }
    });
  }, []);

  const onRegisterChange = (e) => {
    const name = e.target.name;
    const value = e.target.value;
    setRegistrant({ ...registrant, [name]: value });
  };

  const onLoginChange = (e) => {
    const name = e.target.name;
    const value = e.target.value;
    setCredentials({ ...credentials, [name]: value });
  };

  const onLoginSubmit = (e) => {
    e.preventDefault();

    let user_obj = {
      password: credentials.password,
      username: credentials.username,
    };
    console.log(user_obj);

    loginApi(user_obj, (err, result) => {
      if (err) {
        window.alert("wrong username/password combination");
        console.log(err);
      } else {
        console.log("login successful");
        console.log(result.data);
        user_obj.jwt = result.data.jwt;
        setCurrentUser(result.data);
      }
    });
  };

  const onRegisterSubmit = (e) => {
    e.preventDefault();
    registerApi(registrant, (err, result) => {
      if (err) {
        console.log(err);
      } else {
        window.alert(
          `Registration successful, please login ${registrant.username}`
        );
      }
    });
  };

  const toggleButton = () => {
    if (toggle) {
      setToggle(false);
    } else {
      setToggle(true);
    }
  };

  return (
    <>
      {currentUser.withinRange ? (
        <div className="main_home">
          <div className="main_title_right">
            <p>
              <span className="black">V</span>an<span className="black">A</span>
              pp
            </p>
          </div>

          <div className="login_wrapper">
            {toggle ? (
              <div className="login_card">
                <p>
                  Not our family yet?
                  <button className="toggle_button" onClick={toggleButton}>
                    Register
                  </button>
                </p>
                <h1>Login</h1>

                <div
                  style={{
                    display: "flex",
                    margin: "10px",
                  }}
                >
                  <p
                    style={{
                      margin: "auto auto auto 10px",
                    }}
                  >
                    Start your day with VanApp
                  </p>
                  <img
                    src={heart}
                    alt="loading..."
                    style={{
                      width: "50px",
                      height: "50px",
                      justifyContent: "center",
                      alignItems: "center",
                    }}
                  />
                </div>

                <form>
                  <div className="home_input">
                    <input
                      type="text"
                      placeholder="Enter username"
                      name="username"
                      required
                      onChange={onLoginChange}
                    />
                  </div>
                  <div className="home_input">
                    <input
                      type="password"
                      placeholder="Enter password"
                      name="password"
                      required
                      onChange={onLoginChange}
                    />
                  </div>
                  <button
                    className="main_submit"
                    type="submit"
                    onClick={(e) => onLoginSubmit(e)}
                  >
                    Login
                  </button>
                </form>
              </div>
            ) : (
              <div className="login_card">
                <p>
                  {" "}
                  Already have an account?
                  <button className="toggle_button" onClick={toggleButton}>
                    Login
                  </button>
                </p>
                <h1>Register</h1>
                <div
                  style={{
                    display: "flex",
                    margin: "10px",
                  }}
                >
                  <p
                    style={{
                      margin: "auto auto auto 10px",
                    }}
                  >
                    Start your day with VanApp
                  </p>
                  <img
                    src={heart}
                    alt="loading..."
                    style={{
                      width: "50px",
                      height: "50px",
                      justifyContent: "center",
                      alignItems: "center",
                    }}
                  />
                </div>
                <form>
                  <div className="home_input">
                    <input
                      type="text"
                      placeholder="Enter username"
                      name="username"
                      required
                      onChange={onRegisterChange}
                    />
                  </div>
                  <div className="home_input">
                    <input
                      type="password"
                      placeholder="Enter password"
                      name="password"
                      required
                      onChange={onRegisterChange}
                    />
                  </div>
                  <div className="home_input">
                    <input
                      type="text"
                      placeholder="Enter age"
                      name="age"
                      required
                      onChange={onRegisterChange}
                    />
                  </div>
                  <div className="home_input">
                    <input
                      type="text"
                      placeholder="Enter city"
                      name="city"
                      required
                      onChange={onRegisterChange}
                    />
                  </div>
                  <div className="home_input">
                    <input
                      type="text"
                      placeholder="Enter gender"
                      name="gender"
                      required
                      onChange={onRegisterChange}
                    />
                  </div>
                  <button
                    className="main_submit"
                    type="submit"
                    onClick={(e) => onRegisterSubmit(e)}
                  >
                    Register
                  </button>
                </form>
              </div>
            )}
          </div>
        </div>
      ) : (
        <div>
          <h2>
            Sorry, you are not in the greater Vancouver area, so you cannot
            access this site
          </h2>
        </div>
      )}
    </>
  );
}

export default SignIn;
