import React from 'react';

import {
  Container,
  AuthorContainer,
  Author,
  MessageContainer,
  MessageText,
  ItemContainer,
  MessageDate,
} from './styles';
import Auth from '../../utils/auth';

const Message = ({ message }) => {

  const { user } = Auth.getUserData();
  const isFromCurrentUser = message.author === user.username;
  const isFromBOT = message.author === 'Mkt Gossip BOT';
  const isFromSystem = message.author === 'System';

  const authorLabel = isFromCurrentUser ? ` ~ ${message.author} (me)` : message.author;

  const authorColor = isFromBOT ? '#FFF' : isFromSystem ? '#23272A' : message.authorColor;

  const sucessBotMessage = '#1a7b56';
  const errorBotMessage = '#E36387';
  const regularBotMessage = '#575fcf';
  const warningBotMessage = '#aa9f31';

  const botMessageTypes = {
    'regular': regularBotMessage,
    'error': errorBotMessage,
    'success': sucessBotMessage,
    'warning': warningBotMessage
  };

  const containerColor = isFromBOT ? botMessageTypes[message.type] ?? regularBotMessage : '#23272A'

  return (
    <ItemContainer>
      <Container customColor={containerColor} >
        <AuthorContainer>
          {isFromSystem ? (<MessageText systemMessage={isFromSystem}>{message.text}</MessageText>
          ) :
            (
              <Author customColor={authorColor}>
                {isFromBOT ? (<>&#129302; &nbsp;</>) : <></>}
                {authorLabel}
              </Author>
            )
          }
          <MessageDate>{`${message.sentAt}`}</MessageDate>
        </AuthorContainer>
        {isFromSystem ? (<></>) :
          (
            <MessageContainer>
              <MessageText systemMessage={isFromSystem}>{message.text}</MessageText>
            </MessageContainer>
          )}
      </Container>
    </ItemContainer>
  );
};

export default Message;
