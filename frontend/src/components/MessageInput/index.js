import React from 'react';
import { FormControl, Button } from 'react-bootstrap';

import { Container } from './styles';

const MessageInput = ({ setMessage, sendMessage, message }) => {
  return (
    <Container>
      <FormControl
        value={message}
        onChange={e => setMessage(e.target.value)}
        required
        type="text"
        placeholder="Type your message here"
        onKeyPress={event =>
          event.key === 'Enter' ? sendMessage(event) : null
        }
      />
      <Button variant="dark" type="button" onClick={e => sendMessage(e)}>
        Send
      </Button>
    </Container>
  );
};

export default MessageInput;
