import styled from 'styled-components';

export const ItemContainer = styled.div`
  padding: 5px;
`;

export const Container = styled.div`
  padding: 3px;
  border-radius: 10px;

  background-color: ${props => props.customColor };
`;

export const AuthorContainer = styled.div`
  padding: 2px;
  display: flex;
  justify-content: space-between;
`;

export const Author = styled.span`
  font-weight: bold;
  font-size: 14px;

  color: ${props => props.customColor};
`;

export const MessageContainer = styled.div`
  padding: 0 3px 0 3px;
  white-space: pre-wrap;
`;

export const MessageText = styled.span`
  font-size: 12px;
  font-weight: 600;
  color: ${props => props.systemMessage ? '#97A9B4' : '#FFFFFF'};
`;
export const MessageDate = styled.span`
  margin-left: 2px;
  font-size: 10px;
  color: #97A9B4;
`;
