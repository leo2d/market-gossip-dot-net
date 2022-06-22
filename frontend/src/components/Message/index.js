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
  const isCurrentUserOwner = message.author === user.username;
  const isFromBOT = message.author === 'Mkt Gossip BOT';
  return (
    <ItemContainer>
      <Container owner={isCurrentUserOwner} botMessage={isFromBOT}>
        <AuthorContainer>
          <Author botMessage={isFromBOT}>{` ~ ${message.author}`}</Author>
          <MessageDate>{`${message.sentAt}`}</MessageDate>
        </AuthorContainer>
        <MessageContainer>
          <MessageText>{message.text}</MessageText>
        </MessageContainer>
      </Container>
    </ItemContainer>
  );
};

export default Message;
