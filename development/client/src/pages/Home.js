import React, { useState, useEffect, useContext } from "react";
import NavBar from "../components/NavBar";
import { getUsersApi } from "../api/user.api";
import Chatbox from "../components/Chatbox";

function Home({ cUser }) {
  const [Users, setUsers] = useState([]);
  const [recipient, setRecipient] = useState("");

  useEffect(() => {
    getUsersApi((err, result) => {
      if (err) {
        console.log(err);
      } else {
        console.log(result);
        setUsers(result.data);
      }
    });
  }, []);

  const returnUserCard = (u) => {
    let imgSrc = "./assets/blank_avatar.png";
    if (u.photos) {
      if (u.photos[u.photos.length - 1]) {
        imgSrc = u.photos[u.photos.length - 1].url;
      }
    }

    return (
      <div className="chat-container" onClick={() => setRecipient(u.userName)}>
        <img src={imgSrc} alt="" style={{ width: "80px", height: "80px" }} />
        <div>
          <h4 style={{ marginBottom: "0px", marginLeft: "5px" }}>
            {u.userName}
          </h4>
          <div className="chat-info">
            <p>Location: {u.city}</p>
            <p>Gender: {u.gender}</p>
            <p>Age: {u.age}</p>
          </div>
        </div>
      </div>
    );
  };

  return (
    <>
      <NavBar />
      {/* <div style={{ display: "flex", justifyContent: "space-around" }}> */}
      <div className="chat-user-container">
        {Users.length != 0 &&
          Users.filter((u) => u.userName != cUser.username).map((u) => {
            return returnUserCard(u);
          })}
      </div>
      <div className="chat-list-container">
        {recipient ? (
          <Chatbox recipient={recipient} cUser={cUser} />
        ) : (
          <h4>Select an user on the left to chat</h4>
        )}
      </div>
    </>
  );
}

export default Home;
