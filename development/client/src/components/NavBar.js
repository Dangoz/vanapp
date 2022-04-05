import React, { useContext, useState } from "react";
import { globalContext } from "../store/globalContext";
import { FileUpload } from "./FileUpload";
import Logout from "../logout_btn.png";
import Main from "../main_btn.png";
import Alarm from "../alarm_btn.png";
import Setting from "../setting_btn.png";

function NavBar() {
  const { currentUser, setCurrentUser } = useContext(globalContext);
  const [toggle, setToggle] = useState(false);

  const toggleButton = () => {
    if (toggle) {
      setToggle(false);
    } else {
      setToggle(true);
    }
  };

  return (
    <div>
      <div className="nav-container">
        <img
          className="profile-img"
          src={
            currentUser.imgUrl
              ? currentUser.imgUrl
              : "./assets/blank_avatar.png"
          }
          alt=""
        />
        <h2>{currentUser.username}</h2>
        <div>
          <img src={Main} />
        </div>

        <div>
          <img src={Alarm} />
        </div>

        <div>
          <img src={Setting} onClick={toggleButton} />
        </div>

        {toggle && <FileUpload cuser={currentUser} />}

        <div>
          <button
            onClick={() => {
              let removedUser = {
                ...currentUser,
                jwt: "",
              };
              setCurrentUser(removedUser);
            }}
          >
            <img src={Logout} />
          </button>
        </div>
      </div>
    </div>
  );
}

export default NavBar;
