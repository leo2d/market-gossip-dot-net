import React, { useState, useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { Container } from "react-bootstrap";

import { ChatContainer, ChatSquare } from "./styles";
import Message from "../../components/Message";
import MessageInput from "../../components/MessageInput";
import Auth from "../../utils/auth";
import serverURL from "../../serverUrl";
import { isCommand } from "../../utils/commandUtils";
import { formatDateToLT } from "../../utils/dateUtils";
import usePageTitle from "../../hooks/usePageTitle";

const Chat = () => {

  usePageTitle('Chat - Market Gossip');

  const [user] = useState(Auth.getUserData().user);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState([]);

  const [hubConnection, setHubConnection] = useState(undefined);

  const MESSAGES_LIMIT = 50;
  const MESSAGE_EVENT = "SendMessage";
  const COMMAND_EVENT = "SendCommand";
  const JOINED_EVENT = "SendWelcomeGreetings";

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${serverURL}/hubs/chat`)
      .withAutomaticReconnect()
      .build();

    setHubConnection(newConnection);
  }, []);

  useEffect(() => {
    if (hubConnection) {
      hubConnection
        .start()
        .then((result) => {
          console.log("Connected!");

          hubConnection.on("ReceiveMessage", (message) => {
            setMessages((messages) => manageMessages(messages, message));
          });

          hubConnection.send(JOINED_EVENT, {
            id: `${Math.random()}`,
            author: user.username,
            text: '',
            sentAt: new Date(),
          })
            .catch((er) => console.log("Joined event failed: ", er));
        })
        .catch((e) => console.log("Connection failed: ", e));
    }
  }, [hubConnection, user]);

  const manageMessages = (currentMessages, newMesage) => {
    const message = { ...newMesage, sentAt: formatDateToLT(newMesage.sentAt) };

    if (currentMessages.length === MESSAGES_LIMIT) {
      let showedMsgs = currentMessages;
      showedMsgs.splice(0, 1);

      return [...showedMsgs, message];
    }
    return [...currentMessages, message];
  };

  const sendMessage = async (event) => {
    event.preventDefault();

    if (message) {
      const newMessage = {
        id: `${Math.random()}`,
        author: user.username,
        text: message,
        sentAt: new Date(),
        authorColor: user.userColor
      };

      if (hubConnection.state === "Connected") {
        try {
          const hubEntry = isCommand(message) ? COMMAND_EVENT : MESSAGE_EVENT;
          await hubConnection.send(hubEntry, newMessage);
        } catch (e) {
          console.log(e);
        } finally {
          setMessage("");
          return;
        }
      }

      alert("No connection to server yet.");
    }
  };

  return (
    <Container>
      <ChatContainer>
        <ChatSquare>
          {messages.map((m) => {
            return <Message key={m.id} message={m} />;
          })}
        </ChatSquare>
        <MessageInput
          message={message}
          setMessage={setMessage}
          sendMessage={sendMessage}
        />
      </ChatContainer>
    </Container>
  );
};
export default Chat;
