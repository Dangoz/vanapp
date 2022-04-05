import React, { useState, useEffect, useContext } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { HUB_URL } from "../config/constants";
import { globalContext } from "../store/globalContext";

function Chatbox({ recipient, cUser }) {
  const { currentUser } = useContext(globalContext);
  const [message, setMessage] = useState("");
  const [messageThread, setMessageThread] = useState([]);
  const [hubConnection, sethubConnection] = useState(null);

  // let sendMessage;

  useEffect(() => {
    sethubConnection(
      new HubConnectionBuilder()
        .withUrl(HUB_URL + "/message?user=" + recipient, {
          accessTokenFactory: () => cUser.jwt,
        })
        .withAutomaticReconnect()
        .build()
    );
  }, [recipient]);

  useEffect(() => {
    if (hubConnection) {
      hubConnection.start().catch((error) => console.log(error));
      hubConnection.on("ReceiveMessageThread", (messages) => {
        console.log("on ReceiveMessageThread");
        console.log(messages);
        setMessageThread(messages);
      });
      hubConnection.on("NewMessage", (message) => {
        console.log("on NewMessage");
        console.log(message);
        setMessageThread((messageThread) => [...messageThread, message]);
      });
    }
    // return () => {
    //   hubConnection.stop();
    // };
  }, [hubConnection]);

  let sendMessage = async () => {
    setMessage("");
    return hubConnection
      .invoke("SendMessage", {
        RecipientUsername: recipient,
        Message: message,
      })
      .catch((error) => console.log(error));
  };

  const onMessageInput = (e) => {
    setMessage(e.target.value);
  };

  const displayMessageThread = () => {
    return messageThread.map((element) => {
      if (element.senderUsername === currentUser.username) {
        return (
          <div className="message-sent">
            <span>{element.messageSent}</span>
            <p>{element.messageContent}</p>
          </div>
        );
      } else {
        return (
          <div className="message-receive">
            <span>{element.messageSent}</span>
            <p>{element.messageContent}</p>
          </div>
        );
      }
    });
  };

  return (
    <div>
      <h3>Chatting with {recipient}</h3>

      <div>
        <div className="msg-thread">{displayMessageThread()}</div>
        <div className="chat-send-container">
          <input
            type="text"
            placeholder="Enter Message"
            name="username"
            required
            value={message}
            onChange={onMessageInput}
          />
          <button onClick={() => sendMessage()}>Send Message</button>
        </div>
      </div>
    </div>
  );
}

export default Chatbox;
